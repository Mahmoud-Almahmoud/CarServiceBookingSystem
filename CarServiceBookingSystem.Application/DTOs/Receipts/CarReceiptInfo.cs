namespace CarServiceBookingSystem.Application.DTOs.Receipts;

public class CarReceiptInfo
{
    public int CarId { get; set; }

    public string PlateNumber { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}