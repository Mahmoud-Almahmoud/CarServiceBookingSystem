using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class CreateBookingRequest
{
    public int CarId { get; set; }
    public int ServiceId { get; set; }
    public ServiceLocationType LocationType { get; set; }
    public DateTime StartDate { get; set; }
}