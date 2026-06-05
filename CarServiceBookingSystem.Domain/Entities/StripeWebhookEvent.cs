using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;

public class StripeWebhookEvent : BaseEntity
{
    public string StripeEventId { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public WebhookProcessingStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedAt { get; set; }
}