namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiProviderStatusDto
{
    public bool IsReachable { get; init; }

    public string Provider { get; init; } = "Ollama";

    public string BaseUrl { get; init; } = string.Empty;

    public string ConfiguredModel { get; init; } = string.Empty;

    public bool IsConfiguredModelInstalled { get; init; }

    public List<string> AvailableModels { get; init; } = [];

    public string Message { get; init; } = string.Empty;
}