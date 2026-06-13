namespace CarServiceBookingSystem.Domain.Enums;

public enum NotificationType
{
    BookingCreated = 1,
    BookingConfirmed = 2,
    BookingAssigned = 3,
    BookingRescheduled = 4,
    BookingCancelled = 5,
    BookingCompleted = 6,

    PaymentSucceeded = 20,
    PaymentFailed = 21,
    RefundSucceeded = 22,
    RefundFailed = 23,

    ReviewReminder = 40,

    SecurityAlert = 60,

    System = 100
}