using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Cars;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class CarService : ICarService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CarService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CarResponse>> AddCarAsync(CreateCarRequest request)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<CarResponse>.Fail("User is not authenticated");
        }

        var trimExists = await _context.CarTrims
            .AnyAsync(x => x.Id == request.CarTrimId);

        if (!trimExists)
        {
            return ApiResponse<CarResponse>.Fail("Invalid car trim");
        }

        var car = new Car
        {
            UserId = userId,
            CarTrimId = request.CarTrimId,
            PlateNumber = request.PlateNumber
        };

        await _context.Cars.AddAsync(car);
        await _context.SaveChangesAsync();

        var response = await BuildCarResponseAsync(car.Id);

        return ApiResponse<CarResponse>.Ok(response!, "Car added successfully");
    }

    public async Task<ApiResponse<List<CarResponse>>> GetMyCarsAsync()
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<List<CarResponse>>.Fail("User is not authenticated");
        }

        var cars = await _context.Cars
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.CarTrim)
                .ThenInclude(x => x.Year)
                .ThenInclude(x => x.Model)
                .ThenInclude(x => x.Brand)
            .Select(x => new CarResponse
            {
                Id = x.Id,
                PlateNumber = x.PlateNumber,
                CarTrimId = x.CarTrimId,
                Trim = x.CarTrim.Name,
                Year = x.CarTrim.Year.Year.ToString(),
                Model = x.CarTrim.Year.Model.Name,
                Brand = x.CarTrim.Year.Model.Brand.Name
            })
            .ToListAsync();

        return ApiResponse<List<CarResponse>>.Ok(cars);
    }

    public async Task<ApiResponse<string>> DeleteCarAsync(int carId)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<string>.Fail("User is not authenticated");
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(x => x.Id == carId && x.UserId == userId);

        if (car == null)
        {
            return ApiResponse<string>.Fail("Car not found");
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Deleted", "Car deleted successfully");
    }

    private async Task<CarResponse?> BuildCarResponseAsync(int carId)
    {
        return await _context.Cars
            .AsNoTracking()
            .Where(x => x.Id == carId)
            .Include(x => x.CarTrim)
                .ThenInclude(x => x.Year)
                .ThenInclude(x => x.Model)
                .ThenInclude(x => x.Brand)
            .Select(x => new CarResponse
            {
                Id = x.Id,
                PlateNumber = x.PlateNumber,
                CarTrimId = x.CarTrimId,
                Trim = x.CarTrim.Name,
                Year = x.CarTrim.Year.Year.ToString(),
                Model = x.CarTrim.Year.Model.Name,
                Brand = x.CarTrim.Year.Model.Brand.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ApiResponse<CarResponse>> UpdateCarAsync(
    int carId,
    UpdateCarRequest request)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<CarResponse>.Fail("User is not authenticated");
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(x => x.Id == carId && x.UserId == userId);

        if (car == null)
        {
            return ApiResponse<CarResponse>.Fail("Car not found");
        }

        var trimExists = await _context.CarTrims
            .AnyAsync(x => x.Id == request.CarTrimId);

        if (!trimExists)
        {
            return ApiResponse<CarResponse>.Fail("Invalid car trim");
        }

        car.CarTrimId = request.CarTrimId;
        car.PlateNumber = request.PlateNumber;

        await _context.SaveChangesAsync();

        var response = await BuildCarResponseAsync(car.Id);

        return ApiResponse<CarResponse>.Ok(response!, "Car updated successfully");
    }
}