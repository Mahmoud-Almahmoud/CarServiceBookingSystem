namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class TechnicianUnavailableDateResponse
{
    public int Id { get; set; }
    public int TechnicianId { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}