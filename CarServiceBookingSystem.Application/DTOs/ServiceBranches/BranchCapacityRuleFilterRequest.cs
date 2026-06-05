namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class BranchCapacityRuleFilterRequest
{
    public DayOfWeek? DayOfWeek { get; set; }

    public bool? IsActive { get; set; }

    public string? SortBy { get; set; }

    public bool Desc { get; set; } = true;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}