using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Technicians;

namespace CarServiceBookingSystem.Application.Interfaces.ITechnicians;

public interface ITechnicianService
{
    Task<ApiResponse<PagedResponse<TechnicianResponse>>> GetAllAsync(
        TechnicianQueryRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TechnicianResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TechnicianResponse>> CreateAsync(
        CreateTechnicianRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TechnicianResponse>> UpdateAsync(
        int id,
        UpdateTechnicianRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<IEnumerable<TechnicianServiceResponse>>> GetServicesAsync(
        int technicianId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TechnicianServiceResponse>> AddServiceAsync(
        int technicianId,
        AddTechnicianServiceRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> RemoveServiceAsync(
        int technicianId,
        int serviceId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<IEnumerable<TechnicianWorkingHourResponse>>> GetWorkingHoursAsync(
    int technicianId,
    CancellationToken cancellationToken = default);

    Task<ApiResponse<IEnumerable<TechnicianWorkingHourResponse>>> UpdateWorkingHoursAsync(
        int technicianId,
        UpdateTechnicianWorkingHoursRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<IEnumerable<TechnicianUnavailableDateResponse>>> GetUnavailableDatesAsync(
        int technicianId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TechnicianUnavailableDateResponse>> GetUnavailableDateByIdAsync(
        int technicianId,
        int unavailableDateId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TechnicianUnavailableDateResponse>> CreateUnavailableDateAsync(
        int technicianId,
        CreateTechnicianUnavailableDateRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<TechnicianUnavailableDateResponse>> UpdateUnavailableDateAsync(
        int technicianId,
        int unavailableDateId,
        UpdateTechnicianUnavailableDateRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteUnavailableDateAsync(
        int technicianId,
        int unavailableDateId,
        CancellationToken cancellationToken = default);
}