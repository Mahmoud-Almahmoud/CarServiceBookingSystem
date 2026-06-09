using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.DTOs.Reviews;
using CarServiceBookingSystem.Application.Interfaces.IBookings;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Bookings;

public class BookingReviewService : IBookingReviewService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public BookingReviewService(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<BookingReviewResponse>> CreateMyReviewAsync(
        int bookingId,
        CreateBookingReviewRequest request,
        CancellationToken cancellationToken = default)
    {

        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingReviewResponse>.Fail("User is not authenticated");

        var booking = await _context.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == bookingId &&
                x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return ApiResponse<BookingReviewResponse>.Fail("Booking not found.");
        }

        if (booking.Status != BookingStatus.Completed)
        {
            return ApiResponse<BookingReviewResponse>.Fail("Only completed bookings can be reviewed.");
        }

        var alreadyReviewed = await _context.BookingReviews
            .AsNoTracking()
            .AnyAsync(x => x.BookingId == bookingId, cancellationToken);

        if (alreadyReviewed)
        {
            return ApiResponse<BookingReviewResponse>.Fail("This booking already has a review.");
        }

        var review = new BookingReview
        {
            BookingId = bookingId,
            UserId = userId,
            Rating = request.Rating,
            Comment = string.IsNullOrWhiteSpace(request.Comment)
                ? null
                : request.Comment.Trim(),
            IsVisible = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.BookingReviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(review.Id, cancellationToken);
    }

    public async Task<ApiResponse<BookingReviewResponse>> UpdateMyReviewAsync(
        int reviewId,
        UpdateBookingReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingReviewResponse>.Fail("User is not authenticated");

        var review = await _context.BookingReviews
            .FirstOrDefaultAsync(x =>
                x.Id == reviewId &&
                x.UserId == userId,
                cancellationToken);

        if (review is null)
        {
            return ApiResponse<BookingReviewResponse>.Fail("Review not found.");
        }

        review.Rating = request.Rating;
        review.Comment = string.IsNullOrWhiteSpace(request.Comment)
            ? null
            : request.Comment.Trim();
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(review.Id, cancellationToken);
    }

    public async Task<ApiResponse<bool>> DeleteMyReviewAsync(
        int reviewId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<bool>.Fail("User is not authenticated");

        var review = await _context.BookingReviews
            .FirstOrDefaultAsync(x =>
                x.Id == reviewId &&
                x.UserId == userId,
                cancellationToken);

        if (review is null)
        {
            return ApiResponse<bool>.Fail("Review not found.");
        }

        _context.BookingReviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<PagedResponse<BookingReviewResponse>>> GetMyReviewsAsync(
        BookingReviewQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<PagedResponse<BookingReviewResponse>>.Fail("User is not authenticated");

        request.UserId = userId;
        request.IsVisible = null;

        return await GetAllAsync(request, cancellationToken);
    }

    public async Task<ApiResponse<PagedResponse<BookingReviewResponse>>> GetAllAsync(
        BookingReviewQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = BuildReviewQuery();

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            query = query.Where(x => x.UserId == request.UserId);
        }

        if (request.BookingId.HasValue)
        {
            query = query.Where(x => x.BookingId == request.BookingId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            query = query.Where(x => x.Booking.ServiceId == request.ServiceId.Value);
        }

        if (request.Rating.HasValue)
        {
            query = query.Where(x => x.Rating == request.Rating.Value);
        }

        if (request.IsVisible.HasValue)
        {
            query = query.Where(x => x.IsVisible == request.IsVisible.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Comment != null &&
                x.Comment.Contains(search));
        }

        query = request.SortBy?.ToLower() switch
        {
            "rating" => request.Desc
                ? query.OrderByDescending(x => x.Rating).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Rating).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var customerLookup = await GetCustomerLookupAsync(reviews, cancellationToken);

        var items = reviews
            .Select(x => MapToResponse(x, customerLookup))
            .ToList();

        var response = new PagedResponse<BookingReviewResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<BookingReviewResponse>>.Ok(response);
    }

    public async Task<ApiResponse<BookingReviewResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var review = await BuildReviewQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (review is null)
        {
            return ApiResponse<BookingReviewResponse>.Fail("Review not found.");
        }

        var customerLookup = await GetCustomerLookupAsync(
            new[] { review },
            cancellationToken);

        return ApiResponse<BookingReviewResponse>.Ok(
            MapToResponse(review, customerLookup));
    }

    public async Task<ApiResponse<BookingReviewResponse>> UpdateVisibilityAsync(
        int id,
        AdminUpdateBookingReviewVisibilityRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await _context.BookingReviews
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (review is null)
        {
            return ApiResponse<BookingReviewResponse>.Fail("Review not found.");
        }

        review.IsVisible = request.IsVisible;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(review.Id, cancellationToken);
    }

    public async Task<ApiResponse<PublicServiceReviewSummaryResponse>> GetPublicServiceReviewsAsync(
    int serviceId,
    PublicServiceReviewQueryRequest request,
    CancellationToken cancellationToken = default)
    {
        var serviceExists = await _context.Services
            .AsNoTracking()
            .AnyAsync(x => x.Id == serviceId, cancellationToken);

        if (!serviceExists)
        {
            return ApiResponse<PublicServiceReviewSummaryResponse>.Fail("Service not found.");
        }

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.BookingReviews
            .AsNoTracking()
            .Include(x => x.Booking)
                .ThenInclude(x => x.Service)
            .Include(x => x.Booking)
                .ThenInclude(x => x.ServiceBranch)
            .Include(x => x.Booking)
                .ThenInclude(x => x.Technician)
            .Where(x =>
                x.IsVisible &&
                x.Booking.ServiceId == serviceId &&
                x.Booking.Status == BookingStatus.Completed);

        if (request.Rating.HasValue)
        {
            query = query.Where(x => x.Rating == request.Rating.Value);
        }

        query = request.SortBy?.ToLower() switch
        {
            "rating" => request.Desc
                ? query.OrderByDescending(x => x.Rating).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Rating).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var allVisibleRatings = await _context.BookingReviews
            .AsNoTracking()
            .Where(x =>
                x.IsVisible &&
                x.Booking.ServiceId == serviceId &&
                x.Booking.Status == BookingStatus.Completed)
            .Select(x => x.Rating)
            .ToListAsync(cancellationToken);

        var reviews = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var customerLookup = await GetCustomerLookupAsync(reviews, cancellationToken);

        var items = reviews
            .Select(x => MapToPublicServiceReviewResponse(x, customerLookup))
            .ToList();

        var serviceName = reviews.FirstOrDefault()?.Booking.Service.Name;

        if (string.IsNullOrWhiteSpace(serviceName))
        {
            serviceName = await _context.Services
                .AsNoTracking()
                .Where(x => x.Id == serviceId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        var pagedResponse = new PagedResponse<PublicServiceReviewResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        var response = new PublicServiceReviewSummaryResponse
        {
            ServiceId = serviceId,
            ServiceName = serviceName,
            TotalReviews = allVisibleRatings.Count,
            AverageRating = allVisibleRatings.Count == 0
                ? 0
                : Math.Round((decimal)allVisibleRatings.Average(), 2, MidpointRounding.AwayFromZero),

            FiveStarCount = allVisibleRatings.Count(x => x == 5),
            FourStarCount = allVisibleRatings.Count(x => x == 4),
            ThreeStarCount = allVisibleRatings.Count(x => x == 3),
            TwoStarCount = allVisibleRatings.Count(x => x == 2),
            OneStarCount = allVisibleRatings.Count(x => x == 1),

            Reviews = pagedResponse
        };

        return ApiResponse<PublicServiceReviewSummaryResponse>.Ok(response);
    }

    private static PublicServiceReviewResponse MapToPublicServiceReviewResponse(
    BookingReview review,
    IReadOnlyDictionary<string, CustomerLookupItem> customers)
    {
        customers.TryGetValue(review.UserId, out var customer);

        return new PublicServiceReviewResponse
        {
            Id = review.Id,
            BookingId = review.BookingId,

            Rating = review.Rating,
            Comment = review.Comment,

            CustomerName = MaskCustomerName(customer?.Name),

            ServiceId = review.Booking.ServiceId,
            ServiceName = review.Booking.Service.Name,

            ServiceBranchId = review.Booking.ServiceBranchId,
            ServiceBranchName = review.Booking.ServiceBranch?.Name,

            TechnicianId = review.Booking.TechnicianId,
            TechnicianName = review.Booking.Technician?.FullName,

            BookingStartDate = review.Booking.StartDate,

            CreatedAt = review.CreatedAt
        };
    }

    private static string? MaskCustomerName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var trimmed = name.Trim();

        if (trimmed.Length <= 2)
        {
            return $"{trimmed[0]}***";
        }

        return $"{trimmed[0]}***{trimmed[^1]}";
    }

    private IQueryable<BookingReview> BuildReviewQuery()
    {
        return _context.BookingReviews
            .AsNoTracking()
            .Include(x => x.Booking)
                .ThenInclude(x => x.Service)
            .Include(x => x.Booking)
                .ThenInclude(x => x.ServiceBranch)
            .Include(x => x.Booking)
                .ThenInclude(x => x.Technician);
    }

    private async Task<Dictionary<string, CustomerLookupItem>> GetCustomerLookupAsync(
        IEnumerable<BookingReview> reviews,
        CancellationToken cancellationToken)
    {
        var userIds = reviews
            .Select(x => x.UserId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (userIds.Count == 0)
        {
            return new Dictionary<string, CustomerLookupItem>();
        }

        return await _context.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new CustomerLookupItem
            {
                Id = x.Id,
                Name = x.UserName,
                Email = x.Email
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }

    private static BookingReviewResponse MapToResponse(
        BookingReview review,
        IReadOnlyDictionary<string, CustomerLookupItem> customers)
    {
        customers.TryGetValue(review.UserId, out var customer);

        return new BookingReviewResponse
        {
            Id = review.Id,
            BookingId = review.BookingId,
            UserId = review.UserId,

            CustomerName = customer?.Name,
            CustomerEmail = customer?.Email,

            Rating = review.Rating,
            Comment = review.Comment,
            IsVisible = review.IsVisible,

            ServiceId = review.Booking.ServiceId,
            ServiceName = review.Booking.Service.Name,

            ServiceBranchId = review.Booking.ServiceBranchId,
            ServiceBranchName = review.Booking.ServiceBranch?.Name,

            TechnicianId = review.Booking.TechnicianId,
            TechnicianName = review.Booking.Technician?.FullName,

            BookingStartDate = review.Booking.StartDate,

            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }

    private sealed class CustomerLookupItem
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}