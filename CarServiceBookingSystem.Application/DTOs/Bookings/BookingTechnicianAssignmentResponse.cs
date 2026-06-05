namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class BookingTechnicianAssignmentResponse
{
    public int BookingId { get; set; }

    public int TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;

    public int ServiceBranchId { get; set; }
    public string ServiceBranchName { get; set; } = string.Empty;

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string BookingStatus { get; set; } = string.Empty;
}