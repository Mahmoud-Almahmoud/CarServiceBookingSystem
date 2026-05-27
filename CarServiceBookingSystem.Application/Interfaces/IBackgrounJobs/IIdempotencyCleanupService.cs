public interface IIdempotencyCleanupService
{
    Task DeleteExpiredAsync();
}