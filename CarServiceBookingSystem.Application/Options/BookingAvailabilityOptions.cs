namespace CarServiceBookingSystem.Application.Options;

public class BookingAvailabilityOptions
{
    public TimeSpan WorkDayStart { get; set; } = new(9, 0, 0);

    public TimeSpan WorkDayEnd { get; set; } = new(18, 0, 0);

    public int SlotStepMinutes { get; set; } = 30;

    public int MinimumNoticeMinutes { get; set; } = 60;
}