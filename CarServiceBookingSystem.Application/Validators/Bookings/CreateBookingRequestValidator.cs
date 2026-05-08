using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Domain.Enums;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Bookings;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.CarId)
            .GreaterThan(0);

        RuleFor(x => x.ServiceId)
            .GreaterThan(0);

        RuleFor(x => x.LocationType)
            .IsInEnum();

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Start date must be in the future.");
    }
}