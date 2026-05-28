public interface IIdempotencyCleanupService
{
    Task CleanupExpiredKeysAsync();
}