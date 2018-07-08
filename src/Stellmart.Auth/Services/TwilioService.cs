using Microsoft.Extensions.Options;
using Stellmart.Auth.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace Stellmart.Auth.Services
{
    public class TwilioService : ITwilioService
    {
        private readonly IOptions<TwilioCredentials> _twilioCredentials;

        public TwilioService(IOptions<TwilioCredentials> twilioCeredentials)
        {
            _twilioCredentials = twilioCeredentials;
            TwilioClient.Init(_twilioCredentials.Value.Sid, _twilioCredentials.Value.AuthToken);
        }

        public async Task<bool> SendSms(string toPhoneNumber, string message, string fromPhoneNumber = null)
        {
            var success = false;
            try
            {
                var text = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(fromPhoneNumber ?? _twilioCredentials.Value.PhoneNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
                );
                if (text.Status != MessageResource.StatusEnum.Failed &&
                    text.Status != MessageResource.StatusEnum.Undelivered)
                {
                    success = true;
                }
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }
    }
}
