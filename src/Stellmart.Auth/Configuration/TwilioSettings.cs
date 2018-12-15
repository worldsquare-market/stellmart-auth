using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stellmart.Auth.Configuration
{
    public class TwilioSettings
    {
        public string Sid { get; set; }

        public string AuthToken { get; set;  }

        public string PhoneNumber { get; set;  }
    }
}
