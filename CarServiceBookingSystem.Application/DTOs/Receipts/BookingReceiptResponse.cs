namespace CarServiceBookingSystem.Application.DTOs.Receipts;

public class BookingReceiptResponse
{
    public int BookingId { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;

    public DateTime IssuedAt { get; set; }

    public string BookingStatus { get; set; } = string.Empty;

    public CustomerReceiptInfo Customer { get; set; } = new();

    public CarReceiptInfo Car { get; set; } = new();

    public ServiceReceiptInfo Service { get; set; } = new();

    public BranchReceiptInfo? Branch { get; set; }

    public TechnicianReceiptInfo? Technician { get; set; }

    public BookingScheduleReceiptInfo Schedule { get; set; } = new();

    public BookingLocationReceiptInfo Location { get; set; } = new();

    public BookingPriceReceiptInfo Price { get; set; } = new();

    public PaymentReceiptInfo? Payment { get; set; }
}