namespace CarServiceBookingSystem.Domain.Entities;

public class TechnicianUnavailableDate:BaseIdEntity
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; } = true;

}