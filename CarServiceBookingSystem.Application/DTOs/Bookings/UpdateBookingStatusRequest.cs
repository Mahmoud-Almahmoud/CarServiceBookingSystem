using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class UpdateBookingStatusRequest
{
    public BookingStatus Status { get; set; }
}