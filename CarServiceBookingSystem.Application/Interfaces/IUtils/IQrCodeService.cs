namespace CarServiceBookingSystem.Application.Interfaces.IUtils;

public interface IQrCodeService
{
    byte[] GenerateQrCodePng(string text);
}