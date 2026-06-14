namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiConversationMessageDto
{
    public int Id { get; init; }

    public int ConversationId { get; init; }

    public string Role { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }
}