using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SpecialtyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        public int YearsOfExperience { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsultationFee { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        public bool IsApproved { get; set; } = false;

        // Navigation properties
        public virtual AppUser User { get; set; } = null!;
        public virtual Specialty Specialty { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
 
        public virtual ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
        public virtual ICollection<ScheduleException> ScheduleExceptions { get; set; } = new List<ScheduleException>();
    }
}
