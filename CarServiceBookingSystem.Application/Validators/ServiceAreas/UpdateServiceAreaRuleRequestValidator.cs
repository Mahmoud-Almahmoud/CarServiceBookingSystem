using CarServiceBookingSystem.Application.DTOs.ServiceAreas;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.ServiceAreas;

public class UpdateServiceAreaRuleRequestValidator : AbstractValidator<UpdateServiceAreaRuleRequest>
{
    public UpdateServiceAreaRuleRequestValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("CountryCode is required.")
            .MaximumLength(10)
            .WithMessage("CountryCode cannot exceed 10 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Priority cannot be negative.");
    }
}