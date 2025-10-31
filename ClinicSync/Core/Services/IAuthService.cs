using Core.DTO;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request, HttpContext httpContext);
        Task<AuthResponse> VerifyEmailAsync(string token, string email);
        Task<AuthResponse> ForgotPasswordAsync(string email);
        Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<AuthResponse> CreateDoctorAsync(CreateDoctorRequest request, Guid adminId);
        Task<AppUser?> GetUserByIdAsync(Guid userId);
    }
}
