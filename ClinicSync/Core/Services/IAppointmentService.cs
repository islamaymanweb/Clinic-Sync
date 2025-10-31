using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse?> GetAppointmentByIdAsync(Guid id);
        Task<AppointmentResponse?> GetAppointmentByReferenceAsync(string referenceNumber);
        Task<PaginatedAppointmentsResponse> GetPatientAppointmentsAsync(Guid patientId, int pageNumber = 1, int pageSize = 10);
        Task<PaginatedAppointmentsResponse> GetDoctorAppointmentsAsync(Guid doctorId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<AppointmentResponse>> GetTodayAppointmentsAsync(Guid doctorId);
        Task<AvailabilityResponse> GetDoctorAvailabilityAsync(DoctorAvailabilityRequest request);
        Task<AppointmentResponse> CreateAppointmentAsync(CreateAppointmentRequest request, Guid patientId);
        Task<AppointmentResponse?> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequest request);
        Task<bool> CancelAppointmentAsync(Guid id, CancelAppointmentRequest request, Guid userId, string userRole);
        Task<AppointmentResponse?> UpdateAppointmentStatusAsync(Guid id, UpdateAppointmentStatusRequest request, Guid doctorId);
        Task<bool> CanCancelAppointmentAsync(Guid appointmentId);
    }
}
