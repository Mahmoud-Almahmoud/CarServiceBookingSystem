using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Notifications;

public class NotificationQueryRequest
{
    public bool? IsRead { get; set; }

    public NotificationType? Type { get; set; }

    public NotificationSeverity? Severity { get; set; }

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Search { get; set; }

    public string? SortBy { get; set; } = "createdAt";

    public bool Desc { get; set; } = true;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}