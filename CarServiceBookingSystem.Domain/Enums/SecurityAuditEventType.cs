namespace CarServiceBookingSystem.Domain.Enums;

public enum SecurityAuditEventType
{
    LoginSucceeded,
    LoginFailed,
    SuspiciousLogin,
    AccessDenied,

    Registration,

    Logout,
    LogoutAllSessions,

    PasswordChanged,
    PasswordResetRequested,
    PasswordReset,
    EmailConfirmed,
    EmailConfirmationFailed,
    EmailConfirmationResent,

    RefreshTokenUsed,
    RefreshTokenReused,
    RefreshTokenRevoked,
    RefreshTokenFamilyRevoked,

    TwoFactorEnabled,
    TwoFactorDisabled,
    TwoFactorLoginSucceeded,
    TwoFactorLoginFailed,
    TwoFactorRecoveryCodesGenerated,

    TrustedDeviceAdded,
    TrustedDeviceLogin,
    TrustedDeviceLoginFailed,
    TrustedDeviceRevoked,
    TrustedDevicesRevokedAll,

    SessionRevoked,
    SessionsRevokedAll,

    ApiKeyCreated,
    ApiKeyRevoked,
    ApiKeyUsed,
    ApiKeyExpired,
    ApiKeyValidationFailed,

    RoleCreated,
    RoleUpdated,
    RoleDeleted,
    RolePermissionAdded,
    RolePermissionRemoved,

    UserRoleAdded,
    UserRoleRemoved,
    UserLocked,
    UserUnlocked,

    PaymentSucceeded,
    PaymentFailed,

    StripeWebhookReceived,
    StripeWebhookProcessed,
    StripeWebhookFailed,
    StripeWebhookDuplicate,

    IdempotencyKeyConflict,
    IdempotencyKeyReplayed,

    MaintenanceCleanupStarted,
    MaintenanceCleanupCompleted,
    MaintenanceCleanupFailed
}