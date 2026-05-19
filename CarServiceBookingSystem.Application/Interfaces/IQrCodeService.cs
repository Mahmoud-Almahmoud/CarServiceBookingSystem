namespace CarServiceBookingSystem.Application.Interfaces;

public interface IQrCodeService
{
    byte[] GenerateQrCodePng(string text);
}