namespace CarServiceBookingSystem.Application.DTOs.Cars;

public class UpdateCarRequest
{
    public int CarTrimId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
}