namespace CarServiceBookingSystem.Application.Interfaces.IEmail;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}