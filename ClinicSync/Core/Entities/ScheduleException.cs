using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ScheduleException
    {
        public Guid Id { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime ExceptionDate { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        [Required]
        public ExceptionType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Reason { get; set; } = string.Empty;

        // Navigation properties
        public virtual Doctor Doctor { get; set; } = null!;
    }

    public enum ExceptionType
    {
        DayOff,     
        Busy,       
        Emergency   
    }
}
