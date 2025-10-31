using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public string GenerateJwtToken(AppUser user, bool rememberMe = false)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("role", user.Role.ToString()),
                new Claim("avatar", user.ProfilePictureUrl ?? ""),
                new Claim("email_verified", user.EmailVerified.ToString())
            };

            // Add role claims
            var userRoles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenExpiry = rememberMe ?
                DateTime.UtcNow.AddDays(7) : // Remember me for 7 days
                DateTime.UtcNow.AddDays(1);  // Normal session 1 day

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: tokenExpiry,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(AppUser user, bool rememberMe = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("role", user.Role.ToString()),
                new Claim("avatar", user.ProfilePictureUrl ?? ""),
                new Claim("email_verified", user.EmailVerified.ToString())
            };

            // Add role claims
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }

        public async Task ClearAuthCookie(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear auth cookie
            context.Response.Cookies.Delete("ClinicSync.Auth", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }
    }
}
