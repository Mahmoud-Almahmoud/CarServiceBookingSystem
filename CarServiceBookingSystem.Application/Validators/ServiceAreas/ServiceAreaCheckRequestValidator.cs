using CarServiceBookingSystem.Application.DTOs.ServiceAreas;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.ServiceAreas;

public class ServiceAreaCheckRequestValidator : AbstractValidator<ServiceAreaCheckRequest>
{
    public ServiceAreaCheckRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0)
            .WithMessage("ServiceId is required.");

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("CountryCode is required.")
            .MaximumLength(10)
            .WithMessage("CountryCode cannot exceed 10 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters.");
    }
}