using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Domain.Entities;

public class AiConversationMessage : BaseIdEntity
{

    public int ConversationId { get; set; }

    public AiConversation Conversation { get; set; } = null!;

    public AiMessageRole Role { get; set; }

    public string Content { get; set; } = string.Empty;

}