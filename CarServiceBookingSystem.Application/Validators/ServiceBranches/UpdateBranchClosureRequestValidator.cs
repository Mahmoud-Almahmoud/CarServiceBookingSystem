using CarServiceBookingSystem.Application.DTOs.ServiceBranches;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.ServiceBranches;

public class UpdateBranchClosureRequestValidator : AbstractValidator<UpdateBranchClosureRequest>
{
    public UpdateBranchClosureRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("EndDate is required.");

        RuleFor(x => x)
            .Must(x => x.StartDate.Date <= x.EndDate.Date)
            .WithMessage("StartDate cannot be after EndDate.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid closure type.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.")
            .MaximumLength(300)
            .WithMessage("Reason cannot exceed 300 characters.");

        When(x => !x.IsFullDay, () =>
        {
            RuleFor(x => x.StartTime)
                .NotNull()
                .WithMessage("StartTime is required for partial-day closures.");

            RuleFor(x => x.EndTime)
                .NotNull()
                .WithMessage("EndTime is required for partial-day closures.");

            RuleFor(x => x)
                .Must(x => x.StartTime < x.EndTime)
                .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
                .WithMessage("StartTime must be before EndTime.");
        });
    }
}