using CarServiceBookingSystem.Application.DTOs.Notifications;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Notifications;

public class NotificationQueryRequestValidator : AbstractValidator<NotificationQueryRequest>
{
    private static readonly string[] AllowedSortColumns =
    {
        "createdat",
        "type",
        "severity",
        "isread"
    };

    public NotificationQueryRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.EntityType)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.EntityType));

        RuleFor(x => x.Search)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrWhiteSpace(x) ||
                       AllowedSortColumns.Contains(x.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortColumns)}.");

        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue ||
                       !x.ToDate.HasValue ||
                       x.FromDate.Value <= x.ToDate.Value)
            .WithMessage("FromDate must be earlier than or equal to ToDate.");
    }
}