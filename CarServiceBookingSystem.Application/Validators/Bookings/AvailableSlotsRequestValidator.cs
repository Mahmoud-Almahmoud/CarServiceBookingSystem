using CarServiceBookingSystem.Application.DTOs.Bookings;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Bookings;

public class AvailableSlotsRequestValidator : AbstractValidator<AvailableSlotsRequest>
{
    public AvailableSlotsRequestValidator()
    {
        RuleFor(x => x.Date)
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Date cannot be in the past.");

        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("ServiceId is required.");

        RuleFor(x => x.LocationType)
            .IsInEnum()
            .WithMessage("Invalid booking location type.");
    }
}