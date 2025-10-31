using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        // Navigation properties
        public virtual AppUser User { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

    public enum Gender
    {
        Male,
        Female
    }
}
