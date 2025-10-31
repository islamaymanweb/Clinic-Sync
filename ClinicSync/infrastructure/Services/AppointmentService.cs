using Core.DTO;
using Core.Entities;
using Core.interfaces;
using Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IUnitOfWork unitOfWork, ILogger<AppointmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AppointmentResponse?> GetAppointmentByIdAsync(Guid id)
        {
            try
            {
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
                return appointment != null ? MapToAppointmentResponse(appointment) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment by ID: {AppointmentId}", id);
                throw;
            }
        }

        public async Task<AppointmentResponse?> GetAppointmentByReferenceAsync(string referenceNumber)
        {
            try
            {
                var appointment = await _unitOfWork.Appointments.GetByReferenceNumberAsync(referenceNumber);
                return appointment != null ? MapToAppointmentResponse(appointment) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment by reference: {ReferenceNumber}", referenceNumber);
                throw;
            }
        }

        public async Task<PaginatedAppointmentsResponse> GetPatientAppointmentsAsync(Guid patientId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var appointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patientId);
                var totalCount = appointments.Count();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedAppointments = appointments
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToAppointmentResponse)
                    .ToList();

                return new PaginatedAppointmentsResponse
                {
                    Success = true,
                    Message = "Appointments retrieved successfully",
                    Data = pagedAppointments,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                    HasPrevious = pageNumber > 1,
                    HasNext = pageNumber < totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient appointments for patient: {PatientId}", patientId);
                throw;
            }
        }

        public async Task<PaginatedAppointmentsResponse> GetDoctorAppointmentsAsync(Guid doctorId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var appointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(doctorId);
                var totalCount = appointments.Count();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedAppointments = appointments
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToAppointmentResponse)
                    .ToList();

                return new PaginatedAppointmentsResponse
                {
                    Success = true,
                    Message = "Appointments retrieved successfully",
                    Data = pagedAppointments,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                    HasPrevious = pageNumber > 1,
                    HasNext = pageNumber < totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor appointments for doctor: {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<IEnumerable<AppointmentResponse>> GetTodayAppointmentsAsync(Guid doctorId)
        {
            try
            {
                var today = DateTime.Today;
                var appointments = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, today);

                return appointments
                    .OrderBy(a => a.StartTime)
                    .Select(MapToAppointmentResponse)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting today's appointments for doctor: {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<AvailabilityResponse> GetDoctorAvailabilityAsync(DoctorAvailabilityRequest request)
        {
            try
            {
                // Check if date is in past
                if (request.Date.Date < DateTime.Today)
                {
                    return new AvailabilityResponse
                    {
                        Success = false,
                        Message = "Cannot check availability for past dates"
                    };
                }

                // Check if doctor has schedule for this day
                var dayOfWeek = request.Date.DayOfWeek;
                var schedule = await _unitOfWork.DoctorSchedules.GetByDoctorAndDayAsync(request.DoctorId, dayOfWeek);

                if (schedule == null || !schedule.IsActive)
                {
                    return new AvailabilityResponse
                    {
                        Success = false,
                        Message = "Doctor is not available on this day"
                    };
                }

                // Check for exceptions
                var exceptions = await _unitOfWork.ScheduleExceptions.GetByDoctorAndDateAsync(request.DoctorId, request.Date);
                if (exceptions.Any(e => e.Type == ExceptionType.DayOff))
                {
                    return new AvailabilityResponse
                    {
                        Success = false,
                        Message = "Doctor is on leave on this day"
                    };
                }

                // Get existing appointments
                var existingAppointments = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(request.DoctorId, request.Date);

                // Generate time slots (30 minutes each)
                var timeSlots = GenerateTimeSlots(schedule.StartTime, schedule.EndTime, TimeSpan.FromMinutes(30));

                // Filter available slots
                var availableSlots = timeSlots.Select(slot => new TimeSlotResponse
                {
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    IsAvailable = !IsTimeSlotBooked(slot, existingAppointments) &&
                                 !IsTimeSlotInException(slot, exceptions)
                }).ToList();

                return new AvailabilityResponse
                {
                    Success = true,
                    Message = "Availability retrieved successfully",
                    TimeSlots = availableSlots
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor availability for doctor: {DoctorId} on date: {Date}",
                    request.DoctorId, request.Date);

                return new AvailabilityResponse
                {
                    Success = false,
                    Message = "An error occurred while checking availability"
                };
            }
        }

        public async Task<AppointmentResponse> CreateAppointmentAsync(CreateAppointmentRequest request, Guid patientId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate time slot availability
                var endTime = request.StartTime.Add(TimeSpan.FromMinutes(30)); // Fixed 30-minute duration

                var isAvailable = await _unitOfWork.Appointments.IsTimeSlotAvailableAsync(
                    request.DoctorId, request.AppointmentDate, request.StartTime, endTime);

                if (!isAvailable)
                {
                    throw new InvalidOperationException("The selected time slot is no longer available");
                }

                // Create appointment
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    DoctorId = request.DoctorId,
                    AppointmentDate = request.AppointmentDate.Date,
                    StartTime = request.StartTime,
                    EndTime = endTime,
                    ReasonForVisit = request.ReasonForVisit,
                    ReferenceNumber = GenerateReferenceNumber(),
                    Status = AppointmentStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Appointments.AddAsync(appointment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Get the complete appointment with relationships
                var createdAppointment = await _unitOfWork.Appointments.GetByIdAsync(appointment.Id);

                if (createdAppointment == null)
                {
                    throw new Exception("Failed to retrieve created appointment");
                }

                _logger.LogInformation("Appointment created successfully: {ReferenceNumber}", appointment.ReferenceNumber);

                return MapToAppointmentResponse(createdAppointment);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating appointment for patient: {PatientId}", patientId);
                throw;
            }
        }

        public async Task<AppointmentResponse?> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequest request)
        {
            try
            {
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
                if (appointment == null) return null;

                appointment.ReasonForVisit = request.ReasonForVisit ?? appointment.ReasonForVisit;
                appointment.Notes = request.Notes ?? appointment.Notes;
                appointment.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Appointments.Update(appointment);
                await _unitOfWork.SaveChangesAsync();

                return MapToAppointmentResponse(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment: {AppointmentId}", id);
                throw;
            }
        }

        public async Task<bool> CancelAppointmentAsync(Guid id, CancelAppointmentRequest request, Guid userId, string userRole)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
                if (appointment == null) return false;

                // Check permissions
                if (userRole == "Patient" && appointment.PatientId != userId)
                {
                    throw new UnauthorizedAccessException("You can only cancel your own appointments");
                }

                if (userRole == "Doctor" && appointment.DoctorId != userId)
                {
                    throw new UnauthorizedAccessException("You can only cancel your own appointments");
                }

                // Check cancellation policy (24 hours)
                var appointmentDateTime = appointment.AppointmentDate.Add(appointment.StartTime);
                if (DateTime.Now.AddHours(24) > appointmentDateTime && userRole == "Patient")
                {
                    throw new InvalidOperationException("Appointments can only be cancelled at least 24 hours in advance");
                }

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.CancellationReason = request.CancellationReason;
                appointment.CancelledAt = DateTime.UtcNow;
                appointment.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Appointments.Update(appointment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Appointment cancelled: {ReferenceNumber}", appointment.ReferenceNumber);
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error cancelling appointment: {AppointmentId}", id);
                throw;
            }
        }

        public async Task<AppointmentResponse?> UpdateAppointmentStatusAsync(Guid id, UpdateAppointmentStatusRequest request, Guid doctorId)
        {
            try
            {
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
                if (appointment == null || appointment.DoctorId != doctorId) return null;

                appointment.Status = request.Status;
                appointment.Notes = request.Notes ?? appointment.Notes;
                appointment.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Appointments.Update(appointment);
                await _unitOfWork.SaveChangesAsync();

                return MapToAppointmentResponse(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment status: {AppointmentId}", id);
                throw;
            }
        }

        public async Task<bool> CanCancelAppointmentAsync(Guid appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            var appointmentDateTime = appointment.AppointmentDate.Add(appointment.StartTime);
            return DateTime.Now.AddHours(24) <= appointmentDateTime;
        }

        // Helper methods
        private List<(TimeSpan StartTime, TimeSpan EndTime)> GenerateTimeSlots(TimeSpan start, TimeSpan end, TimeSpan duration)
        {
            var slots = new List<(TimeSpan, TimeSpan)>();
            var current = start;

            while (current + duration <= end)
            {
                slots.Add((current, current + duration));
                current = current + duration;
            }

            return slots;
        }

        private bool IsTimeSlotBooked((TimeSpan StartTime, TimeSpan EndTime) slot, IEnumerable<Appointment> appointments)
        {
            return appointments.Any(apt =>
                (slot.StartTime >= apt.StartTime && slot.StartTime < apt.EndTime) ||
                (slot.EndTime > apt.StartTime && slot.EndTime <= apt.EndTime) ||
                (slot.StartTime <= apt.StartTime && slot.EndTime >= apt.EndTime));
        }

        private bool IsTimeSlotInException((TimeSpan StartTime, TimeSpan EndTime) slot, IEnumerable<ScheduleException> exceptions)
        {
            foreach (var exception in exceptions)
            {
                if (exception.Type == ExceptionType.DayOff) return true;

                if (exception.StartTime.HasValue && exception.EndTime.HasValue)
                {
                    if ((slot.StartTime >= exception.StartTime.Value && slot.StartTime < exception.EndTime.Value) ||
                        (slot.EndTime > exception.StartTime.Value && slot.EndTime <= exception.EndTime.Value) ||
                        (slot.StartTime <= exception.StartTime.Value && slot.EndTime >= exception.EndTime.Value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string GenerateReferenceNumber()
        {
            return $"APT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        private AppointmentResponse MapToAppointmentResponse(Appointment appointment)
        {
            return new AppointmentResponse
            {
                Id = appointment.Id,
                ReferenceNumber = appointment.ReferenceNumber,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString(),
                ReasonForVisit = appointment.ReasonForVisit,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt,
                CancelledAt = appointment.CancelledAt,
                CancellationReason = appointment.CancellationReason,
                Doctor = new DoctorInfo
                {
                    Id = appointment.Doctor.Id,
                    FullName = appointment.Doctor.User.FullName,
                    Specialty = appointment.Doctor.Specialty.Name,
                    ConsultationFee = appointment.Doctor.ConsultationFee,
                    ProfilePictureUrl = appointment.Doctor.User.ProfilePictureUrl
                },
                Patient = new PatientInfo
                {
                    Id = appointment.Patient.Id,
                    FullName = appointment.Patient.User.FullName,
                    Email = appointment.Patient.User.Email!,
                    PhoneNumber = appointment.Patient.PhoneNumber
                }
            };
        }
    }
}
