using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OtpSharp;
using Wiry.Base32;

namespace Stellmart.Auth.Services
{
    public class TotpService : ITotpService
    {
        public bool Validate(string token, string code)
        {
            long timeStepMatched = 0;
            byte[] decodedKey = Base32Encoding.Standard.ToBytes(token);
            var otp = new Totp(decodedKey);
            bool valid = otp.VerifyTotp(
                code, out timeStepMatched, new VerificationWindow(2, 2));
            return valid;
        }
    }
}
