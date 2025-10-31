using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;

        [MaxLength(500)]
        public string? ReasonForVisit { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(20)]
        public string ReferenceNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
    }

    public enum AppointmentStatus
    {
        Confirmed,     
        Completed,    
        Cancelled,    
        NoShow        
    }
}
