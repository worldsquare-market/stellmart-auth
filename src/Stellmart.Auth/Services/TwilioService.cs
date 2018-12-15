using Microsoft.Extensions.Options;
using Stellmart.Auth.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using System.Diagnostics;

namespace Stellmart.Auth.Services
{
    public class TwilioService : ITwilioService
    {
        private readonly IOptions<TwilioSettings> _TwilioSettings;

        public TwilioService(IOptions<TwilioSettings> twilioCeredentials)
        {
            _TwilioSettings = twilioCeredentials;
            TwilioClient.Init(_TwilioSettings.Value.Sid, _TwilioSettings.Value.AuthToken);
        }

        public async Task<bool> SendSms(string toPhoneNumber, string message, string fromPhoneNumber = null)
        {
            var success = false;
            try
            {
                var text = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(fromPhoneNumber ?? _TwilioSettings.Value.PhoneNumber),
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
                Debug.WriteLine(e.Message);
            }

            return success;
        }
    }
}
