using CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.CancellationPolicyRules;

public class CreateCancellationPolicyRuleRequestValidator : AbstractValidator<CreateCancellationPolicyRuleRequest>
{
    public CreateCancellationPolicyRuleRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .When(x => x.ServiceId.HasValue);

        RuleFor(x => x.ServiceBranchId)
            .GreaterThan(0)
            .When(x => x.ServiceBranchId.HasValue);

        RuleFor(x => x.HoursBeforeStart)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(720);

        RuleFor(x => x.RefundPercentage)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.Description)
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}