using Microsoft.AspNetCore.Identity;
using System;

namespace Stellmart.Auth.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public bool UseTwoFactorForLogin { get; set; }

        public string TotpSecret { get; set; }

        public string TwoFactorCode { get; set; }

        public int TwoFactorId { get; set; }

        public int TwoFactorFailedCount { get; set; }

        public int MaxTwoFactorFailedAccessAttempts { get; set; }

        public int DefaultTwoFatorLockoutMinutes { get; set; }

        public DateTime TwoFactorAuthTime { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName ?? "" + " "  + LastName ?? "";
            }
        }
    }
}
