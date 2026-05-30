using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.DTOs.ServiceAreas;
using CarServiceBookingSystem.Application.DTOs.ServicePricing;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Domain.Enums;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class BookingQuoteService : IBookingQuoteService
{
    private readonly IServicePricingService _servicePricingService;
    private readonly IBookingAvailabilityService _bookingAvailabilityService;
    private readonly ITravelEstimateService _travelEstimateService;
    private readonly BookingQuoteOptions _options;
    private readonly IServiceAreaService _serviceAreaService;
    private readonly ICurrentUserService _currentUserService;

    public BookingQuoteService(
        IServicePricingService servicePricingService,
        IBookingAvailabilityService bookingAvailabilityService,
        ITravelEstimateService travelEstimateService,
        IOptions<BookingQuoteOptions> options,
        IServiceAreaService serviceAreaService,
        ICurrentUserService currentUserService)
    {
        _servicePricingService = servicePricingService;
        _bookingAvailabilityService = bookingAvailabilityService;
        _travelEstimateService = travelEstimateService;
        _options = options.Value;
        _serviceAreaService = serviceAreaService;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<BookingQuoteResponse>> GetQuoteAsync(
    BookingQuoteRequest request,
    CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingQuoteResponse>.Fail("User is not authenticated");

        if (string.IsNullOrWhiteSpace(userId))
        {
            return new ApiResponse<BookingQuoteResponse>
            {
                Success = false,
                Message = "User was not found."
            };
        }

        if (request.StartDate <= DateTime.UtcNow)
        {
            return new ApiResponse<BookingQuoteResponse>
            {
                Success = false,
                Message = "StartDate must be in the future."
            };
        }

        if (request.LocationType == ServiceLocationType.OnUserSite &&
            (!request.CustomerLatitude.HasValue ||
             !request.CustomerLongitude.HasValue ||
             string.IsNullOrWhiteSpace(request.CustomerCountryCode) ||
             string.IsNullOrWhiteSpace(request.CustomerCity)))
        {
            return new ApiResponse<BookingQuoteResponse>
            {
                Success = false,
                Message = "Customer latitude, longitude, country code, and city are required for customer-site bookings."
            };
        }

        var pricingResponse = await _servicePricingService.GetPriceQuoteAsync(
            new ServicePriceQuoteRequest
            {
                CarId = request.CarId,
                ServiceId = request.ServiceId
            },
            userId,
            cancellationToken);

        if (!pricingResponse.Success || pricingResponse.Data is null)
        {
            return new ApiResponse<BookingQuoteResponse>
            {
                Success = false,
                Message = pricingResponse.Message
            };
        }

        var pricing = pricingResponse.Data;

        var endDate = request.StartDate.AddMinutes(pricing.DurationMinutes);

        int? matchedServiceAreaRuleId = null;
        string? matchedServiceAreaRuleScope = null;

        if (request.LocationType == ServiceLocationType.OnUserSite)
        {
            var areaResponse = await _serviceAreaService.CheckAvailabilityAsync(
                new ServiceAreaCheckRequest
                {
                    ServiceId = request.ServiceId,
                    CountryCode = request.CustomerCountryCode!,
                    City = request.CustomerCity
                },
                cancellationToken);

            if (!areaResponse.Success || areaResponse.Data is null)
            {
                return new ApiResponse<BookingQuoteResponse>
                {
                    Success = false,
                    Message = areaResponse.Message
                };
            }

            matchedServiceAreaRuleId = areaResponse.Data.MatchedRuleId;
            matchedServiceAreaRuleScope = areaResponse.Data.MatchedRuleScope;

            if (!areaResponse.Data.IsAvailable)
            {
                return new ApiResponse<BookingQuoteResponse>
                {
                    Success = true,
                    Message = "Booking quote calculated successfully.",
                    Data = new BookingQuoteResponse
                    {
                        CarId = request.CarId,
                        ServiceId = request.ServiceId,
                        ServiceName = pricing.ServiceName,
                        LocationType = request.LocationType,
                        StartDate = request.StartDate,
                        EndDate = endDate,
                        DurationMinutes = pricing.DurationMinutes,

                        ServicePrice = pricing.Price,
                        TravelFee = 0,
                        TotalPrice = pricing.Price,

                        IsAvailable = false,
                        UnavailableReason = areaResponse.Data.Reason,

                        CustomerLatitude = request.CustomerLatitude,
                        CustomerLongitude = request.CustomerLongitude,
                        CustomerCountryCode = NormalizeCountryCode(request.CustomerCountryCode),
                        CustomerCity = NormalizeCityForDisplay(request.CustomerCity),

                        DistanceKm = null,
                        EstimatedTravelTimeMinutes = null,

                        UsedCustomPriceRule = pricing.UsedCustomPriceRule,
                        ServicePriceRuleId = pricing.ServicePriceRuleId,
                        PricingSource = pricing.PricingSource,

                        MatchedServiceAreaRuleId = matchedServiceAreaRuleId,
                        MatchedServiceAreaRuleScope = matchedServiceAreaRuleScope
                    }
                };
            }
        }

        var isSlotAvailable = await _bookingAvailabilityService.IsSlotAvailableAsync(
            request.StartDate,
            endDate,
            request.LocationType,
            excludedBookingId: null,
            cancellationToken);

        decimal travelFee = 0;
        double? distanceKm = null;
        int? estimatedTravelTimeMinutes = null;

        if (request.LocationType == ServiceLocationType.OnUserSite)
        {
            var travelEstimate = await _travelEstimateService.EstimateAsync(
                _options.DefaultBranchLatitude,
                _options.DefaultBranchLongitude,
                request.CustomerLatitude!.Value,
                request.CustomerLongitude!.Value,
                cancellationToken);

            distanceKm = travelEstimate.DistanceKm;
            estimatedTravelTimeMinutes = travelEstimate.EstimatedTravelTimeMinutes;

            travelFee = CalculateTravelFee(travelEstimate.DistanceKm);
        }

        var response = new BookingQuoteResponse
        {
            CarId = request.CarId,
            ServiceId = request.ServiceId,
            ServiceName = pricing.ServiceName,
            LocationType = request.LocationType,
            StartDate = request.StartDate,
            EndDate = endDate,
            DurationMinutes = pricing.DurationMinutes,

            ServicePrice = pricing.Price,
            TravelFee = travelFee,
            TotalPrice = pricing.Price + travelFee,

            IsAvailable = isSlotAvailable,
            UnavailableReason = isSlotAvailable
                ? null
                : "Selected time slot is not available.",

            CustomerLatitude = request.LocationType == ServiceLocationType.OnUserSite
                ? request.CustomerLatitude
                : null,

            CustomerLongitude = request.LocationType == ServiceLocationType.OnUserSite
                ? request.CustomerLongitude
                : null,

            CustomerCountryCode = request.LocationType == ServiceLocationType.OnUserSite
                ? NormalizeCountryCode(request.CustomerCountryCode)
                : null,

            CustomerCity = request.LocationType == ServiceLocationType.OnUserSite
                ? NormalizeCityForDisplay(request.CustomerCity)
                : null,

            DistanceKm = distanceKm,
            EstimatedTravelTimeMinutes = estimatedTravelTimeMinutes,

            UsedCustomPriceRule = pricing.UsedCustomPriceRule,
            ServicePriceRuleId = pricing.ServicePriceRuleId,
            PricingSource = pricing.PricingSource,

            MatchedServiceAreaRuleId = matchedServiceAreaRuleId,
            MatchedServiceAreaRuleScope = matchedServiceAreaRuleScope
        };

        return new ApiResponse<BookingQuoteResponse>
        {
            Success = true,
            Message = "Booking quote calculated successfully.",
            Data = response
        };
    }

    private static string? NormalizeCountryCode(string? countryCode)
    {
        return string.IsNullOrWhiteSpace(countryCode)
            ? null
            : countryCode.Trim().ToUpper();
    }

    private static string? NormalizeCityForDisplay(string? city)
    {
        return string.IsNullOrWhiteSpace(city)
            ? null
            : city.Trim();
    }

    private decimal CalculateTravelFee(double distanceKm)
    {
        var fee = _options.BaseTravelFee +
                  ((decimal)distanceKm * _options.TravelFeePerKm);

        if (_options.MaxTravelFee > 0 && fee > _options.MaxTravelFee)
        {
            fee = _options.MaxTravelFee;
        }

        if (fee < 0)
        {
            fee = 0;
        }

        return Math.Round(fee, 2);
    }
}