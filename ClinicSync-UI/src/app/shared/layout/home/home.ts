import { CommonModule } from '@angular/common';
import {  Component, computed,  OnInit, signal } from '@angular/core';
 
import { RouterLink } from '@angular/router';
import { interval, map, Subject, takeUntil, takeWhile } from 'rxjs';
 
 
interface User {
  id: string;
  name: string;
  role: 'patient' | 'doctor' | 'admin' | 'anonymous';
  email: string;
  avatar?: string;
}

interface QuickStats {
  totalAppointments: number;
  availableDoctors: number;
  todayAppointments: number;
  patientSatisfaction: number;
}

interface Appointment {
  id: string;
  doctorName: string;
  specialty: string;
  date: Date;
  time: string;
  status: 'pending' | 'confirmed' | 'completed';
  doctorAvatar?: string;
}

interface Doctor {
  id: string;
  name: string;
  specialty: string;
  rating: number;
  experience: number;
  avatar?: string;
  available: boolean;
}

interface Alert {
  id: string;
  type: 'info' | 'warning' | 'error' | 'success';
  message: string;
  timestamp: Date;
}

interface Announcement {
  id: string;
  title: string;
  content: string;
  date: Date;
  priority: 'low' | 'medium' | 'high';
}

interface HomePageData {
  user: User;
  quickStats: QuickStats;
  upcomingAppointments: Appointment[];
  featuredDoctors: Doctor[];
  systemAlerts: Alert[];
  announcements: Announcement[];
}
@Component({
  selector: 'app-home',
  imports: [CommonModule,RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.scss',
 
})
export class Home  implements OnInit {
  // Signals for reactive state management
  homeData = signal<HomePageData | null>(null);
  isLoading = signal<boolean>(true);
  currentTime = signal<Date>(new Date());
  error = signal<string | null>(null);

  // Computed signals
  user = computed(() => this.homeData()?.user || this.getAnonymousUser());
  quickStats = computed(() => this.homeData()?.quickStats);
  upcomingAppointments = computed(() => this.homeData()?.upcomingAppointments || []);
  featuredDoctors = computed(() => this.homeData()?.featuredDoctors || []);
  systemAlerts = computed(() => this.homeData()?.systemAlerts || []);
  announcements = computed(() => this.homeData()?.announcements || []);
  
  // Role-based computed properties
  isPatient = computed(() => this.user().role === 'patient');
  isDoctor = computed(() => this.user().role === 'doctor');
  isAdmin = computed(() => this.user().role === 'admin');
  isAnonymous = computed(() => this.user().role === 'anonymous');
  
  // UI state
  greeting = computed(() => this.getGreeting());

  ngOnInit(): void {
    this.initializeComponent();
    this.startClock();
  }

  private initializeComponent(): void {
    // Simulate API call with realistic delay
    setTimeout(() => {
      this.loadHomeData();
    }, 1000);
  }

  private startClock(): void {
    interval(1000)
      .pipe(
        takeWhile(() => true),
        map(() => new Date())
      )
      .subscribe(time => this.currentTime.set(time));
  }

  private loadHomeData(): void {
    try {
      // Simulate fetching data from service
      const mockData = this.getMockHomeData();
      this.homeData.set(mockData);
      this.isLoading.set(false);
    } catch (err) {
      this.error.set('Failed to load dashboard data. Please try again.');
      this.isLoading.set(false);
    }
  }

  private getGreeting(): string {
    const hour = this.currentTime().getHours();
    const name = this.user().name;
    
    let timeGreeting = 'Good evening';
    if (hour < 12) timeGreeting = 'Good morning';
    else if (hour < 18) timeGreeting = 'Good afternoon';
    
    return this.isAnonymous() 
      ? `${timeGreeting}! Welcome to Clinic Sync` 
      : `${timeGreeting}, ${name}`;
  }

  private getAnonymousUser(): User {
    return {
      id: 'anonymous',
      name: 'Guest',
      role: 'anonymous',
      email: ''
    };
  }

  private getMockHomeData(): HomePageData {
    // Determine user role (in real app, this comes from auth service)
    const userRole: 'patient' | 'doctor' | 'admin' = 'patient';
    
    return {
      user: {
        id: '1',
        name: 'Sarah Johnson',
        role: userRole,
        email: 'sarah.johnson@email.com',
        avatar: undefined
      },
      quickStats: {
        totalAppointments: 156,
        availableDoctors: 24,
        todayAppointments: 8,
        patientSatisfaction: 4.8
      },
      upcomingAppointments: [
        {
          id: '1',
          doctorName: 'Dr. Michael Chen',
          specialty: 'Cardiology',
          date: new Date(Date.now() + 86400000),
          time: '10:00 AM',
          status: 'confirmed'
        },
        {
          id: '2',
          doctorName: 'Dr. Emily Rodriguez',
          specialty: 'General Medicine',
          date: new Date(Date.now() + 172800000),
          time: '2:30 PM',
          status: 'pending'
        },
        {
          id: '3',
          doctorName: 'Dr. James Wilson',
          specialty: 'Orthopedics',
          date: new Date(Date.now() + 259200000),
          time: '11:15 AM',
          status: 'confirmed'
        }
      ],
      featuredDoctors: [
        {
          id: '1',
          name: 'Dr. Michael Chen',
          specialty: 'Cardiology',
          rating: 4.9,
          experience: 15,
          available: true
        },
        {
          id: '2',
          name: 'Dr. Emily Rodriguez',
          specialty: 'General Medicine',
          rating: 4.8,
          experience: 12,
          available: true
        },
        {
          id: '3',
          name: 'Dr. James Wilson',
          specialty: 'Orthopedics',
          rating: 4.7,
          experience: 10,
          available: false
        },
        {
          id: '4',
          name: 'Dr. Sarah Thompson',
          specialty: 'Pediatrics',
          rating: 4.9,
          experience: 18,
          available: true
        }
      ],
      systemAlerts: [
        {
          id: '1',
          type: 'info',
          message: 'System maintenance scheduled for tomorrow at 2:00 AM',
          timestamp: new Date()
        }
      ],
      announcements: [
        {
          id: '1',
          title: 'New Telemedicine Services Available',
          content: 'Book virtual consultations with our doctors from the comfort of your home.',
          date: new Date(),
          priority: 'high'
        },
        {
          id: '2',
          title: 'Extended Hours This Week',
          content: 'We will be open until 8 PM all week to serve you better.',
          date: new Date(Date.now() - 86400000),
          priority: 'medium'
        }
      ]
    };
  }

  // Event handlers
  onBookAppointment(): void {
    console.log('Navigate to book appointment');
    // In real app: this.router.navigate(['/appointments/book']);
  }

  onSearchDoctors(): void {
    console.log('Navigate to doctor search');
    // In real app: this.router.navigate(['/doctors']);
  }

  onViewMedicalHistory(): void {
    console.log('Navigate to medical history');
    // In real app: this.router.navigate(['/medical-history']);
  }

  onViewSchedule(): void {
    console.log('Navigate to schedule');
    // In real app: this.router.navigate(['/schedule']);
  }

  onViewStats(): void {
    console.log('Navigate to statistics');
    // In real app: this.router.navigate(['/analytics']);
  }

  onDismissAlert(alertId: string): void {
    const currentData = this.homeData();
    if (currentData) {
      const updatedAlerts = currentData.systemAlerts.filter(a => a.id !== alertId);
      this.homeData.set({
        ...currentData,
        systemAlerts: updatedAlerts
      });
    }
  }

  onRetry(): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.initializeComponent();
  }

  // Utility methods
  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    }).format(date);
  }

  getStatusClass(status: string): string {
    const statusMap: Record<string, string> = {
      'confirmed': 'status-confirmed',
      'pending': 'status-pending',
      'completed': 'status-completed',
      'available': 'status-available'
    };
    return statusMap[status] || '';
  }

  getAlertIcon(type: string): string {
    const iconMap: Record<string, string> = {
      'info': '📋',
      'warning': '⚠️',
      'error': '❌',
      'success': '✅'
    };
    return iconMap[type] || '📋';
  }

  getRatingStars(rating: number): string {
    return '⭐'.repeat(Math.floor(rating));
  }

  trackByAppointment(index: number, appointment: Appointment): string {
    return appointment.id;
  }

  trackByDoctor(index: number, doctor: Doctor): string {
    return doctor.id;
  }

  trackByAlert(index: number, alert: Alert): string {
    return alert.id;
  }

  trackByAnnouncement(index: number, announcement: Announcement): string {
    return announcement.id;
  }
}