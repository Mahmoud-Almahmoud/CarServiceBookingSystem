using CarServiceBookingSystem.Application.DTOs.Auth;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Auth;

public class ForgotPasswordRequestValidator
    : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}