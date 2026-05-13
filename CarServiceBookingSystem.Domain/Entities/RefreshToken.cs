namespace CarServiceBookingSystem.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public string UserId { get; set; } = string.Empty;
    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public string? RevokedByIp { get; set; }

    public string? RevocationReason { get; set; }

    public string? Device { get; set; }
}