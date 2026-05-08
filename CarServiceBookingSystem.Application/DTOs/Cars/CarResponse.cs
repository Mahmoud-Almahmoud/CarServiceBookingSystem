namespace CarServiceBookingSystem.Application.DTOs.Cars;

public class CarResponse
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;

    public int CarTrimId { get; set; }
    public string Trim { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
}