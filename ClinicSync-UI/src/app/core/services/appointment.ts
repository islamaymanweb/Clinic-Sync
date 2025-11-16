import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppointmentResponse, AvailabilityResponse, CancelAppointmentRequest, CreateAppointmentRequest, DoctorAvailabilityRequest, PaginatedAppointmentsResponse, UpdateAppointmentRequest, UpdateAppointmentStatusRequest } from '../../shared/models/CreateAppointmentRequest ';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class Appointment {
  private readonly baseUrl = `${environment.apiUrl}/api/appointments`;

  constructor(private http: HttpClient) {}
 // تحديث دالة الحصول على التوافر
  getDoctorAvailability(request: DoctorAvailabilityRequest): Observable<AvailabilityResponse> {
    const params = new HttpParams()
      .set('doctorId', request.doctorId)
      .set('date', request.date);

    const url = `${this.baseUrl}/availability`;
    console.log('Availability Request URL:', url);  
    console.log('Availability Request Params:', params.toString());  

    return this.http.get<AvailabilityResponse>(url, { 
      params: params,
      withCredentials: true 
    });
  }
  // Get user's appointments with pagination
  getMyAppointments(pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedAppointmentsResponse> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PaginatedAppointmentsResponse>(this.baseUrl, { params });
  }

  // Get today's appointments (for doctors)
  getTodayAppointments(): Observable<AppointmentResponse[]> {
    return this.http.get<AppointmentResponse[]>(`${this.baseUrl}/today`);
  }

  // Get appointment by ID
  getAppointmentById(id: string): Observable<AppointmentResponse> {
    return this.http.get<AppointmentResponse>(`${this.baseUrl}/${id}`);
  }

  // Get appointment by reference number
  getAppointmentByReference(referenceNumber: string): Observable<AppointmentResponse> {
    return this.http.get<AppointmentResponse>(`${this.baseUrl}/reference/${referenceNumber}`);
  }

  // Create new appointment
  createAppointment(request: CreateAppointmentRequest): Observable<AppointmentResponse> {
    return this.http.post<AppointmentResponse>(this.baseUrl, request);
  }

  // Update appointment
  updateAppointment(id: string, request: UpdateAppointmentRequest): Observable<AppointmentResponse> {
    return this.http.put<AppointmentResponse>(`${this.baseUrl}/${id}`, request);
  }

  // Cancel appointment
  cancelAppointment(id: string, request: CancelAppointmentRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/cancel`, request);
  }

  // Update appointment status (for doctors)
  updateAppointmentStatus(id: string, request: UpdateAppointmentStatusRequest): Observable<AppointmentResponse> {
    return this.http.put<AppointmentResponse>(`${this.baseUrl}/${id}/status`, request);
  }

  // Check doctor availability
/*   getDoctorAvailability(request: DoctorAvailabilityRequest): Observable<AvailabilityResponse> {
    const params = new HttpParams()
      .set('doctorId', request.doctorId)
      .set('date', request.date);

    return this.http.get<AvailabilityResponse>(`${this.baseUrl}/availability`, { params });
  } */

  // Check if appointment can be cancelled
  canCancelAppointment(id: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/${id}/can-cancel`);
  }
}