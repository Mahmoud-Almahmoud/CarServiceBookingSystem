namespace CarServiceBookingSystem.Application.Interfaces;

public interface IBackgroundJobService
{
    void EnqueueEmail(string to, string subject, string body);
}