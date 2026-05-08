using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class BookingQueryRequest : PagedRequest
{
    public BookingStatus? Status { get; set; }
}