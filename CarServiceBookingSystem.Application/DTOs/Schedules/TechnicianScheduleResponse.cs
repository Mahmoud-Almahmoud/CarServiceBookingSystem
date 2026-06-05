namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class TechnicianScheduleResponse
{
    public int TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;

    public int ServiceBranchId { get; set; }
    public string ServiceBranchName { get; set; } = string.Empty;

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public TechnicianScheduleSummaryResponse Summary { get; set; } = new();

    public List<TechnicianWorkingHourScheduleResponse> WorkingHours { get; set; } = new();
    public List<TechnicianUnavailableScheduleResponse> UnavailableDates { get; set; } = new();
    public List<ScheduleBookingResponse> Bookings { get; set; } = new();
}