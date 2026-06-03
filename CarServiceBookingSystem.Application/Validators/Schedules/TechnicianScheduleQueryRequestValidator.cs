using CarServiceBookingSystem.Application.DTOs.Schedules;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Schedules;

public class TechnicianScheduleQueryRequestValidator : AbstractValidator<TechnicianScheduleQueryRequest>
{
    public TechnicianScheduleQueryRequestValidator()
    {
        RuleFor(x => x.FromDate)
            .NotEmpty();

        RuleFor(x => x.ToDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.FromDate);

        RuleFor(x => x)
            .Must(x => (x.ToDate.Date - x.FromDate.Date).TotalDays <= 31)
            .WithMessage("Schedule range cannot exceed 31 days.");
    }
}