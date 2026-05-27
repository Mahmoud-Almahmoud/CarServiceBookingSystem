public class StripeWebhookEvent
{
    public int Id { get; set; }

    public string StripeEventId { get; set; } = null!;
    public string EventType { get; set; } = null!;

    public bool Processed { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
}