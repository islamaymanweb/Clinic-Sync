using Core.DTO;
using Core.Services;
using infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAuthService authService,
            ApplicationDbContext context,
            ILogger<AdminController> logger)
        {
            _authService = authService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("doctors")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> CreateDoctor(CreateDoctorRequest request)
        {
            try
            {
                var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
                var result = await _authService.CreateDoctorAsync(request, adminId);

                if (result.Success)
                {
                    _logger.LogInformation("Doctor created by admin {AdminId}: {Email}", adminId, request.Email);
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
                _logger.LogError(ex, "Error creating doctor by admin");
                return StatusCode(500, new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Message = "An error occurred while creating doctor account"
                });
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<object>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.FullName,
                        u.Email,
                        Role = u.Role.ToString(),
                        u.IsActive,
                       
                        u.CreatedAt,
                        // ✅ بيانات محدودة فقط - أكثر أماناً
                        HasPatientProfile = u.Patient != null,
                        HasDoctorProfile = u.Doctor != null,
                        IsDoctorApproved = u.Doctor != null ? u.Doctor.IsApproved : (bool?)null,
                        Specialty = u.Doctor != null ? u.Doctor.Specialty.Name : null
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving users"
                });
            }
        }
    }
}
