using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class CreateAppointmentRequest
    {
        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [MaxLength(500)]
        public string? ReasonForVisit { get; set; }
    }

    public class UpdateAppointmentRequest
    {
        [MaxLength(500)]
        public string? ReasonForVisit { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class CancelAppointmentRequest
    {
        [Required]
        [MaxLength(500)]
        public string CancellationReason { get; set; } = string.Empty;
    }

    public class UpdateAppointmentStatusRequest
    {
        [Required]
        public AppointmentStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class DoctorAvailabilityRequest
    {
        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}

 
