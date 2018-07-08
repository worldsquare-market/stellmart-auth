using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stellmart.Auth.Data.Enumerations
{
    public enum TwoFactorTypes
    {
        None = 0,
        Email = 1,
        Sms = 2,
        Totp = 3
    }
}
