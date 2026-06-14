using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class OllamaProviderStatusProvider : IAiProviderStatusProvider
{
    private readonly HttpClient _httpClient;
    private readonly AiAdvisorOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OllamaProviderStatusProvider(
        HttpClient httpClient,
        IOptions<AiAdvisorOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<AiProviderStatusDto> GetStatusAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(
                "/api/tags",
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new AiProviderStatusDto
                {
                    IsReachable = false,
                    Provider = _options.Provider,
                    BaseUrl = _options.BaseUrl,
                    ConfiguredModel = _options.Model,
                    IsConfiguredModelInstalled = false,
                    Message = $"Ollama returned status code {(int)response.StatusCode}."
                };
            }

            var tagsResponse = JsonSerializer.Deserialize<OllamaTagsResponse>(
                body,
                JsonOptions);

            var models = tagsResponse?.Models
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .OrderBy(x => x)
                .ToList() ?? [];

            var configuredModelInstalled = models.Any(x =>
                x.Equals(_options.Model, StringComparison.OrdinalIgnoreCase));

            return new AiProviderStatusDto
            {
                IsReachable = true,
                Provider = _options.Provider,
                BaseUrl = _options.BaseUrl,
                ConfiguredModel = _options.Model,
                IsConfiguredModelInstalled = configuredModelInstalled,
                AvailableModels = models,
                Message = configuredModelInstalled
                    ? "Ollama is reachable."
                    : "Ollama is reachable, but the configured model is not installed."
            };
        }
        catch (Exception ex)
        {
            return new AiProviderStatusDto
            {
                IsReachable = false,
                Provider = _options.Provider,
                BaseUrl = _options.BaseUrl,
                ConfiguredModel = _options.Model,
                IsConfiguredModelInstalled = false,
                AvailableModels = [],
                Message = $"Ollama is not reachable: {ex.Message}"
            };
        }
    }

    private sealed class OllamaTagsResponse
    {
        public List<OllamaModelInfo> Models { get; init; } = [];
    }

    private sealed class OllamaModelInfo
    {
        public string Name { get; init; } = string.Empty;
    }
}