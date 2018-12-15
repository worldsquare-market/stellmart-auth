using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Stellmart.Auth.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Stellmart.Auth.Services
{
    public class ClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClaimsProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();

            var typeClaim = claims.Where(c => c.Value == "2fa_type").FirstOrDefault();
            if (typeClaim != null)
            {
                claims.Remove(typeClaim);
            }
            claims.Add(new Claim("2fa_type", user.TwoFactorTypeId.ToString()));

            var timeClaim = claims.Where(c => c.Value == "2fa_time").FirstOrDefault();
            if (timeClaim != null)
            {
                claims.Remove(timeClaim);
            }
            if (user.TwoFactorAuthTime != null)
            {
                claims.Add(new Claim("2fa_time", (Math.Round(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)).ToString()));
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
