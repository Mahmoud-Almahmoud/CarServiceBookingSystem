using CarServiceBookingSystem.Application.DTOs.Notifications;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Notifications;

public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .MaximumLength(450);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Severity)
            .IsInEnum();

        RuleFor(x => x.EntityType)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.EntityType));

        RuleFor(x => x.ActionUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.ActionUrl));

        RuleFor(x => x.EntityId)
            .GreaterThan(0)
            .When(x => x.EntityId.HasValue);
    }
}