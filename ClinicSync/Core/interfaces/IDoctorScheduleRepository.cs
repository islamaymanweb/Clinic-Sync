using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.interfaces
{
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        Task<DoctorSchedule?> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek dayOfWeek);
        Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId);
        Task<bool> HasScheduleForDayAsync(Guid doctorId, DayOfWeek dayOfWeek);
    }
}
