using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<BookingResponse>> CreateAsync(CreateBookingRequest request);
    Task<ApiResponse<List<BookingResponse>>> GetMyBookingsAsync();
    Task<ApiResponse<PagedResponse<BookingResponse>>> GetAllAsync(BookingQueryRequest request);
    Task<ApiResponse<BookingResponse>> UpdateStatusAsync(int bookingId, UpdateBookingStatusRequest request);
}