using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class BookingResponse
{
    public int Id { get; set; }

    public int CarId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }

    public ServiceLocationType LocationType { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public BookingStatus Status { get; set; }
}