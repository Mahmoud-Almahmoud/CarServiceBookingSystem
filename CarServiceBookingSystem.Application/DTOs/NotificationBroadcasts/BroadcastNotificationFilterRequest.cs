namespace CarServiceBookingSystem.Application.DTOs.NotificationBroadcasts;

public class BroadcastNotificationFilterRequest
{
    public int? ServiceId { get; set; }

    public int? CarBrandId { get; set; }

    public int? CarModelId { get; set; }

    public int? ServiceBranchId { get; set; }

    public string? City { get; set; }

    public bool? OnlyUsersWithBookings { get; set; }

    public bool? OnlyUsersWithoutBookings { get; set; }

    public DateTime? LastBookingFrom { get; set; }

    public DateTime? LastBookingTo { get; set; }

    public decimal? MinTotalSpent { get; set; }

    public DateTime? RegisteredFrom { get; set; }

    public DateTime? RegisteredTo { get; set; }
}