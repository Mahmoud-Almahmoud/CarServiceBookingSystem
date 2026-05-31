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
    private readonly IReverseGeocodingService _reverseGeocodingService;
    private readonly IServiceBranchService _serviceBranchService;

    public BookingQuoteService(
        IServicePricingService servicePricingService,
        IBookingAvailabilityService bookingAvailabilityService,
        ITravelEstimateService travelEstimateService,
        IOptions<BookingQuoteOptions> options,
        IServiceAreaService serviceAreaService,
        ICurrentUserService currentUserService,
        IReverseGeocodingService reverseGeocodingService,
        IServiceBranchService serviceBranchService)
    {
        _servicePricingService = servicePricingService;
        _bookingAvailabilityService = bookingAvailabilityService;
        _travelEstimateService = travelEstimateService;
        _options = options.Value;
        _serviceAreaService = serviceAreaService;
        _currentUserService = currentUserService;
        _reverseGeocodingService = reverseGeocodingService;
        _serviceBranchService = serviceBranchService;
    }

    public async Task<ApiResponse<BookingQuoteResponse>> GetQuoteAsync(
    BookingQuoteRequest request,
    CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingQuoteResponse>.Fail("User is not authenticated");

        if (request.StartDate <= DateTime.UtcNow)
        {
            return new ApiResponse<BookingQuoteResponse>
            {
                Success = false,
                Message = "StartDate must be in the future."
            };
        }

        if (request.LocationType == ServiceLocationType.OnUserSite &&
            (!request.CustomerLatitude.HasValue || !request.CustomerLongitude.HasValue))
        {
            return new ApiResponse<BookingQuoteResponse>
            {
                Success = false,
                Message = "Customer latitude and longitude are required for customer-site bookings."
            };
        }

        int? serviceBranchId = null;
        string? serviceBranchName = null;
        double? branchStraightLineDistanceKm = null;

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

        string? customerCountryCode = null;
        string? customerCity = null;
        string? customerAddress = null;

        int? matchedServiceAreaRuleId = null;
        string? matchedServiceAreaRuleScope = null;

        decimal travelFee = 0;
        double? distanceKm = null;
        int? estimatedTravelTimeMinutes = null;

        if (request.LocationType == ServiceLocationType.OnUserSite)
        {
            var geocode = await _reverseGeocodingService.ReverseGeocodeAsync(
                request.CustomerLatitude!.Value,
                request.CustomerLongitude!.Value,
                cancellationToken);

            customerCountryCode =
                !string.IsNullOrWhiteSpace(geocode?.CountryCode)
                    ? NormalizeCountryCode(geocode.CountryCode)
                    : NormalizeCountryCode(request.CustomerCountryCode);

            customerCity =
                !string.IsNullOrWhiteSpace(geocode?.City)
                    ? NormalizeCityForDisplay(geocode.City)
                    : NormalizeCityForDisplay(request.CustomerCity);

            customerAddress = geocode?.FormattedAddress;

            if (string.IsNullOrWhiteSpace(customerCountryCode))
            {
                return new ApiResponse<BookingQuoteResponse>
                {
                    Success = false,
                    Message = "Could not determine customer country from location."
                };
            }

            if (string.IsNullOrWhiteSpace(customerCity))
            {
                return new ApiResponse<BookingQuoteResponse>
                {
                    Success = false,
                    Message = "Could not determine customer city from location."
                };
            }

            var areaResponse = await _serviceAreaService.CheckAvailabilityAsync(
                new ServiceAreaCheckRequest
                {
                    ServiceId = request.ServiceId,
                    CountryCode = customerCountryCode,
                    City = customerCity
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
                        CustomerCountryCode = customerCountryCode,
                        CustomerCity = customerCity,
                        CustomerFormattedAddress = customerAddress,

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

            var branchResponse = await _serviceBranchService.SelectNearestBranchAsync(
                    request.ServiceId,
                    request.CustomerLatitude!.Value,
                    request.CustomerLongitude!.Value,
                    cancellationToken);

            if (!branchResponse.Success || branchResponse.Data is null)
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
                        UnavailableReason = branchResponse.Message,

                        CustomerLatitude = request.CustomerLatitude,
                        CustomerLongitude = request.CustomerLongitude,
                        CustomerCountryCode = customerCountryCode,
                        CustomerCity = customerCity,
                        CustomerFormattedAddress = customerAddress,

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

            var selectedBranch = branchResponse.Data;

            serviceBranchId = selectedBranch.ServiceBranchId;
            serviceBranchName = selectedBranch.ServiceBranchName;
            branchStraightLineDistanceKm = selectedBranch.StraightLineDistanceKm;

            var travelEstimate = await _travelEstimateService.EstimateAsync(
                selectedBranch.Latitude,
                selectedBranch.Longitude,
                request.CustomerLatitude.Value,
                request.CustomerLongitude.Value,
                cancellationToken);

            distanceKm = travelEstimate.DistanceKm;
            estimatedTravelTimeMinutes = travelEstimate.EstimatedTravelTimeMinutes;
            travelFee = CalculateTravelFee(travelEstimate.DistanceKm);
        }

        var isSlotAvailable = await _bookingAvailabilityService.IsSlotAvailableAsync(
            request.StartDate,
            endDate,
            request.LocationType,
            excludedBookingId: null,
            cancellationToken);

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

            ServiceBranchId = serviceBranchId,
            ServiceBranchName = serviceBranchName,
            BranchStraightLineDistanceKm = branchStraightLineDistanceKm,

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
                ? customerCountryCode
                : null,

            CustomerCity = request.LocationType == ServiceLocationType.OnUserSite
                ? customerCity
                : null,

            CustomerFormattedAddress = request.LocationType == ServiceLocationType.OnUserSite
                ? customerAddress
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