using Microsoft.Extensions.Options;
using Stellmart.Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;
using Stellmart.Auth.Configuration;
using System.Diagnostics;

namespace Stellmart.Auth.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SendGridSettings> _sendGridSettings;
        private readonly string _systemDisplayName = "The WorldSquare Team";
        private readonly string _systemEmail = "info@worldsquare.io";
        private readonly SendGridClient _client;

        public EmailService(IOptions<SendGridSettings> SendGridSettings)
        {
            _sendGridSettings = SendGridSettings;
            _client = new SendGridClient(_sendGridSettings.Value.ApiKey);
        }

        public async Task<bool> SendEmail(EmailModel email)
        {
           
            var success = false;
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(email.FromAddress ?? _systemEmail, email.FromDisplayName ?? _systemDisplayName),
                Subject = email.Subject,
                PlainTextContent = email.Body
            };
            msg.AddTo(email.ToAddress, email.ToDisplayName);
            try
            {
                var response = await _client.SendEmailAsync(msg);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return success;
        }
    }
}
