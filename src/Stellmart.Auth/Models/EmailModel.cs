using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stellmart.Auth.Models
{
    public class EmailModel
    {
        public string ToAddress { get; set; }

        public string FromAddress { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string FromDisplayName { get; set; }

        public string ToDisplayName { get; set; }
    }
}
