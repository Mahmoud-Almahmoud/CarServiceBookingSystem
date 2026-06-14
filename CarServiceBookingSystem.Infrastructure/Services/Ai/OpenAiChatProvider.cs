using CarServiceBookingSystem.Application.Interfaces.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class OpenAiChatProvider : IAiChatProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAiChatProvider> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenAiChatProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenAiChatProvider> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetJsonChatCompletionAsync(
        string baseUrl,
        string model,
        int timeoutSeconds,
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is not configured. Set OpenAI__ApiKey environment variable.");

        var effectiveBaseUrl = string.IsNullOrWhiteSpace(baseUrl)
            ? _configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com"
            : baseUrl;

        _httpClient.BaseAddress = new Uri(effectiveBaseUrl.TrimEnd('/'));
        _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "/v1/chat/completions");

        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        requestMessage.Content = JsonContent.Create(
            new
            {
                model,
                temperature = 0.1,
                max_tokens = 600,
                response_format = BuildResponseFormat(),
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = systemPrompt
                    },
                    new
                    {
                        role = "user",
                        content = userPrompt
                    }
                }
            },
            options: JsonOptions);

        try
        {
            using var response = await _httpClient.SendAsync(
                requestMessage,
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "OpenAI request failed. StatusCode={StatusCode}, Body={Body}",
                    (int)response.StatusCode,
                    body);

                throw new InvalidOperationException(
                    $"OpenAI request failed with status code {(int)response.StatusCode}.");
            }

            var result = JsonSerializer.Deserialize<OpenAiChatCompletionResponse>(
                body,
                JsonOptions);

            return result?.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                $"OpenAI request timed out after {timeoutSeconds} seconds.",
                ex);
        }
    }

    private static object BuildResponseFormat()
    {
        return new
        {
            type = "json_schema",
            json_schema = new
            {
                name = "service_advisor_response",
                strict = true,
                schema = new
                {
                    type = "object",
                    additionalProperties = false,
                    properties = new
                    {
                        reply = new
                        {
                            type = "string"
                        },
                        urgency = new
                        {
                            type = "string",
                            @enum = new[] { "Low", "Medium", "High", "Unknown" }
                        },
                        suggestedServices = new
                        {
                            type = "array",
                            items = new
                            {
                                type = "object",
                                additionalProperties = false,
                                properties = new
                                {
                                    serviceId = new
                                    {
                                        type = "integer"
                                    },
                                    reason = new
                                    {
                                        type = "string"
                                    },
                                    confidence = new
                                    {
                                        type = "number"
                                    }
                                },
                                required = new[]
                                {
                                    "serviceId",
                                    "reason",
                                    "confidence"
                                }
                            }
                        },
                        followUpQuestions = new
                        {
                            type = "array",
                            items = new
                            {
                                type = "string"
                            }
                        }
                    },
                    required = new[]
                    {
                        "reply",
                        "urgency",
                        "suggestedServices",
                        "followUpQuestions"
                    }
                }
            }
        };
    }

    private sealed class OpenAiChatCompletionResponse
    {
        public List<OpenAiChoice> Choices { get; init; } = [];
    }

    private sealed class OpenAiChoice
    {
        public OpenAiMessage? Message { get; init; }
    }

    private sealed class OpenAiMessage
    {
        public string Content { get; init; } = string.Empty;
    }
}