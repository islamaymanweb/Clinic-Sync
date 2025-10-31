using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ReasonForVisit { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }

        // Doctor info
        public DoctorInfo Doctor { get; set; } = null!;

        // Patient info (for doctor/admin)
        public PatientInfo? Patient { get; set; }
    }

    public class DoctorInfo
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public decimal ConsultationFee { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    public class PatientInfo
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class TimeSlotResponse
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AvailabilityResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<TimeSlotResponse>? TimeSlots { get; set; }
    }

    public class PaginatedAppointmentsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<AppointmentResponse>? Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
