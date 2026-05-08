namespace CarServiceBookingSystem.Application.DTOs.Services;

public class UpdateServiceRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationInMinutes { get; set; }
}