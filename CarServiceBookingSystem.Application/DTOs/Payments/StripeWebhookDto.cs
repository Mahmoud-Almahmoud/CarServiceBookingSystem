public class StripeWebhookDto
{
    public string EventId { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string? PaymentIntentId { get; set; }
    public long Amount { get; set; }
    public string? Currency { get; set; }
    public string? FailureReason { get; set; }
    public string? FailureCode { get; set; }
    public string? DeclineCode { get; set; }
}