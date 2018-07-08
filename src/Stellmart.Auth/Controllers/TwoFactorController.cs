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
        private readonly IIdentityServerInteractionService _interaction;

        private const string _emailDisplayText = "A code has been sent to your email. Please enter it below:";


        public TwoFactorController(
            UserManager<ApplicationUser> userManager,
            IEventService events,
            IEmailService emailService,
            IIdentityServerInteractionService interaction
            )
        {
            _userManager = userManager;
            _events = events;
            _emailService = emailService;
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
            }
            var vm = new TwoFactorAuthenticationViewModel()
            {
                ReturnUrl = returnUrl,
                Username = username,
                DisplayText = _emailDisplayText
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TwoFactorAuthenticationViewModel model, string button)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (button == "login")
            {
                if (model.Code == user.TwoFactorCode)
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }
                else
                {
                    ModelState.AddModelError("Invalid Code", "Invalid Code");
                    model.Code = null;
                    return View(model);
                }
            }
            else
            {
                await SendCodeToEmail(user);
                model.Code = null;
                switch (user.TwoFactorTypeId)
                {
                    case (int)TwoFactorTypes.Email:
                        model.DisplayText = _emailDisplayText;
                        break;
                }
                return View(model);
            }
        }

        private async Task SendCodeToEmail(ApplicationUser user)
        {
            var rng = new Random();
            var code = rng.Next(0, 999999).ToString("000000");
            user.TwoFactorCode = code;
            await _userManager.UpdateAsync(user);
            var email = new EmailModel()
            {
                ToAddress = user.NormalizedEmail,
                Subject = "Access Code",
                Body = "Your access code is " + code
            };
            await _emailService.SendEmail(email);
        }
    }
}
