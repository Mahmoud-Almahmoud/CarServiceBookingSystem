namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class BranchWorkingHourResponse
{
    public int Id { get; set; }

    public int ServiceBranchId { get; set; }

    public string BranchName { get; set; } = string.Empty;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan OpenTime { get; set; }

    public TimeSpan CloseTime { get; set; }

    public bool IsClosed { get; set; }
}