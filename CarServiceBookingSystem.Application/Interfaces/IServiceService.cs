using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Services;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IServiceService
{
    Task<ApiResponse<PagedResponse<ServiceResponse>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<ServiceResponse>> CreateAsync(CreateServiceRequest request);
    Task<ApiResponse<ServiceResponse>> UpdateAsync(int id, UpdateServiceRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
}