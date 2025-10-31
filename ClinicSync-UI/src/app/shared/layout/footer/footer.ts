import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
 
interface FooterLink {
  label: string;
  route: string;
  external?: boolean;
}

interface FooterSection {
  title: string;
  links: FooterLink[];
}

interface SocialLink {
  name: string;
  url: string;
  icon: string;
}

interface ContactInfo {
  icon: string;
  text: string;
  link?: string;
}

interface Feature {
  icon: string;
  title: string;
  description: string;
  colorClass: string;
}

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,RouterModule],
 
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer{
  currentYear: number = new Date().getFullYear();

  quickLinks: FooterLink[] = [
    { label: 'Find Doctors', route: '/doctors' },
    { label: 'Book Appointment', route: '/appointments/book' },
    { label: 'My Appointments', route: '/patient/appointments' },
    { label: 'Specialties', route: '/specialties' },
    { label: 'About Us', route: '/about' }
  ];

  professionalLinks: FooterLink[] = [
    { label: 'Doctor Login', route: '/auth/login?role=doctor' },
    { label: 'Admin Portal', route: '/auth/login?role=admin' },
    { label: 'Schedule Management', route: '/doctor/schedule' },
    { label: 'Reports & Analytics', route: '/admin/reports' },
    { label: 'Join Our Team', route: '/careers' }
  ];

  legalLinks: FooterLink[] = [
    { label: 'Privacy Policy', route: '/privacy' },
    { label: 'Terms of Service', route: '/terms' },
    { label: 'Cookie Policy', route: '/cookies' },
    { label: 'Accessibility', route: '/accessibility' }
  ];

  socialLinks: SocialLink[] = [
    { name: 'Facebook', url: 'https://facebook.com', icon: 'facebook' },
    { name: 'Twitter', url: 'https://twitter.com', icon: 'twitter' },
    { name: 'Instagram', url: 'https://instagram.com', icon: 'instagram' },
    { name: 'LinkedIn', url: 'https://linkedin.com', icon: 'linkedin' }
  ];

  contactInfo: ContactInfo[] = [
    { 
      icon: 'map-pin', 
      text: '123 Medical Center Drive\nHealthcare District, City 12345' 
    },
    { 
      icon: 'phone', 
      text: '+1 (555) 123-4567',
      link: 'tel:+15551234567'
    },
    { 
      icon: 'mail', 
      text: 'support@clinicsync.com',
      link: 'mailto:support@clinicsync.com'
    },
    { 
      icon: 'clock', 
      text: '24/7 Support Available' 
    }
  ];

  features: Feature[] = [
    {
      icon: 'shield',
      title: 'Secure & Private',
      description: 'HIPAA Compliant Platform',
      colorClass: 'primary'
    },
    {
      icon: 'heart',
      title: 'Patient Focused',
      description: 'Your Health, Our Priority',
      colorClass: 'secondary'
    },
    {
      icon: 'clock',
      title: 'Always Available',
      description: 'Book Anytime, Anywhere',
      colorClass: 'accent'
    }
  ];

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}