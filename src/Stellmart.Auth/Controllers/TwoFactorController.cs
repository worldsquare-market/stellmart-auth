using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stellmart.Auth.ViewModels;
using Microsoft.AspNetCore.Identity;
using Stellmart.Auth.Data;
using IdentityServer4.Services;
using Stellmart.Auth.Data.Enumerations;
using Stellmart.Auth.Services;
using Stellmart.Auth.Models;
using System.Security.Cryptography;
using IdentityServer4.Events;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stellmart.Auth.Controllers
{
    public class TwoFactorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventService _events;
        private readonly IEmailService _emailService;
        private readonly ITwilioService _twilioService;
        private readonly ITotpService _totpService;
        private readonly IIdentityServerInteractionService _interaction;

        private const string _emailDisplayText = "A code has been sent to your email. Please enter it below:";
        private const string _smsDisplayText = "A code has been sent to your phone. Please enter it below:";
        private const string _totpDisplayText = "Enter the code from your device below:";


        public TwoFactorController(
            UserManager<ApplicationUser> userManager,
            IEventService events,
            IEmailService emailService,
            ITwilioService twilioService,
            ITotpService totpService,
            IIdentityServerInteractionService interaction
            )
        {
            _userManager = userManager;
            _events = events;
            _emailService = emailService;
            _twilioService = twilioService;
            _totpService = totpService;
            _interaction = interaction;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            switch (user.TwoFactorTypeId)
            {
                case (int)TwoFactorTypes.Email:
                    await SendCodeToEmail(user);
                    break;
                case (int)TwoFactorTypes.Sms:
                    await SendSmsCode(user);
                    break;
            }
            var vm = new TwoFactorAuthenticationViewModel()
            {
                ReturnUrl = returnUrl,
                Username = username,
                DisplayText = GetDisplayText(user),
                AllowResend = user.TwoFactorTypeId != (int)TwoFactorTypes.Totp
        };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TwoFactorAuthenticationViewModel model, string button)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (button == "submit")
            {
                if (user.TwoFactorTypeId == (int)TwoFactorTypes.Totp)
                {
                    if (_totpService.Validate(user.TotpSecret, model.Code))
                    {
                        return await Authenticated(user, model);
                    }
                    else
                    {
                        return await AccessDenied(user, model);
                    }
                }
                else
                {
                    if (model.Code == user.TwoFactorCode)
                    {
                        return await Authenticated(user, model);
                    }
                    else
                    {
                        return await AccessDenied(user, model);
                    }
                }
            }
            else
            {
                switch (user.TwoFactorTypeId)
                {
                    case (int)TwoFactorTypes.Email:
                        await SendCodeToEmail(user);
                        break;
                    case (int)TwoFactorTypes.Sms:
                        await SendSmsCode(user);
                        break;
                }
                
                model.Code = null;
                model.DisplayText = GetDisplayText(user);
                return View(model);
            }
        }

        private async Task<IActionResult> Authenticated(ApplicationUser user, TwoFactorAuthenticationViewModel model)
        {
            await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));
            if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("~/");
        }

        private async Task<IActionResult> AccessDenied(ApplicationUser user, TwoFactorAuthenticationViewModel model)
        {
            await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid authentication code"));
            ModelState.AddModelError("Invalid Code", "Invalid Code");
            model.Code = null;
            return View(model);
        }

        private async Task SendCodeToEmail(ApplicationUser user)
        {
            var code = GenerateOtpCode();
            await SaveOtpCode(user, code);
            var email = new EmailModel()
            {
                ToAddress = user.NormalizedEmail,
                Subject = "Access Code",
                Body = BuildMessage(code)
            };
            await _emailService.SendEmail(email);
        }

        private async Task SendSmsCode(ApplicationUser user)
        {
            var code = GenerateOtpCode();
            await SaveOtpCode(user, code);
            await _twilioService.SendSms(user.PhoneNumber, BuildMessage(code));
        }

        private string GenerateOtpCode()
        {
            var rng = new Random();
            var code = rng.Next(0, 999999).ToString("000000");
            return code;
        }

        private async Task SaveOtpCode(ApplicationUser user, string code)
        {
            user.TwoFactorCode = code;
            await _userManager.UpdateAsync(user);
        }

        private string BuildMessage(string code)
        {
            return "Your access code is " + code;
        }

        private string GetDisplayText(ApplicationUser user)
        {
            switch (user.TwoFactorTypeId)
            {
                case (int)TwoFactorTypes.Email:
                    return _emailDisplayText;
                case (int)TwoFactorTypes.Sms:
                    return  _smsDisplayText;
                case (int)TwoFactorTypes.Totp:
                    return _totpDisplayText;
                default:
                    return _emailDisplayText;
            }
        }
    }
}
