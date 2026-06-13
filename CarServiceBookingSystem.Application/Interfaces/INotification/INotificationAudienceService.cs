namespace CarServiceBookingSystem.Application.Interfaces.INotification
{
    public interface INotificationAudienceService
    {
        Task<List<string>> GetUserIdsWithPermissionAsync(
            string permission,
            CancellationToken cancellationToken = default);

        Task<List<string>> GetAdminUserIdsAsync(
            CancellationToken cancellationToken = default);
    }
}
