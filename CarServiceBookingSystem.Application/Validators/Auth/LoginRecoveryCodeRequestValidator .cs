using CarServiceBookingSystem.Application.DTOs.Auth;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Auth;

public class LoginRecoveryCodeRequestValidator
    : AbstractValidator<LoginRecoveryCodeRequest>
{
    public LoginRecoveryCodeRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.RecoveryCode)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(20);
    }
}