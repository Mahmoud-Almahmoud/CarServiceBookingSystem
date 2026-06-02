namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class BranchCapacityRuleResponse
{
    public int Id { get; set; }

    public int ServiceBranchId { get; set; }

    public string BranchName { get; set; } = string.Empty;

    public DayOfWeek? DayOfWeek { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public int Capacity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}