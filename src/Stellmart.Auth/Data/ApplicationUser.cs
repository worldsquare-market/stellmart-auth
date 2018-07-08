using Microsoft.AspNetCore.Identity;

namespace Stellmart.Auth.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public bool UseTwoFactorForLogin { get; set; }

        public string TotpSecret { get; set; }

        public string TwoFactorCode { get; set; }

        public int TwoFactorTypeId { get; set; }

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
