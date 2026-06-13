namespace CarServiceBookingSystem.Application.DTOs.PushNotifications;

public class PushSubscriptionResponse
{
    public int Id { get; set; }

    public string Endpoint { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}