namespace CarServiceBookingSystem.Application.DTOs.Auth;

public class ActiveSessionResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? Device { get; set; }
    public bool IsCurrentSession { get; set; }
}