using CarServiceBookingSystem.Application.DTOs.Cars;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Cars;

public class UpdateCarRequestValidator : AbstractValidator<UpdateCarRequest>
{
    public UpdateCarRequestValidator()
    {
        RuleFor(x => x.CarTrimId)
            .GreaterThan(0);

        RuleFor(x => x.PlateNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}