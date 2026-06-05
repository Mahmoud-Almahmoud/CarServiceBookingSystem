namespace CarServiceBookingSystem.Application.DTOs.Reviews;

public class BookingReviewResponse
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string? CustomerName { get; set; }

    public string? CustomerEmail { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsVisible { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int? ServiceBranchId { get; set; }

    public string? ServiceBranchName { get; set; }

    public int? TechnicianId { get; set; }

    public string? TechnicianName { get; set; }

    public DateTime BookingStartDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}