namespace CarServiceBookingSystem.Domain.Entities;

public class TechnicianWorkingHour :BaseIdEntity
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }

    public bool IsClosed { get; set; }
}