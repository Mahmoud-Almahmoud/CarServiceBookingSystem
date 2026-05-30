using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IBookingQuoteService
{
    Task<ApiResponse<BookingQuoteResponse>> GetQuoteAsync(
        BookingQuoteRequest request,
        string userId,
        CancellationToken cancellationToken = default);
}