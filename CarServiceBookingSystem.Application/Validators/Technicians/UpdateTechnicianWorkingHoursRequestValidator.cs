using CarServiceBookingSystem.Application.DTOs.Technicians;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Technicians;

public class UpdateTechnicianWorkingHoursRequestValidator : AbstractValidator<UpdateTechnicianWorkingHoursRequest>
{
    public UpdateTechnicianWorkingHoursRequestValidator()
    {
        RuleFor(x => x.WorkingHours)
            .NotNull();

        RuleForEach(x => x.WorkingHours)
            .ChildRules(hour =>
            {
                hour.RuleFor(x => x.OpenTime)
                    .LessThan(x => x.CloseTime)
                    .When(x => !x.IsClosed)
                    .WithMessage("OpenTime must be before CloseTime.");

                hour.RuleFor(x => x.CloseTime)
                    .GreaterThan(x => x.OpenTime)
                    .When(x => !x.IsClosed)
                    .WithMessage("CloseTime must be after OpenTime.");
            });

        RuleFor(x => x.WorkingHours)
            .Must(x => x.Select(h => h.DayOfWeek).Distinct().Count() == x.Count)
            .WithMessage("Duplicate DayOfWeek values are not allowed.");
    }
}