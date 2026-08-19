using System.Security.Cryptography;
using System.Text;
using QRCoder;
using VMS.Application.Interfaces;

namespace VMS.Infrastructure.QR;

public class QrCodeService : IQrCodeService
{
    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public string ComputeTokenHash(string token)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }

    public byte[] GenerateQrPng(string token)
    {
        using var qrGenerator = new QRCodeGenerator();

        using var qrData = qrGenerator.CreateQrCode(
            token,
            QRCodeGenerator.ECCLevel.Q);

        var pngQrCode = new PngByteQRCode(qrData);

        return pngQrCode.GetGraphic(20);
    }
}