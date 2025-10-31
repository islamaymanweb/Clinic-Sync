using Core.Entities;
using Core.interfaces;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repositories
{
    public class ScheduleExceptionRepository : IScheduleExceptionRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleExceptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleException?> GetByIdAsync(Guid id)
        {
            return await _context.ScheduleExceptions
                .Include(se => se.Doctor)
                .FirstOrDefaultAsync(se => se.Id == id);
        }

        public async Task<IEnumerable<ScheduleException>> GetAllAsync()
        {
            return await _context.ScheduleExceptions
                .Include(se => se.Doctor)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleException>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date)
        {
            return await _context.ScheduleExceptions
                .Where(se => se.DoctorId == doctorId &&
                           se.ExceptionDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleException>> GetByDoctorAndDateRangeAsync(Guid doctorId, DateTime startDate, DateTime endDate)
        {
            return await _context.ScheduleExceptions
                .Where(se => se.DoctorId == doctorId &&
                           se.ExceptionDate >= startDate.Date &&
                           se.ExceptionDate <= endDate.Date)
                .ToListAsync();
        }

        public async Task<bool> HasExceptionForDateAsync(Guid doctorId, DateTime date)
        {
            return await _context.ScheduleExceptions
                .AnyAsync(se => se.DoctorId == doctorId &&
                              se.ExceptionDate.Date == date.Date);
        }

        public async Task AddAsync(ScheduleException entity)
        {
            await _context.ScheduleExceptions.AddAsync(entity);
        }

        public void Update(ScheduleException entity)
        {
            _context.ScheduleExceptions.Update(entity);
        }

        public void Delete(ScheduleException entity)
        {
            _context.ScheduleExceptions.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
