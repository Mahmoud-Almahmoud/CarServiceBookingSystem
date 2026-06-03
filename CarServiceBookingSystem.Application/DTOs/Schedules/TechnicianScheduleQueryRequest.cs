namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class TechnicianScheduleQueryRequest
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public bool IncludeCancelled { get; set; } = false;
}