using CarServiceBookingSystem.Application.DTOs.Technicians;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Technicians;

public class AddTechnicianServiceRequestValidator : AbstractValidator<AddTechnicianServiceRequest>
{
    public AddTechnicianServiceRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0);
    }
}