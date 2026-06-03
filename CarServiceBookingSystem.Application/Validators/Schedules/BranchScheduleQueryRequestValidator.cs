using CarServiceBookingSystem.Application.DTOs.Schedules;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Schedules;

public class BranchScheduleQueryRequestValidator : AbstractValidator<BranchScheduleQueryRequest>
{
    public BranchScheduleQueryRequestValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();
    }
}