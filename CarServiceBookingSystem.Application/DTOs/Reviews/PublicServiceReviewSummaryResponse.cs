using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.DTOs.Reviews;

public class PublicServiceReviewSummaryResponse
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int TotalReviews { get; set; }

    public decimal AverageRating { get; set; }

    public int FiveStarCount { get; set; }

    public int FourStarCount { get; set; }

    public int ThreeStarCount { get; set; }

    public int TwoStarCount { get; set; }

    public int OneStarCount { get; set; }

    public PagedResponse<PublicServiceReviewResponse> Reviews { get; set; } = null!;
}