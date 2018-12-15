using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stellmart.Auth.ViewModels
{
    public class TwoFactorAuthenticationViewModel
    {
        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public string Nonce { get; set; }

        public string State { get; set; }

        public string ClientId { get; set; }

        public int TwoFactorType { get; set; }

        public string Username { get; set; }

        public string Code { get; set; }

        public string DisplayText { get; set; }

        public bool AllowResend { get; set;  }
    }
}
