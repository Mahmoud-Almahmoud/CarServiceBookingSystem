using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        using var message = new MailMessage();

        message.From = new MailAddress(_settings.FromEmail, _settings.FromName);
        message.To.Add(to);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = false;

        using var client = new SmtpClient(_settings.Host, _settings.Port);

        client.Credentials = new NetworkCredential(
            _settings.FromEmail,
            _settings.Password);

        client.EnableSsl = _settings.EnableSsl;

        await client.SendMailAsync(message);
    }
}