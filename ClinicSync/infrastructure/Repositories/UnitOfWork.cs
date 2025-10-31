using Core.interfaces;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Appointments = new AppointmentRepository(_context);
            DoctorSchedules = new DoctorScheduleRepository(_context);
            ScheduleExceptions = new ScheduleExceptionRepository(_context);
        }

        public IAppointmentRepository Appointments { get; private set; }
        public IDoctorScheduleRepository DoctorSchedules { get; private set; }
        public IScheduleExceptionRepository ScheduleExceptions { get; private set; }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
