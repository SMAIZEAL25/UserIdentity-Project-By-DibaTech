using Application.Interface;
using OtpNet;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TwoFactorsService : ITwoFactorService
    {
        public (string seceret, string qrCodeUrl, string manualKey) GenerateSecret(string email, string appName = "UserRegisterAPI")
        {
            var secretBytes = KeyGeneration.GenerateRandomKey(20);
            var secret = Base32Encoding.ToString(secretBytes);
            var issuer = Uri.EscapeDataString(appName);
            var account = Uri.EscapeDataString(email);

            var otpAuth = $"otpAuth://totp/{issuer}:{account}?secret={secret}&issuer={issuer}&digits = 6";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodedata = qrGenerator.CreateQrCode(otpAuth, QRCodeGenerator.ECCLevel.Q);
            using var qrcode = new PngByteQRCode(qrCodedata);

            var qrcodeBytes = qrcode.GetGraphic(20);
            var qrcodeImageurl = $"data:Image/png;based64,{Convert.ToBase64String(qrcodeBytes)}";

            return (secret, qrcodeImageurl, secret);
        }

        public bool VerifyCode(string secret, string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2)); 
        }
    }
}
