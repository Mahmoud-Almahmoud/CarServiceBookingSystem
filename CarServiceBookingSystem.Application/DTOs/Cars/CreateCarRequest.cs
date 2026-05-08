namespace CarServiceBookingSystem.Application.DTOs.Cars;

public class CreateCarRequest
{
    public int CarTrimId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
}