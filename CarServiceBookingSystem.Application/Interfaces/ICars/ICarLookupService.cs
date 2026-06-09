using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Cars;

namespace CarServiceBookingSystem.Application.Interfaces.ICars;

public interface ICarLookupService
{
    Task<ApiResponse<List<CarBrandDto>>> GetBrandsAsync();
    Task<ApiResponse<List<CarModelDto>>> GetModelsByBrandAsync(int brandId);
    Task<ApiResponse<List<CarYearDto>>> GetYearsByModelAsync(int modelId);
    Task<ApiResponse<List<CarTrimDto>>> GetTrimsByYearAsync(int yearId);
}