namespace CarServiceBookingSystem.Application.DTOs.PushNotifications;

public class PushSubscriptionRequest
{
    public string Endpoint { get; set; } = string.Empty;

    public PushSubscriptionKeysRequest Keys { get; set; } = new();
}

public class PushSubscriptionKeysRequest
{
    public string P256dh { get; set; } = string.Empty;

    public string Auth { get; set; } = string.Empty;
}