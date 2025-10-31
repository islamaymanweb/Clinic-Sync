using Core.DTO;
using Core.Entities;
using Core.Services;
using infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedDoctorsResponse>> GetDoctors(
            [FromQuery] DoctorSearchRequest request)
        {
            try
            {
                var result = await _doctorService.SearchDoctorsAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors");
                return StatusCode(500, new PaginatedDoctorsResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving doctors"
                });
            }
        }

        [HttpGet("specialties")]
        public async Task<ActionResult<List<Specialty>>> GetSpecialties()
        {
            try
            {
                var specialties = await _doctorService.GetSpecialtiesAsync();
                return Ok(specialties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting specialties");
                return StatusCode(500, new List<Specialty>());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorSearchResult>> GetDoctor(Guid id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor {DoctorId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving doctor" });
            }
        }

        [HttpGet("suggestions")]
        public async Task<ActionResult<List<string>>> GetSearchSuggestions([FromQuery] string query)
        {
            try
            {
                var suggestions = await _doctorService.GetSearchSuggestionsAsync(query);
                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search suggestions");
                return StatusCode(500, new List<string>());
            }
        }
    }
}