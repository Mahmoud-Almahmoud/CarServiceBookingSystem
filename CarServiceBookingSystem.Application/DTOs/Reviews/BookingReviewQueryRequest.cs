using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.DTOs.Reviews;

public class BookingReviewQueryRequest : PagedRequest
{
    public string? UserId { get; set; }

    public int? BookingId { get; set; }

    public int? ServiceId { get; set; }

    public int? Rating { get; set; }

    public bool? IsVisible { get; set; }

}