using CarServiceBookingSystem.Application.DTOs.Technicians;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Technicians;

public class CreateTechnicianUnavailableDateRequestValidator : AbstractValidator<CreateTechnicianUnavailableDateRequest>
{
    public CreateTechnicianUnavailableDateRequestValidator()
    {
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate);

        RuleFor(x => x)
            .Must(x => x.StartTime == null && x.EndTime == null || x.StartTime != null && x.EndTime != null)
            .WithMessage("StartTime and EndTime must both be provided or both be null.");

        RuleFor(x => x)
            .Must(x => x.StartTime == null || x.EndTime == null || x.StartTime < x.EndTime)
            .WithMessage("StartTime must be before EndTime.");

        RuleFor(x => x.Reason)
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}