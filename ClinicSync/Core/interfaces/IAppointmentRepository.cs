using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date);
        Task<Appointment?> GetByReferenceNumberAsync(string referenceNumber);
        Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<int> GetAppointmentsCountByDoctorAndDateAsync(Guid doctorId, DateTime date);
    }
}
