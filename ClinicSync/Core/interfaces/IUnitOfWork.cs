using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAppointmentRepository Appointments { get; }
        IDoctorScheduleRepository DoctorSchedules { get; }
        IScheduleExceptionRepository ScheduleExceptions { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
