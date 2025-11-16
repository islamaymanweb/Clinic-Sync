// features/appointments/book-appointment/book-appointment.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DoctorSearchResult } from '../../../shared/models/Specialty';
 
import { Appointment } from '../../../core/services/appointment';
import { Subject, takeUntil } from 'rxjs';
import { Doctor } from '../../../core/services/doctor';
import { AvailabilityResponse, CreateAppointmentRequest, DoctorAvailabilityRequest, TimeSlotResponse } from '../../../shared/models/CreateAppointmentRequest ';
import { Auth } from '../../../core/services/auth/auth';
import { UserState } from '../../../core/services/auth/user-state';

@Component({
  selector: 'app-book-appointment',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './book-appointment.html',
  styleUrl: './book-appointment.scss',
})
export class BookAppointment implements OnInit, OnDestroy {
 appointmentForm: FormGroup;
  doctor: DoctorSearchResult | null = null;
  availability: AvailabilityResponse | null = null;
  loading = false;
  loadingAvailability = false;
  selectedDate: string = '';
  selectedTimeSlot: TimeSlotResponse | null = null;
  isAuthenticated: boolean = false; // ✅ إضافة
  currentUser: any = null; // ✅ إضافة
  
  private destroy$ = new Subject<void>();

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    public router: Router,
    private appointmentService: Appointment,
    private doctorService: Doctor,
    private authService: Auth , // ✅ إضافة
    private userStateService: UserState  // ✅ إضافة
  ) {
    this.appointmentForm = this.createAppointmentForm();
  }

  ngOnInit(): void {
    this.checkAuthentication(); // ✅ التحقق من المصادقة أولاً
    this.loadDoctor();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ✅ التحقق من حالة المصادقة
  private checkAuthentication(): void {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.currentUser = this.userStateService.getCurrentUser();
    
    console.log('🔐 Authentication check:', {
      isAuthenticated: this.isAuthenticated,
      currentUser: this.currentUser
    });

    if (!this.isAuthenticated) {
      console.warn('⚠️ User not authenticated - redirecting to login');
      this.redirectToLogin();
      return;
    }

    if (this.currentUser?.role !== 'Patient') {
      console.warn('⚠️ User is not a patient - redirecting to doctors page');
      this.router.navigate(['/doctors']);
      return;
    }
  }

  // ✅ توجيه لصفحة تسجيل الدخول
  private redirectToLogin(): void {
    const currentUrl = this.router.url;
    this.router.navigate(['/login'], {
      queryParams: { returnUrl: currentUrl }
    });
  }

  private createAppointmentForm(): FormGroup {
    return this.formBuilder.group({
      reasonForVisit: ['', [Validators.maxLength(500)]]
    });
  }

  private loadDoctor(): void {
    // ✅ التحقق من المصادقة أولاً
    if (!this.isAuthenticated) {
      return;
    }

    const doctorId = this.route.snapshot.paramMap.get('id');
    if (!doctorId) {
      this.router.navigate(['/doctors']);
      return;
    }

    this.loading = true;
    this.doctorService.getDoctorById(doctorId).subscribe({
      next: (doctor) => {
        this.doctor = doctor;
        this.loading = false;
        this.onDateSelect(new Date().toISOString().split('T')[0]);
      },
      error: (error) => {
        console.error('Error loading doctor:', error);
        this.loading = false;
        this.router.navigate(['/doctors']);
      }
    });
  }

  onDateSelect(date: string): void {
    if (!this.doctor || !this.isAuthenticated) return;

    this.selectedDate = date;
    this.selectedTimeSlot = null;
    this.loadingAvailability = true;

    const request: DoctorAvailabilityRequest = {
      doctorId: this.doctor.id,
      date: date
    };

    this.appointmentService.getDoctorAvailability(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.availability = response;
          this.loadingAvailability = false;
        },
        error: (error) => {
          console.error('Error loading availability:', error);
          this.loadingAvailability = false;
          this.availability = {
            success: false,
            message: 'Failed to load availability'
          };
        }
      });
  }

  onTimeSlotSelect(slot: TimeSlotResponse): void {
    if (!slot.isAvailable || !this.isAuthenticated) return;
    this.selectedTimeSlot = slot;
  }

  onSubmit(): void {
    // ✅ التحقق المكثف من المصادقة قبل الإرسال
    if (!this.isAuthenticated) {
      console.warn('🚫 User not authenticated - redirecting to login');
      this.redirectToLogin();
      return;
    }

    if (this.appointmentForm.invalid || !this.doctor || !this.selectedTimeSlot || !this.selectedDate) {
      console.warn('🚫 Form validation failed');
      return;
    }

    console.log('✅ Creating appointment for authenticated user:', this.currentUser);

    const request: CreateAppointmentRequest = {
      doctorId: this.doctor.id,
      appointmentDate: this.selectedDate,
      startTime: this.selectedTimeSlot.startTime,
      reasonForVisit: this.appointmentForm.get('reasonForVisit')?.value
    };

    this.loading = true;
    this.appointmentService.createAppointment(request).subscribe({
      next: (appointment) => {
        this.loading = false;
        console.log('✅ Appointment created successfully:', appointment);
        this.router.navigate(['/patient/appointments', appointment.id], {
          queryParams: { success: 'true' }
        });
      },
      error: (error) => {
        console.error('❌ Error creating appointment:', error);
        this.loading = false;
         
        if (error.status === 401) {
          console.warn('🔐 Session expired - redirecting to login');
          this.redirectToLogin();
        } else {
          // عرض رسالة خطأ للمستخدم
          this.showErrorMessage('Failed to create appointment. Please try again.');
        }
      }
    });
  }
 
  private showErrorMessage(message: string): void {
    
    alert(message);  
  }
 
  getDayName(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { weekday: 'short' });
  }

  getDateNumber(dateString: string): string {
    const date = new Date(dateString);
    return date.getDate().toString();
  }

  getMonthName(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'short' });
  }

  formatDisplayDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatTime(time: string): string {
    const [hours, minutes] = time.split(':');
    const hour = parseInt(hours);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    const displayHour = hour % 12 || 12;
    return `${displayHour}:${minutes} ${ampm}`;
  }

  getTimeSlotDuration(slot: TimeSlotResponse): string {
    const start = new Date(`2000-01-01T${slot.startTime}`);
    const end = new Date(`2000-01-01T${slot.endTime}`);
    const duration = (end.getTime() - start.getTime()) / (1000 * 60);
    return `${duration} minutes`;
  }

  getNextAvailableDates(): string[] {
    const dates: string[] = [];
    const today = new Date();
    
    for (let i = 0; i < 14; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() + i);
       
      if (date.getDay() !== 0 && date.getDay() !== 6) {
        dates.push(date.toISOString().split('T')[0]);
      }
    }
    
    return dates;
  }

  isDateInPast(date: string): boolean {
    return new Date(date) < new Date();
  }
}