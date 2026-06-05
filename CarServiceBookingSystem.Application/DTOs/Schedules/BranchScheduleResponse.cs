namespace CarServiceBookingSystem.Application.DTOs.Schedules;

public class BranchScheduleResponse
{
    public int ServiceBranchId { get; set; }
    public string ServiceBranchName { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public int TotalBookings { get; set; }
    public int AssignedBookings { get; set; }
    public int UnassignedBookings { get; set; }

    public List<BranchTechnicianScheduleResponse> Technicians { get; set; } = new();

    public List<ScheduleBookingResponse> UnassignedBookingsList { get; set; } = new();
}