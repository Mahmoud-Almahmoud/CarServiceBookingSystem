namespace CarServiceBookingSystem.Application.Options;

public class BookingCleanupOptions
{
    public int PendingBookingExpiryMinutes { get; set; } = 30;

    public int BatchSize { get; set; } = 100;

    public bool Enabled { get; set; } = true;
}