namespace CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;

public class CreateCancellationPolicyRuleRequest
{
    public int? ServiceId { get; set; }

    public int? ServiceBranchId { get; set; }

    public int HoursBeforeStart { get; set; }

    public decimal RefundPercentage { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}