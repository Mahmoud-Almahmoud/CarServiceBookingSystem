using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IBookingAvailabilityService
{
    Task<ApiResponse<List<AvailableSlotResponse>>> GetAvailableSlotsAsync(
        AvailableSlotsRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> IsSlotAvailableAsync(
        DateTime startDate,
        DateTime endDate,
        ServiceLocationType locationType,
        int? excludedBookingId = null,
        CancellationToken cancellationToken = default);
}