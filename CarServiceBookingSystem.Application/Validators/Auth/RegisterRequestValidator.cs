using CarServiceBookingSystem.Application.DTOs.Auth;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}