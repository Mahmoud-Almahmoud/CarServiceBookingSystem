namespace CarServiceBookingSystem.Application.DTOs.Reviews;

public class CreateBookingReviewRequest
{
    public int Rating { get; set; }

    public string? Comment { get; set; }
}