namespace CarServiceBookingSystem.Application.Options;

public class WebPushOptions
{
    public string Subject { get; set; } = string.Empty;

    public string PublicKey { get; set; } = string.Empty;

    public string PrivateKey { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;
}