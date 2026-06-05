namespace CarServiceBookingSystem.Application.DTOs.Reviews;

public class UpdateBookingReviewRequest
{
    public int Rating { get; set; }

    public string? Comment { get; set; }
}