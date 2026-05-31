namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class UpdateBranchCapacityRuleRequest
{
    public DayOfWeek? DayOfWeek { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public int Capacity { get; set; }

    public bool IsActive { get; set; }
}