namespace CarServiceBookingSystem.Domain.Entities;

public class BookingReview : BaseIdEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsVisible { get; set; } = true;

}