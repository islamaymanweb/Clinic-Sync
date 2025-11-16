using Core.DTO;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("New user registered: {Email}", request.Email);
                    return Ok(new ApiResponse<AuthResponse>
                    {
                        Success = true,
                        Message = result.Message,
                        Data = result
                    });
                }

                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", request.Email);
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "An error occurred during registration"
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
        {
            try
            {
                
                var result = await _authService.LoginAsync(request, HttpContext);

                if (result.Success)
                {
                    _logger.LogInformation("User logged in: {Email}", request.Email);
                    return Ok(new ApiResponse<AuthResponse>
                    {
                        Success = true,
                        Message = result.Message,
                        Data = result
                    });
                }

                return Unauthorized(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", request.Email);
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                await _tokenService.ClearAuthCookie(HttpContext);

                _logger.LogInformation("User logged out");
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logout successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during logout"
                });
            }
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Cookies,Bearer")]
        public async Task<ActionResult<ApiResponse<UserInfo>>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<UserInfo>
                    {
                        Success = false,
                        Message = "User not authenticated"
                    });
                }

                var user = await _authService.GetUserByIdAsync(Guid.Parse(userId));
                if (user == null)
                {
                    return NotFound(new ApiResponse<UserInfo>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                var userInfo = new UserInfo
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Role = user.Role.ToString(),
                    ProfilePictureUrl = user.ProfilePictureUrl
                };

                return Ok(new ApiResponse<UserInfo>
                {
                    Success = true,
                    Message = "User data retrieved successfully",
                    Data = userInfo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user");
                return StatusCode(500, new ApiResponse<UserInfo>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user data"
                });
            }
        }

       
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var result = await _authService.ForgotPasswordAsync(request.Email);

                return Ok(new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for {Email}", request.Email);
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "An error occurred processing your request"
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Password reset for: {Email}", request.Email);
                    return Ok(new ApiResponse<AuthResponse>
                    {
                        Success = true,
                        Message = result.Message,
                        Data = result
                    });
                }

                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for {Email}", request.Email);
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "An error occurred during password reset"
                });
            }
        }
        
    }
}
