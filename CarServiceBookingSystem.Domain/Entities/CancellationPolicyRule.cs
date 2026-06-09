using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Domain.Entities;

public class CancellationPolicyRule : BaseIdEntity
{
    public int? ServiceId { get; set; }
    public Service? Service { get; set; }

    public int? ServiceBranchId { get; set; }
    public ServiceBranch? ServiceBranch { get; set; }

    public int HoursBeforeStart { get; set; }

    public decimal RefundPercentage { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public CancellationPolicyRuleScope Scope
    {
        get
        {
            if (ServiceId.HasValue && ServiceBranchId.HasValue)
                return CancellationPolicyRuleScope.ServiceAndBranch;

            if (ServiceId.HasValue)
                return CancellationPolicyRuleScope.Service;

            if (ServiceBranchId.HasValue)
                return CancellationPolicyRuleScope.Branch;

            return CancellationPolicyRuleScope.Global;
        }
    }
}