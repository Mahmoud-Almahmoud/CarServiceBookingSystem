namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class ScheduleBookingResponse
{
    public int BookingId { get; set; }

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    public int CarId { get; set; }
    public string CarDisplayName { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;

    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }

    public int? TechnicianId { get; set; }
    public string? TechnicianName { get; set; }

    public int? ServiceBranchId { get; set; }
    public string? ServiceBranchName { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }
}