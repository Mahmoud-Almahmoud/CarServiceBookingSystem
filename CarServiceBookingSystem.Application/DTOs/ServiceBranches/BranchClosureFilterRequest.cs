using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class BranchClosureFilterRequest : PagedRequest
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public BranchClosureType? Type { get; set; }

    public bool? IsActive { get; set; }
}