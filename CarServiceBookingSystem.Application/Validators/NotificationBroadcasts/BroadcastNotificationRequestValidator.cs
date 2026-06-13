using CarServiceBookingSystem.Application.DTOs.NotificationBroadcasts;
using CarServiceBookingSystem.Domain.Enums;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.NotificationBroadcasts;

public class BroadcastNotificationRequestValidator
    : AbstractValidator<BroadcastNotificationRequest>
{
    public BroadcastNotificationRequestValidator()
    {
        RuleFor(x => x.AudienceType)
            .IsInEnum();

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

        RuleFor(x => x.UserIds)
            .Must(x => x.Count > 0)
            .When(x => x.AudienceType == NotificationAudienceType.SpecificUsers)
            .WithMessage("UserIds are required when AudienceType is SpecificUsers.");

        RuleFor(x => x.UserIds)
            .Must(x => x.Count <= 5000)
            .WithMessage("Maximum 5000 specific users can be targeted at once.");

        RuleFor(x => x.Filter)
            .NotNull()
            .When(x => x.AudienceType == NotificationAudienceType.UsersByFilter)
            .WithMessage("Filter is required when AudienceType is UsersByFilter.");

        RuleFor(x => x.EntityType)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.EntityType));

        RuleFor(x => x.ActionUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.ActionUrl));
    }
}