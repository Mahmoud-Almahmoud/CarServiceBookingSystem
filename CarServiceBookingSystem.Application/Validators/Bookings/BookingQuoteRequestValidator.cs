using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Domain.Enums;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Bookings;

public class BookingQuoteRequestValidator : AbstractValidator<BookingQuoteRequest>
{
    public BookingQuoteRequestValidator()
    {
        RuleFor(x => x.CarId)
            .GreaterThan(0)
            .WithMessage("CarId is required.");

        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("ServiceId is required.");

        RuleFor(x => x.LocationType)
            .IsInEnum()
            .WithMessage("Invalid booking location type.");

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("StartDate must be in the future.");

        When(x => x.LocationType == ServiceLocationType.OnUserSite, () =>
        {
            RuleFor(x => x.CustomerLatitude)
                .NotNull()
                .WithMessage("CustomerLatitude is required for customer-site bookings.")
                .InclusiveBetween(-90, 90)
                .WithMessage("CustomerLatitude must be between -90 and 90.");

            RuleFor(x => x.CustomerLongitude)
                .NotNull()
                .WithMessage("CustomerLongitude is required for customer-site bookings.")
                .InclusiveBetween(-180, 180)
                .WithMessage("CustomerLongitude must be between -180 and 180.");

            RuleFor(x => x.CustomerCountryCode)
                .NotEmpty()
                .WithMessage("CustomerCountryCode is required for customer-site bookings.")
                .MaximumLength(10)
                .WithMessage("CustomerCountryCode cannot exceed 10 characters.");

            RuleFor(x => x.CustomerCity)
                .NotEmpty()
                .WithMessage("CustomerCity is required for customer-site bookings.")
                .MaximumLength(100)
                .WithMessage("CustomerCity cannot exceed 100 characters.");
        });
    }
}