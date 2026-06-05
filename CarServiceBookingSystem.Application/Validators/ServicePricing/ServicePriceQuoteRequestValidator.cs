using CarServiceBookingSystem.Application.DTOs.ServicePricing;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.ServicePricing;

public class ServicePriceQuoteRequestValidator : AbstractValidator<ServicePriceQuoteRequest>
{
    public ServicePriceQuoteRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("ServiceId is required.");

        RuleFor(x => x.CarId)
            .GreaterThan(0)
            .WithMessage("CarId is required.");
    }
}