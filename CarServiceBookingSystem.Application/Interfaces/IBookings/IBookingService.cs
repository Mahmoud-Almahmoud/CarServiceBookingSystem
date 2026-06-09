using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;

namespace CarServiceBookingSystem.Application.Interfaces.IBookings;

public interface IBookingService
{
    Task<ApiResponse<BookingResponse>> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<BookingResponse>>> GetMyBookingsAsync();
    Task<ApiResponse<PagedResponse<BookingResponse>>> GetAllAsync(BookingQueryRequest request);
    Task<ApiResponse<BookingResponse>> UpdateStatusAsync(int bookingId, UpdateBookingStatusRequest request);
    Task<ApiResponse<BookingCancellationResponse>> CancelMyBookingAsync(
    int bookingId,
    CancelBookingRequest request,
    CancellationToken cancellationToken = default);

    Task<ApiResponse<RescheduleBookingResponse>> RescheduleMyBookingAsync(
    int bookingId,
    RescheduleBookingRequest request,
    CancellationToken cancellationToken = default);
}