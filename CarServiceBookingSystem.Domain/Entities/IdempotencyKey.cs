namespace CarServiceBookingSystem.Domain.Entities;

public class IdempotencyKey : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Key { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public string RequestHash { get; set; } = null!;
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}