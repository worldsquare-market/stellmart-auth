using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stellmart.Auth.Services
{
    public interface ITwilioService
    {
        Task<bool> SendSms(string toPhoneNumber, string message, string fromPhoneNumber = null);
    }
}
