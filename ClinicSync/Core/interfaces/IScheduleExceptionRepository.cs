using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.interfaces
{
    public interface IScheduleExceptionRepository : IRepository<ScheduleException>
    {
        Task<IEnumerable<ScheduleException>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date);
        Task<IEnumerable<ScheduleException>> GetByDoctorAndDateRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate);
        Task<bool> HasExceptionForDateAsync(Guid doctorId, DateTime date);
    }

}
