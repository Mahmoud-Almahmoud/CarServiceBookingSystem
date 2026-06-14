using CarServiceBookingSystem.Application.Interfaces.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using Stripe;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class OllamaChatProvider : IAiChatProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OllamaChatProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetJsonChatCompletionAsync(
        string baseUrl,
        string model,
        int timeoutSeconds,
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("OllamaAiClient");

        client.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
        client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        var request = new OllamaChatRequest
        {
            Model = model,
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
                NumPredict = 350
            }
        };

        HttpResponseMessage response;

        try
        {
            response = await client.PostAsJsonAsync(
                "/api/chat",
                request,
                JsonOptions,
                cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                $"Ollama request timed out after {client.Timeout} seconds. " +
                "Increase AiAdvisor:TimeoutSeconds, use a smaller model, or reduce the prompt size.",
                ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new InvalidOperationException(
                "Ollama request was canceled before it completed.",
                ex);
        }

        using (response)
        {
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
    }

    private sealed class OllamaChatRequest
    {
        public string Model { get; init; } = string.Empty;

        public List<OllamaMessage> Messages { get; init; } = [];

        public bool Stream { get; init; }

        public string Format { get; init; } = "json";
        public string KeepAlive { get; init; } = "10m";

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