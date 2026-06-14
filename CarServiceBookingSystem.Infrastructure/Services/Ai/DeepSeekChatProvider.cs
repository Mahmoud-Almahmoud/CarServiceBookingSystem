using CarServiceBookingSystem.Application.Interfaces.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class DeepSeekChatProvider : IAiChatProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeepSeekChatProvider> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DeepSeekChatProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DeepSeekChatProvider> logger)
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
        var apiKey = _configuration["DeepSeek:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "DeepSeek API key is not configured. Set DeepSeek__ApiKey environment variable.");
        }

        var effectiveBaseUrl = string.IsNullOrWhiteSpace(baseUrl)
            ? _configuration["DeepSeek:BaseUrl"] ?? "https://api.deepseek.com"
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

                // DeepSeek supports JSON output in its API feature list.
                // We still validate and parse the output in AiServiceAdvisorService.
                response_format = new
                {
                    type = "json_object"
                },

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
                    "DeepSeek request failed. StatusCode={StatusCode}, Body={Body}",
                    (int)response.StatusCode,
                    body);

                throw new InvalidOperationException(
                    $"DeepSeek request failed with status code {(int)response.StatusCode}.");
            }

            var result = JsonSerializer.Deserialize<DeepSeekChatCompletionResponse>(
                body,
                JsonOptions);

            return result?.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                $"DeepSeek request timed out after {timeoutSeconds} seconds.",
                ex);
        }
    }

    private sealed class DeepSeekChatCompletionResponse
    {
        public List<DeepSeekChoice> Choices { get; init; } = [];
    }

    private sealed class DeepSeekChoice
    {
        public DeepSeekMessage? Message { get; init; }
    }

    private sealed class DeepSeekMessage
    {
        public string Content { get; init; } = string.Empty;
    }
}