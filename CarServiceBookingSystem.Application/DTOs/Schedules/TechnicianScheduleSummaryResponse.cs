namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class TechnicianScheduleSummaryResponse
{
    public int TotalBookings { get; set; }

    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int AssignedBookings { get; set; }
    public int InProgressBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
}