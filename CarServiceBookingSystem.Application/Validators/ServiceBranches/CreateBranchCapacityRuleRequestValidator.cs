using CarServiceBookingSystem.Application.DTOs.ServiceBranches;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.ServiceBranches;

public class CreateBranchCapacityRuleRequestValidator : AbstractValidator<CreateBranchCapacityRuleRequest>
{
    public CreateBranchCapacityRuleRequestValidator()
    {
        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be greater than zero.");

        RuleFor(x => x)
            .Must(x =>
                (!x.StartTime.HasValue && !x.EndTime.HasValue) ||
                (x.StartTime.HasValue && x.EndTime.HasValue))
            .WithMessage("StartTime and EndTime must be provided together.");

        RuleFor(x => x)
            .Must(x =>
                !x.StartTime.HasValue ||
                x.DayOfWeek.HasValue)
            .WithMessage("DayOfWeek is required when StartTime and EndTime are provided.");

        RuleFor(x => x)
            .Must(x =>
                !x.StartTime.HasValue ||
                x.StartTime.Value < x.EndTime!.Value)
            .WithMessage("StartTime must be before EndTime.");
    }
}