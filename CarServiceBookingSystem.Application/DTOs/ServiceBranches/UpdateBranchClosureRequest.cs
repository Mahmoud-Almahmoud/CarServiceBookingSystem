using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class UpdateBranchClosureRequest
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsFullDay { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public BranchClosureType Type { get; set; }

    public string Reason { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}