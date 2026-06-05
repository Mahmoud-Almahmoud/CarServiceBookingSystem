using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.DTOs.Reviews;

public class PublicServiceReviewQueryRequest : PagedRequest
{
    public int? Rating { get; set; }

}