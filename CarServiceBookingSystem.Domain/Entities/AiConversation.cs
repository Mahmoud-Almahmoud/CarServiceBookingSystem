namespace CarServiceBookingSystem.Domain.Entities;

public class AiConversation : BaseIdEntity
{
    public string UserId { get; set; } = string.Empty;

    public int? CarId { get; set; }

    public string? Title { get; set; }

    public bool IsArchived { get; set; }

    public ICollection<AiConversationMessage> Messages { get; set; } = [];

    public ICollection<AiServiceRecommendation> Recommendations { get; set; } = [];
}