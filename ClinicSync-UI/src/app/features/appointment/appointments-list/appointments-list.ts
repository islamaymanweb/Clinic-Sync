import { Component, OnInit } from '@angular/core';
import { AppointmentResponse, PaginatedAppointmentsResponse } from '../../../shared/models/CreateAppointmentRequest ';
import { Appointment } from '../../../core/services/appointment';
import { UserState } from '../../../core/services/auth/user-state';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-appointments-list',
  imports: [CommonModule, RouterModule],
  templateUrl: './appointments-list.html',
  styleUrl: './appointments-list.scss',
})
export class AppointmentsList implements OnInit {
  appointments: AppointmentResponse[] = [];
  loading = false;
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  totalCount = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  currentUserRole: string = '';

  constructor(
    private appointmentService: Appointment ,
    private userStateService: UserState ,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUserRole = this.userStateService.getCurrentUser()?.role || '';
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.loading = true;
    this.appointmentService.getMyAppointments(this.currentPage, this.pageSize).subscribe({
      next: (response: PaginatedAppointmentsResponse) => {
        this.loading = false;
        if (response.success && response.data) {
          this.appointments = response.data;
          this.totalPages = response.totalPages;
          this.totalCount = response.totalCount;
          this.hasPreviousPage = response.hasPrevious;
          this.hasNextPage = response.hasNext;
        }
      },
      error: (error) => {
        this.loading = false;
        console.error('Error loading appointments:', error);
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadAppointments();
  }

  getStatusBadgeClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'status-confirmed';
      case 'completed':
        return 'status-completed';
      case 'cancelled':
        return 'status-cancelled';
      case 'noshow':
        return 'status-noshow';
      default:
        return 'status-pending';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatTime(timeString: string): string {
    const [hours, minutes] = timeString.split(':');
    const hour = parseInt(hours);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    const displayHour = hour % 12 || 12;
    return `${displayHour}:${minutes} ${ampm}`;
  }

  isPatient(): boolean {
    return this.currentUserRole === 'Patient';
  }

  isDoctor(): boolean {
    return this.currentUserRole === 'Doctor';
  }

  viewAppointmentDetails(appointment: AppointmentResponse): void {
    this.router.navigate([`/${this.currentUserRole.toLowerCase()}/appointments`, appointment.id]);
  }
}