namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiChatProvider
{
    Task<string> GetJsonChatCompletionAsync(
     string baseUrl,
     string model,
     int timeoutSeconds,
     string systemPrompt,
     string userPrompt,
     CancellationToken cancellationToken = default);
}