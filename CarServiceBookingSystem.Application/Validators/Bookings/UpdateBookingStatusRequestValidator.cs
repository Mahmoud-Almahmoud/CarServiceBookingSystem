using CarServiceBookingSystem.Application.DTOs.Bookings;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Bookings;

public class UpdateBookingStatusRequestValidator : AbstractValidator<UpdateBookingStatusRequest>
{
    public UpdateBookingStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}