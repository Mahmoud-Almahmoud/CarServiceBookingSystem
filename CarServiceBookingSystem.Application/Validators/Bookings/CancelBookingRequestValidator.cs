using CarServiceBookingSystem.Application.DTOs.Bookings;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Bookings;

public class CancelBookingRequestValidator : AbstractValidator<CancelBookingRequest>
{
    public CancelBookingRequestValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}