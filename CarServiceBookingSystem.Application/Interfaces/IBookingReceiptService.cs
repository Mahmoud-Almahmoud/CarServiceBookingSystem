using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Receipts;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IBookingReceiptService
{
    Task<ApiResponse<BookingReceiptResponse>> GetMyReceiptAsync(
        int bookingId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BookingReceiptResponse>> GetAdminReceiptAsync(
        int bookingId,
        CancellationToken cancellationToken = default);
}