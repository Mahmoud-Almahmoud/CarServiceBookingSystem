using CarServiceBookingSystem.Application.DTOs.Technicians;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Technicians;

public class UpdateTechnicianRequestValidator : AbstractValidator<UpdateTechnicianRequest>
{
    public UpdateTechnicianRequestValidator()
    {
        RuleFor(x => x.ServiceBranchId)
            .GreaterThan(0);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Email)
            .MaximumLength(150)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}