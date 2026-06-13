using CarServiceBookingSystem.Application.DTOs.NotificationPreferences;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.NotificationPreferences;

public class BulkUpdateNotificationPreferencesRequestValidator
    : AbstractValidator<BulkUpdateNotificationPreferencesRequest>
{
    public BulkUpdateNotificationPreferencesRequestValidator()
    {
        RuleFor(x => x.Preferences)
            .NotNull()
            .Must(x => x.Count <= 100)
            .WithMessage("Maximum 100 preferences can be updated at once.");

        RuleForEach(x => x.Preferences)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Type)
                    .IsInEnum();
            });

        RuleFor(x => x.Preferences)
            .Must(x => x.Select(p => p.Type).Distinct().Count() == x.Count)
            .WithMessage("Duplicate notification preference types are not allowed.");
    }
}