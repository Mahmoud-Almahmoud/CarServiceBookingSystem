namespace CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;

public class CancellationPolicyQuoteResponse
{
    public int? CancellationPolicyRuleId { get; set; }

    public decimal RefundPercentage { get; set; }

    public decimal RefundAmount { get; set; }

    public bool RefundRequired { get; set; }

    public string? PolicyDescription { get; set; }
}