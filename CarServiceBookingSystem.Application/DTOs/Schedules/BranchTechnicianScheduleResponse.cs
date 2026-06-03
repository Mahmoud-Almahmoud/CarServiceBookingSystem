namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class BranchTechnicianScheduleResponse
{
    public int TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<TechnicianWorkingHourScheduleResponse> WorkingHours { get; set; } = new();
    public List<TechnicianUnavailableScheduleResponse> UnavailableDates { get; set; } = new();
    public List<ScheduleBookingResponse> Bookings { get; set; } = new();
}