using Core.DTO;
using Core.Entities;
using Core.Services;
using infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
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

    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            ApplicationDbContext context,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, HttpContext httpContext)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new AuthResponse { Success = false, Message = "Invalid credentials" };
                }

                if (!user.IsActive)
                {
                    return new AuthResponse { Success = false, Message = "Account is deactivated" };
                }

                if (!user.EmailVerified)
                {
                    return new AuthResponse { Success = false, Message = "Please verify your email before logging in" };
                }

                var result = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!result)
                {
                    return new AuthResponse { Success = false, Message = "Invalid credentials" };
                }

                // For doctors, check if approved
                if (user.Role == UserRole.Doctor)
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                    if (doctor == null || !doctor.IsApproved)
                    {
                        return new AuthResponse { Success = false, Message = "Doctor account pending approval" };
                    }
                }

                // Create claims principal
                var claimsPrincipal = await _tokenService.CreateClaimsPrincipalAsync(user, request.RememberMe);
                var token = _tokenService.GenerateJwtToken(user, request.RememberMe);

                // Set authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = request.RememberMe,
                    ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddDays(1),
                    AllowRefresh = true
                };

                // Sign in user
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);

                // Set HTTP-only cookie with JWT token
                httpContext.Response.Cookies.Append("ClinicSync.Auth", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = request.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddDays(1),
                    Path = "/"
                });

                var userInfo = new UserInfo
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Role = user.Role.ToString(),
                    ProfilePictureUrl = user.ProfilePictureUrl
                };

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful",
                    User = userInfo
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Login failed: {ex.Message}" };
            }
        }
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new AuthResponse { Success = false, Message = "User already exists with this email" };
                }

                // Create new user
                var user = new AppUser
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    UserName = request.Email,
                    Email = request.Email,
                    Role = UserRole.Patient,
                    IsActive = true,
                    EmailVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "User creation failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // Add to Patient role
                await _userManager.AddToRoleAsync(user, "Patient");

                // Create patient profile
                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                // Send email verification
                var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(verificationToken));

                await _emailService.SendEmailVerificationAsync(user.Email!, user.FullName, encodedToken);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful. Please check your email for verification."
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Registration failed: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> VerifyEmailAsync(string token, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new AuthResponse { Success = false, Message = "User not found" };
                }

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    user.EmailVerified = true;
                    await _userManager.UpdateAsync(user);

                    return new AuthResponse { Success = true, Message = "Email verified successfully" };
                }

                return new AuthResponse
                {
                    Success = false,
                    Message = "Email verification failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Verification failed: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !user.EmailVerified)
                {
                    // Don't reveal that the user doesn't exist
                    return new AuthResponse { Success = true, Message = "If the email exists, a reset link has been sent" };
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                await _emailService.SendPasswordResetAsync(user.Email!, user.FullName, encodedToken);

                return new AuthResponse { Success = true, Message = "If the email exists, a reset link has been sent" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Password reset failed: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new AuthResponse { Success = false, Message = "Invalid request" };
                }

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

                if (result.Succeeded)
                {
                    return new AuthResponse { Success = true, Message = "Password reset successfully" };
                }

                return new AuthResponse
                {
                    Success = false,
                    Message = "Password reset failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Password reset failed: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> CreateDoctorAsync(CreateDoctorRequest request, Guid adminId)
        {
            try
            {
                // Verify admin exists
                var admin = await _userManager.FindByIdAsync(adminId.ToString());
                if (admin == null || admin.Role != UserRole.Admin)
                {
                    return new AuthResponse { Success = false, Message = "Unauthorized" };
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new AuthResponse { Success = false, Message = "User already exists with this email" };
                }

                // Check if specialty exists
                var specialty = await _context.Specialties.FindAsync(request.SpecialtyId);
                if (specialty == null)
                {
                    return new AuthResponse { Success = false, Message = "Invalid specialty" };
                }

                // Generate temporary password
                var tempPassword = GenerateTemporaryPassword();

                // Create doctor user
                var user = new AppUser
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    UserName = request.Email,
                    Email = request.Email,
                    Role = UserRole.Doctor,
                    IsActive = true,
                    EmailVerified = true, // Auto-verify for doctors created by admin
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, tempPassword);
                if (!result.Succeeded)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Doctor creation failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // Add to Doctor role
                await _userManager.AddToRoleAsync(user, "Doctor");

                // Create doctor profile
                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    SpecialtyId = request.SpecialtyId,
                    LicenseNumber = request.LicenseNumber,
                    YearsOfExperience = request.YearsOfExperience,
                    ConsultationFee = request.ConsultationFee,
                    Bio = request.Bio,
                    IsApproved = true // Auto-approve for admin-created doctors
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                // Send credentials to doctor
                await _emailService.SendDoctorCredentialsAsync(user.Email!, user.FullName, request.Email, tempPassword);

                return new AuthResponse { Success = true, Message = "Doctor account created successfully" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Doctor creation failed: {ex.Message}" };
            }
        }

        public async Task<AppUser?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                return await _userManager.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Doctor)
                    .ThenInclude(d => d.Specialty)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error getting user by ID: {ex.Message}");
                return null;
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%";
            var random = new Random();
            var chars = new char[8];

            for (int i = 0; i < 8; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }
    }
}
