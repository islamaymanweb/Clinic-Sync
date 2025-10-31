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
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Include(a => a.Doctor.Specialty)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor.User)
                .Include(a => a.Doctor.Specialty)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Include(a => a.Doctor.Specialty)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient.User)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == date.Date &&
                           a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByReferenceNumberAsync(string referenceNumber)
        {
            return await _context.Appointments
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor.User)
                .Include(a => a.Doctor.Specialty)
                .FirstOrDefaultAsync(a => a.ReferenceNumber == referenceNumber);
        }

        public async Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            return !await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId &&
                             a.AppointmentDate.Date == date.Date &&
                             a.Status != AppointmentStatus.Cancelled &&
                             ((startTime >= a.StartTime && startTime < a.EndTime) ||
                              (endTime > a.StartTime && endTime <= a.EndTime) ||
                              (startTime <= a.StartTime && endTime >= a.EndTime)));
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            return await _context.Appointments
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor.User)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<int> GetAppointmentsCountByDoctorAndDateAsync(Guid doctorId, DateTime date)
        {
            return await _context.Appointments
                .CountAsync(a => a.DoctorId == doctorId &&
                               a.AppointmentDate.Date == date.Date &&
                               a.Status != AppointmentStatus.Cancelled);
        }

        public async Task AddAsync(Appointment entity)
        {
            await _context.Appointments.AddAsync(entity);
        }

        public void Update(Appointment entity)
        {
            _context.Appointments.Update(entity);
        }

        public void Delete(Appointment entity)
        {
            _context.Appointments.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
