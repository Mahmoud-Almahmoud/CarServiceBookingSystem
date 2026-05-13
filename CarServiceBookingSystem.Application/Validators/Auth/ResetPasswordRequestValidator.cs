using CarServiceBookingSystem.Application.DTOs.Auth;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Auth;

public class ResetPasswordRequestValidator
    : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6);
    }
}