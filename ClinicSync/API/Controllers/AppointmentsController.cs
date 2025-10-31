using Core.DTO;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedAppointmentsResponse>> GetMyAppointments(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value!;

                PaginatedAppointmentsResponse response;

                if (userRole == "Patient")
                {
                    response = await _appointmentService.GetPatientAppointmentsAsync(userId, pageNumber, pageSize);
                }
                else if (userRole == "Doctor")
                {
                    response = await _appointmentService.GetDoctorAppointmentsAsync(userId, pageNumber, pageSize);
                }
                else
                {
                    return Forbid();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for user");
                return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
            }
        }

        [HttpGet("today")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetTodayAppointments()
        {
            try
            {
                var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var appointments = await _appointmentService.GetTodayAppointmentsAsync(doctorId);

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting today's appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving today's appointments" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentResponse>> GetAppointment(Guid id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                // Check permissions
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value!;

                if (userRole == "Patient" && appointment.Patient?.Id != userId)
                {
                    return Forbid();
                }

                if (userRole == "Doctor" && appointment.Doctor.Id != userId)
                {
                    return Forbid();
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the appointment" });
            }
        }

        [HttpGet("reference/{referenceNumber}")]
        public async Task<ActionResult<AppointmentResponse>> GetAppointmentByReference(string referenceNumber)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByReferenceAsync(referenceNumber);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                // Check permissions
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value!;

                if (userRole == "Patient" && appointment.Patient?.Id != userId)
                {
                    return Forbid();
                }

                if (userRole == "Doctor" && appointment.Doctor.Id != userId)
                {
                    return Forbid();
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment by reference: {ReferenceNumber}", referenceNumber);
                return StatusCode(500, new { message = "An error occurred while retrieving the appointment" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<AppointmentResponse>> CreateAppointment(CreateAppointmentRequest request)
        {
            try
            {
                var patientId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var appointment = await _appointmentService.CreateAppointmentAsync(request, patientId);

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, new { message = "An error occurred while creating the appointment" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AppointmentResponse>> UpdateAppointment(Guid id, UpdateAppointmentRequest request)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentAsync(id, request);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                // Check permissions
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value!;

                if (userRole == "Patient" && appointment.Patient?.Id != userId)
                {
                    return Forbid();
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the appointment" });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(Guid id, CancelAppointmentRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value!;

                var result = await _appointmentService.CancelAppointmentAsync(id, request, userId, userRole);
                if (!result)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                return Ok(new { message = "Appointment cancelled successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while cancelling the appointment" });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<AppointmentResponse>> UpdateAppointmentStatus(
            Guid id, UpdateAppointmentStatusRequest request)
        {
            try
            {
                var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var appointment = await _appointmentService.UpdateAppointmentStatusAsync(id, request, doctorId);

                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment status: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the appointment status" });
            }
        }

        [HttpGet("availability")]
        public async Task<ActionResult<AvailabilityResponse>> GetDoctorAvailability(
            [FromQuery] DoctorAvailabilityRequest request)
        {
            try
            {
                var response = await _appointmentService.GetDoctorAvailabilityAsync(request);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor availability");
                return StatusCode(500, new AvailabilityResponse
                {
                    Success = false,
                    Message = "An error occurred while checking availability"
                });
            }
        }

        [HttpGet("{id}/can-cancel")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<bool>> CanCancelAppointment(Guid id)
        {
            try
            {
                var canCancel = await _appointmentService.CanCancelAppointmentAsync(id);
                return Ok(canCancel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if appointment can be cancelled: {AppointmentId}", id);
                return StatusCode(500, false);
            }
        }
    }
}
