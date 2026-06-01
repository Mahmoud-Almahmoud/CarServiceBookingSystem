using CarServiceBookingSystem.Application.DTOs.Bookings;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Bookings;

public class AssignTechnicianRequestValidator : AbstractValidator<AssignTechnicianRequest>
{
    public AssignTechnicianRequestValidator()
    {
        RuleFor(x => x.TechnicianId)
            .GreaterThan(0);
    }
}