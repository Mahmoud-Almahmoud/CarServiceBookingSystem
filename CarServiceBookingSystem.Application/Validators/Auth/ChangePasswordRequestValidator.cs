using CarServiceBookingSystem.Application.DTOs.Auth;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Auth;

public class ChangePasswordRequestValidator
    : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6);
    }
}