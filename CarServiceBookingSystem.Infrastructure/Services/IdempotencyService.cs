using CarServiceBookingSystem.Application.Common.Interfaces;
using CarServiceBookingSystem.Application.Common.Models;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class IdempotencyService : IIdempotencyService
{
    private readonly ApplicationDbContext _dbContext;

    public IdempotencyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IdempotencyCheckResult> CheckAsync(
        string key,
        string userId,
        string endpoint,
        string requestHash,
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.IdempotencyKeys
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Key == key,
                cancellationToken);

        if (existing is not null)
        {
            if (existing.RequestHash != requestHash)
            {
                return new IdempotencyCheckResult(
                    IsReplay: false,
                    IsConflict: true,
                    RecordId: existing.Id,
                    StatusCode: null,
                    ResponseBody: null);
            }

            if (existing.IsCompleted)
            {
                return new IdempotencyCheckResult(
                    IsReplay: true,
                    IsConflict: false,
                    RecordId: existing.Id,
                    StatusCode: existing.StatusCode,
                    ResponseBody: existing.ResponseBody);
            }

            return new IdempotencyCheckResult(
                IsReplay: false,
                IsConflict: true,
                RecordId: existing.Id,
                StatusCode: null,
                ResponseBody: null);
        }

        var record = new IdempotencyKey
        {
            Key = key,
            UserId = userId,
            Endpoint = endpoint,
            RequestHash = requestHash,
            ExpiresAtUtc = DateTime.UtcNow.AddHours(24)
        };

        _dbContext.IdempotencyKeys.Add(record);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            var duplicate = await _dbContext.IdempotencyKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Key == key,
                    cancellationToken);

            if (duplicate is null)
                throw;

            if (duplicate.RequestHash != requestHash)
            {
                return new IdempotencyCheckResult(
                    false,
                    true,
                    duplicate.Id,
                    null,
                    null);
            }

            if (duplicate.IsCompleted)
            {
                return new IdempotencyCheckResult(
                    true,
                    false,
                    duplicate.Id,
                    duplicate.StatusCode,
                    duplicate.ResponseBody);
            }

            return new IdempotencyCheckResult(
                false,
                true,
                duplicate.Id,
                null,
                null);
        }

        return new IdempotencyCheckResult(
            IsReplay: false,
            IsConflict: false,
            RecordId: record.Id,
            StatusCode: null,
            ResponseBody: null);
    }

    public async Task CompleteAsync(
        Guid id,
        int statusCode,
        string responseBody,
        CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.IdempotencyKeys
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (record is null)
            return;

        record.StatusCode = statusCode;
        record.ResponseBody = responseBody;
        record.IsCompleted = true;
        record.CompletedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}