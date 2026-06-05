namespace CarServiceBookingSystem.Application.DTOs.Receipts;

public class BookingScheduleReceiptInfo
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int DurationMinutes { get; set; }
}