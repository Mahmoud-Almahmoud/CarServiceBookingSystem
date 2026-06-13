using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.NotificationBroadcasts;
using CarServiceBookingSystem.Application.DTOs.Notifications;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarServiceBookingSystem.Infrastructure.Services.Notifications;

public class NotificationBroadcastService : INotificationBroadcastService
{
    private const int BatchSize = 250;

    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IValidator<BroadcastNotificationRequest> _validator;
    private readonly ILogger<NotificationBroadcastService> _logger;

    public NotificationBroadcastService(
        ApplicationDbContext context,
        INotificationService notificationService,
        IValidator<BroadcastNotificationRequest> validator,
        ILogger<NotificationBroadcastService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApiResponse<BroadcastNotificationResponse>> SendAsync(
        BroadcastNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<BroadcastNotificationResponse>.Fail(errors);
        }

        var targetUserIds = await ResolveTargetUserIdsAsync(request, cancellationToken);

        if (targetUserIds.Count == 0)
        {
            return ApiResponse<BroadcastNotificationResponse>.Ok(
                new BroadcastNotificationResponse
                {
                    TargetUserCount = 0,
                    CreatedNotificationCount = 0,
                    BatchCount = 0
                });
        }

        var createdCount = 0;
        var batchCount = 0;

        foreach (var batch in targetUserIds.Chunk(BatchSize))
        {
            batchCount++;

            var notificationRequests = batch
                .Select(userId => new CreateNotificationRequest
                {
                    UserId = userId,
                    Title = request.Title,
                    Message = request.Message,
                    Type = request.Type,
                    Severity = request.Severity,
                    EntityType = request.EntityType,
                    EntityId = request.EntityId,
                    ActionUrl = request.ActionUrl
                })
                .ToList();

            var result = await _notificationService.CreateManyAsync(
                notificationRequests,
                cancellationToken);

            if (!result.Success)
            {
                _logger.LogWarning(
                    "Notification broadcast batch {BatchCount} failed. Message: {Message}",
                    batchCount,
                    result.Message);

                continue;
            }

            createdCount += result.Data.Count(x => x.Id > 0);
        }

        return ApiResponse<BroadcastNotificationResponse>.Ok(
            new BroadcastNotificationResponse
            {
                TargetUserCount = targetUserIds.Count,
                CreatedNotificationCount = createdCount,
                BatchCount = batchCount
            });
    }

    private async Task<List<string>> ResolveTargetUserIdsAsync(
        BroadcastNotificationRequest request,
        CancellationToken cancellationToken)
    {
        return request.AudienceType switch
        {
            NotificationAudienceType.AllUsers =>
                await GetAllUserIdsAsync(cancellationToken),

            NotificationAudienceType.SpecificUsers =>
                await GetSpecificUserIdsAsync(request.UserIds, cancellationToken),

            NotificationAudienceType.UsersByFilter =>
                await GetFilteredUserIdsAsync(request.Filter!, cancellationToken),

            _ => new List<string>()
        };
    }

    private async Task<List<string>> GetAllUserIdsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Select(x => x.Id)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<string>> GetSpecificUserIdsAsync(
        List<string> userIds,
        CancellationToken cancellationToken)
    {
        var normalizedIds = userIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList();

        return await _context.Users
            .AsNoTracking()
            .Where(x => normalizedIds.Contains(x.Id))
            .Select(x => x.Id)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<string>> GetFilteredUserIdsAsync(
        BroadcastNotificationFilterRequest filter,
        CancellationToken cancellationToken)
    {
        var usersQuery = _context.Users
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.EmailConfirmed
            });

        // Optional if ApplicationUser has CreatedAt.
        // If your ApplicationUser does not have CreatedAt, remove this block.
        /*
        if (filter.RegisteredFrom.HasValue)
        {
            usersQuery = usersQuery.Where(x => x.CreatedAt >= filter.RegisteredFrom.Value);
        }

        if (filter.RegisteredTo.HasValue)
        {
            usersQuery = usersQuery.Where(x => x.CreatedAt <= filter.RegisteredTo.Value);
        }
        */

        IQueryable<string> query = usersQuery.Select(x => x.Id);

        if (filter.OnlyUsersWithBookings == true)
        {
            var bookingUserIds = _context.Bookings
                .AsNoTracking()
                .Select(x => x.UserId)
                .Distinct();

            query = query.Where(userId => bookingUserIds.Contains(userId));
        }

        if (filter.OnlyUsersWithoutBookings == true)
        {
            var bookingUserIds = _context.Bookings
                .AsNoTracking()
                .Select(x => x.UserId)
                .Distinct();

            query = query.Where(userId => !bookingUserIds.Contains(userId));
        }

        if (filter.ServiceId.HasValue ||
            filter.ServiceBranchId.HasValue ||
            filter.LastBookingFrom.HasValue ||
            filter.LastBookingTo.HasValue ||
            !string.IsNullOrWhiteSpace(filter.City))
        {
            var bookingQuery = _context.Bookings
                .AsNoTracking()
                .AsQueryable();

            if (filter.ServiceId.HasValue)
            {
                bookingQuery = bookingQuery.Where(x => x.ServiceId == filter.ServiceId.Value);
            }

            if (filter.ServiceBranchId.HasValue)
            {
                bookingQuery = bookingQuery.Where(x => x.ServiceBranchId == filter.ServiceBranchId.Value);
            }

            if (filter.LastBookingFrom.HasValue)
            {
                bookingQuery = bookingQuery.Where(x => x.StartDate >= filter.LastBookingFrom.Value);
            }

            if (filter.LastBookingTo.HasValue)
            {
                bookingQuery = bookingQuery.Where(x => x.StartDate <= filter.LastBookingTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.City))
            {
                var city = filter.City.Trim();

                bookingQuery = bookingQuery.Where(x =>
                    x.CustomerCity != null &&
                    x.CustomerCity == city);
            }

            var matchingBookingUserIds = bookingQuery
                .Select(x => x.UserId)
                .Distinct();

            query = query.Where(userId => matchingBookingUserIds.Contains(userId));
        }

        if (filter.CarBrandId.HasValue || filter.CarModelId.HasValue)
        {
            var carQuery = _context.Cars
                .AsNoTracking()
                .Include(x => x.CarTrim)
                    .ThenInclude(x => x.Year)
                    .ThenInclude(x => x.Model)
                .AsQueryable();

            if (filter.CarBrandId.HasValue)
            {
                carQuery = carQuery.Where(x =>
                    x.CarTrim.Year.Model.BrandId == filter.CarBrandId.Value);
            }

            if (filter.CarModelId.HasValue)
            {
                carQuery = carQuery.Where(x =>
                    x.CarTrim.Year.ModelId == filter.CarModelId.Value);
            }

            var matchingCarUserIds = carQuery
                .Select(x => x.UserId)
                .Distinct();

            query = query.Where(userId => matchingCarUserIds.Contains(userId));
        }

        if (filter.MinTotalSpent.HasValue)
        {
            var paidUserIds = _context.Payments
                .AsNoTracking()
                .Where(x => x.Status == PaymentStatus.Succeeded)
                .GroupBy(x => x.UserId)
                .Where(g => g.Sum(x => x.Amount) >= filter.MinTotalSpent.Value)
                .Select(g => g.Key);

            query = query.Where(userId => paidUserIds.Contains(userId));
        }

        return await query
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}