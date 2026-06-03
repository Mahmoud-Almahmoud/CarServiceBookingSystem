namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class BranchScheduleQueryRequest
{
    public DateTime Date { get; set; }

    public bool IncludeCancelled { get; set; } = false;
}