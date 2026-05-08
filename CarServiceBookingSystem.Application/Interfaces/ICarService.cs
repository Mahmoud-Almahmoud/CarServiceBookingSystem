using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Cars;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ICarService
{
    Task<ApiResponse<CarResponse>> AddCarAsync(CreateCarRequest request);
    Task<ApiResponse<List<CarResponse>>> GetMyCarsAsync();
    Task<ApiResponse<string>> DeleteCarAsync(int carId);
    Task<ApiResponse<CarResponse>> UpdateCarAsync(int carId, UpdateCarRequest request);
}