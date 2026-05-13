using CarServiceBookingSystem.Application.DTOs.Auth;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Auth;

public class ResendEmailConfirmationRequestValidator
    : AbstractValidator<ResendEmailConfirmationRequest>
{
    public ResendEmailConfirmationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}