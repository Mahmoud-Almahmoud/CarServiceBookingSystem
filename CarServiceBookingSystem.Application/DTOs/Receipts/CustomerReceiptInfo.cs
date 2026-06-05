namespace CarServiceBookingSystem.Application.DTOs.Receipts;

public class CustomerReceiptInfo
{
    public string UserId { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Email { get; set; }
}