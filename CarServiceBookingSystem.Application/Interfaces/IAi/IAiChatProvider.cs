namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiChatProvider
{
    Task<string> GetJsonChatCompletionAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default);
}