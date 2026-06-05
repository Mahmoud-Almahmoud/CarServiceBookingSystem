namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class TechnicianWorkingHourResponse
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }

    public bool IsClosed { get; set; }
}