namespace CarServiceBookingSystem.Domain.Entities;

public class RefreshToken : BaseIdEntity
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public string UserId { get; set; } = string.Empty;
    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public string? CreatedByIp { get; set; }

    public string? RevokedByIp { get; set; }

    public string? RevocationReason { get; set; }

    public string? Device { get; set; }
    public string? DeviceFingerprintHash { get; set; }
    public int? ParentTokenId { get; set; }

    public int? ReplacedByTokenId { get; set; }

    public string TokenFamilyId { get; set; } = string.Empty;
}