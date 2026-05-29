namespace CarServiceBookingSystem.Domain.Enums;

public enum SecurityAuditEventType
{
    LoginSucceeded,
    LoginFailed,
    SuspiciousLogin,
    AccessDenied,

    Logout,
    LogoutAllSessions,

    PasswordChanged,
    PasswordResetRequested,
    PasswordReset,
    EmailConfirmed,
    EmailConfirmationResent,

    RefreshTokenUsed,
    RefreshTokenReused,
    RefreshTokenRevoked,
    RefreshTokenFamilyRevoked,

    TwoFactorEnabled,
    TwoFactorDisabled,
    TwoFactorLogin,
    TwoFactorLoginFailed,
    TwoFactorRecoveryCodesGenerated,

    TrustedDeviceAdded,
    TrustedDeviceLogin,
    TrustedDeviceLoginFailed,
    TrustedDeviceRevoked,
    TrustedDevicesRevoked,

    SessionRevoked,
    SessionsRevoked,

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