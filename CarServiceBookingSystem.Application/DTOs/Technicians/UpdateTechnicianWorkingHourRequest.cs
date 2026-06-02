namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class UpdateTechnicianWorkingHourRequest
{
    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }

    public bool IsClosed { get; set; }
}