using CarServiceBookingSystem.Application.DTOs.Reviews;
using FluentValidation;

namespace CarServiceBookingSystem.Application.Validators.Reviews;

public class UpdateBookingReviewRequestValidator : AbstractValidator<UpdateBookingReviewRequest>
{
    public UpdateBookingReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}