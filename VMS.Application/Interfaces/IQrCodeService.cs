namespace VMS.Application.Interfaces;

public interface IQrCodeService
{
    string GenerateToken();

    string ComputeTokenHash(string token);

    byte[] GenerateQrPng(string token);
}