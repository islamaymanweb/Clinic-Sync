using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(AppUser user, bool rememberMe = false);
        ClaimsPrincipal? ValidateToken(string token);
        Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(AppUser user, bool rememberMe = false);
        Task ClearAuthCookie(HttpContext context);
    }

}
