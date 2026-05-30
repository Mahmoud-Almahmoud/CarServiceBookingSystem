using CarServiceBookingSystem.Application.DTOs.ServicePricing;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.ServicePricing;

public class UpdateServicePriceRuleRequestValidator : AbstractValidator<UpdateServicePriceRuleRequest>
{
    public UpdateServicePriceRuleRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("ServiceId is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("DurationMinutes must be greater than zero.");

        RuleFor(x => x.CarYearId)
            .GreaterThanOrEqualTo(1900)
            .When(x => x.CarYearId.HasValue)
            .WithMessage("CarYearId is invalid.");

        RuleFor(x => x.CarTrimId)
            .GreaterThanOrEqualTo(1)
            .When(x => x.CarTrimId.HasValue)
            .WithMessage("CarTrimId is invalid.");

        RuleFor(x => x)
            .Must(x =>
                x.CarBrandId.HasValue ||
                x.CarModelId.HasValue ||
                x.CarTrimId.HasValue ||
                x.CarYearId.HasValue)
            .WithMessage("At least one pricing condition is required.");
    }
}