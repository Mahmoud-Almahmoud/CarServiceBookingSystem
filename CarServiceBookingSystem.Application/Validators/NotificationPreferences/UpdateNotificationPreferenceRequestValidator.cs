using CarServiceBookingSystem.Application.DTOs.NotificationPreferences;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.NotificationPreferences;

public class UpdateNotificationPreferenceRequestValidator
    : AbstractValidator<UpdateNotificationPreferenceRequest>
{
    public UpdateNotificationPreferenceRequestValidator()
    {
        // No field-specific validation needed yet.
    }
}