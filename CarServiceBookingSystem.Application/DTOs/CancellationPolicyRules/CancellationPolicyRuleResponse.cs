namespace CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;

public class CancellationPolicyRuleResponse
{
    public int Id { get; set; }

    public int? ServiceId { get; set; }
    public string? ServiceName { get; set; }

    public int? ServiceBranchId { get; set; }
    public string? ServiceBranchName { get; set; }

    public int HoursBeforeStart { get; set; }

    public decimal RefundPercentage { get; set; }

    public string Scope { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}