namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class UpdateTechnicianUnavailableDateRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; } = true;
}