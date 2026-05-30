namespace CarServiceBookingSystem.Application.DTOs.ServicePricing;

public class ServicePriceQuoteRequest
{
    public int ServiceId { get; set; }

    public int CarId { get; set; }
}