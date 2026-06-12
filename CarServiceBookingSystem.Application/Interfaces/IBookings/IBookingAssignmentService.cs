using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;

namespace CarServiceBookingSystem.Application.Interfaces.IBookings;

public interface IBookingAssignmentService
{
    Task<ApiResponse<BookingTechnicianAssignmentResponse>> AssignTechnicianAsync(
        int bookingId,
        AssignTechnicianRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BookingTechnicianAssignmentResponse>> AutoAssignTechnicianAsync(
        int bookingId,
        CancellationToken cancellationToken = default);
}