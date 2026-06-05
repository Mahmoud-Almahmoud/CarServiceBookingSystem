using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;

public class CancellationPolicyRuleQueryRequest : PagedRequest
{
    public int? ServiceId { get; set; }

    public int? ServiceBranchId { get; set; }

    public bool? IsActive { get; set; }
}