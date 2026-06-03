namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class TechnicianWorkingHourScheduleResponse
{
    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }

    public bool IsClosed { get; set; }
}