using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IBookingAssignmentService
{
    Task<ApiResponse<BookingTechnicianAssignmentResponse>> AssignTechnicianAsync(
        int bookingId,
        AssignTechnicianRequest request,
        CancellationToken cancellationToken = default);
}