using CarServiceBookingSystem.Application.DTOs.Bookings;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Bookings;

public class RescheduleBookingRequestValidator : AbstractValidator<RescheduleBookingRequest>
{
    public RescheduleBookingRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("StartDate must be in the future.");

        RuleFor(x => x.ServiceBranchId)
            .GreaterThan(0)
            .When(x => x.ServiceBranchId.HasValue);

        RuleFor(x => x.CustomerLatitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.CustomerLatitude.HasValue);

        RuleFor(x => x.CustomerLongitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.CustomerLongitude.HasValue);

        RuleFor(x => x)
            .Must(x =>
                x.CustomerLatitude.HasValue == x.CustomerLongitude.HasValue)
            .WithMessage("CustomerLatitude and CustomerLongitude must both be provided or both be null.");
    }
}