namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class TechnicianUnavailableScheduleResponse
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public string? Reason { get; set; }
}