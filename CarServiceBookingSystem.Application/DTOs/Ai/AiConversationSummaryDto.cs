namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiConversationSummaryDto
{
    public int Id { get; init; }

    public int? CarId { get; init; }

    public string? Title { get; init; }

    public bool IsArchived { get; init; }

    public int MessagesCount { get; init; }

    public string? LastMessagePreview { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}