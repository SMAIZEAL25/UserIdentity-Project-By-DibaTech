using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface ITwoFactorService
    {
        (string seceret, string qrCodeUrl, string manualKey) GenerateSecret(string email, string appName = "MyApp");
        bool VerifyCode(string secret, string code);
    }
}
