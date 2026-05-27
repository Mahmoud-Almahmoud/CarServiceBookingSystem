public class StripeWebhookDto
{
    public string EventId { get; set; } = null!;
    public string EventType { get; set; } = null!;

    public string? PaymentIntentId { get; set; }

    public long Amount { get; set; }

    public string? Currency { get; set; }
}