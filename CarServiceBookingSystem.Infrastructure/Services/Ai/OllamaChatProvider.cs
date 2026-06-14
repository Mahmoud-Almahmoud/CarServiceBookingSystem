using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class OllamaChatProvider : IAiChatProvider
{
    private readonly HttpClient _httpClient;
    private readonly AiAdvisorOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OllamaChatProvider(
        HttpClient httpClient,
        IOptions<AiAdvisorOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GetJsonChatCompletionAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        var request = new OllamaChatRequest
        {
            Model = _options.Model,
            Stream = false,
            Format = "json",
            Messages =
            [
                new OllamaMessage
                {
                    Role = "system",
                    Content = systemPrompt
                },
                new OllamaMessage
                {
                    Role = "user",
                    Content = userPrompt
                }
            ],
            Options = new OllamaRequestOptions
            {
                Temperature = 0.2,
                NumPredict = 800
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "/api/chat",
            request,
            JsonOptions,
            cancellationToken);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Ollama request failed with status code {(int)response.StatusCode}: {body}");
        }

        var result = JsonSerializer.Deserialize<OllamaChatResponse>(
            body,
            JsonOptions);

        return result?.Message?.Content ?? string.Empty;
    }

    private sealed class OllamaChatRequest
    {
        public string Model { get; init; } = string.Empty;

        public List<OllamaMessage> Messages { get; init; } = [];

        public bool Stream { get; init; }

        public string Format { get; init; } = "json";

        public OllamaRequestOptions Options { get; init; } = new();
    }

    private sealed class OllamaMessage
    {
        public string Role { get; init; } = string.Empty;

        public string Content { get; init; } = string.Empty;
    }

    private sealed class OllamaRequestOptions
    {
        public double Temperature { get; init; }

        public int NumPredict { get; init; }
    }

    private sealed class OllamaChatResponse
    {
        public OllamaMessage? Message { get; init; }

        public bool Done { get; init; }
    }
}