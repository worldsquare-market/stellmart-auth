using Stellmart.Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stellmart.Auth.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailModel email);
    }
}
