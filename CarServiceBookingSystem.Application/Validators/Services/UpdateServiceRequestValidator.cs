using CarServiceBookingSystem.Application.DTOs.Services;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Services;

public class UpdateServiceRequestValidator : AbstractValidator<UpdateServiceRequest>
{
    public UpdateServiceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.DurationInMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(1440);
    }
}