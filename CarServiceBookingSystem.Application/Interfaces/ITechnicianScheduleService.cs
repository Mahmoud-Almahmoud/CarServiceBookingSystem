using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Schedules;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ITechnicianScheduleService
{
    Task<ApiResponse<TechnicianScheduleResponse>> GetTechnicianScheduleAsync(
        int technicianId,
        TechnicianScheduleQueryRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchScheduleResponse>> GetBranchScheduleAsync(
        int serviceBranchId,
        BranchScheduleQueryRequest request,
        CancellationToken cancellationToken = default);
}