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
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorSchedule?> GetByIdAsync(Guid id)
        {
            return await _context.DoctorSchedules
                .Include(ds => ds.Doctor)
                .FirstOrDefaultAsync(ds => ds.Id == id);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetAllAsync()
        {
            return await _context.DoctorSchedules
                .Include(ds => ds.Doctor)
                .ToListAsync();
        }

        public async Task<DoctorSchedule?> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek dayOfWeek)
        {
            return await _context.DoctorSchedules
                .FirstOrDefaultAsync(ds => ds.DoctorId == doctorId &&
                                         ds.DayOfWeek == dayOfWeek &&
                                         ds.IsActive);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId && ds.IsActive)
                .OrderBy(ds => ds.DayOfWeek)
                .ToListAsync();
        }

        public async Task<bool> HasScheduleForDayAsync(Guid doctorId, DayOfWeek dayOfWeek)
        {
            return await _context.DoctorSchedules
                .AnyAsync(ds => ds.DoctorId == doctorId &&
                              ds.DayOfWeek == dayOfWeek &&
                              ds.IsActive);
        }

        public async Task AddAsync(DoctorSchedule entity)
        {
            await _context.DoctorSchedules.AddAsync(entity);
        }

        public void Update(DoctorSchedule entity)
        {
            _context.DoctorSchedules.Update(entity);
        }

        public void Delete(DoctorSchedule entity)
        {
            _context.DoctorSchedules.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}