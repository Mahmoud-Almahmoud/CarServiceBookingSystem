namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class RescheduleBookingResponse
{
    public int BookingId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int? ServiceBranchId { get; set; }
    public string? ServiceBranchName { get; set; }

    public int? TechnicianId { get; set; }
    public string? TechnicianName { get; set; }

    public decimal ServicePrice { get; set; }
    public decimal TravelFee { get; set; }
    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool TechnicianReassigned { get; set; }
}