export interface CreateAppointmentRequest {
  doctorId: string;
  appointmentDate: string; // ISO string
  startTime: string; // HH:mm format
  reasonForVisit?: string;
}

export interface UpdateAppointmentRequest {
  reasonForVisit?: string;
  notes?: string;
}

export interface CancelAppointmentRequest {
  cancellationReason: string;
}

export interface UpdateAppointmentStatusRequest {
  status: AppointmentStatus;
  notes?: string;
}

export interface DoctorAvailabilityRequest {
  doctorId: string;
  date: string; // ISO string
}

export interface AppointmentResponse {
  id: string;
  referenceNumber: string;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  status: string;
  reasonForVisit?: string;
  notes?: string;
  createdAt: string;
  cancelledAt?: string;
  cancellationReason?: string;
  doctor: DoctorInfo;
  patient?: PatientInfo;
}

export interface DoctorInfo {
  id: string;
  fullName: string;
  specialty: string;
  consultationFee: number;
  profilePictureUrl?: string;
}

export interface PatientInfo {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
}

export interface TimeSlotResponse {
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

export interface AvailabilityResponse {
  success: boolean;
  message: string;
  timeSlots?: TimeSlotResponse[];
}

export interface PaginatedAppointmentsResponse {
  success: boolean;
  message: string;
  data?: AppointmentResponse[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export enum AppointmentStatus {
  Confirmed = 'Confirmed',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  NoShow = 'NoShow'
}