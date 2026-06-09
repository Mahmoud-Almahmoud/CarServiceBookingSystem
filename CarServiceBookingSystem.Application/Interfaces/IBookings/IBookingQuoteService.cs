using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;

namespace CarServiceBookingSystem.Application.Interfaces.IBookings;

public interface IBookingQuoteService
{
    Task<ApiResponse<BookingQuoteResponse>> GetQuoteAsync(BookingQuoteRequest request,CancellationToken cancellationToken = default);
}