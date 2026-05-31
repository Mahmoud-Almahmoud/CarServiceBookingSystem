using CarServiceBookingSystem.Application.DTOs.Payments;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Payments;

public class CreatePaymentIntentRequestValidator : AbstractValidator<CreatePaymentIntentRequest>
{
    public CreatePaymentIntentRequestValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("BookingId is required.");
    }
}