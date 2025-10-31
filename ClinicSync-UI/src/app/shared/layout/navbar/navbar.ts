import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, OnDestroy } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { UserRole } from '../../models/enums';
import { UserInfo } from '../../models/user';
import { Auth } from '../../../core/services/auth/auth';
import { UserState } from '../../../core/services/auth/user-state';
 

interface NavItem {
  label: string;
  path: string;
  icon: string;
  roles?: UserRole[];
  children?: NavItem[];
  isActive?: boolean;
  isExpanded?: boolean;
}

type NavItemsMap = {
  [key in UserRole]: NavItem[];
};

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.scss']
})
export class Navbar  implements OnInit, OnDestroy { // ✅ تصحيح اسم الكلاس
  isScrolled = false;
  isMobileMenuOpen = false;
  isUserMenuOpen = false;
  currentUser: UserInfo | null = null;
  private subscriptions: Subscription = new Subscription();

  navItems: NavItemsMap = {
    [UserRole.Patient]: [
      {
        label: 'Home',
        path: '/patient/dashboard',
        icon: '🏠',
        roles: [UserRole.Patient]
      },
      {
        label: 'Doctors',
        path: '/patient/doctors',
        icon: '👨‍⚕️',
        roles: [UserRole.Patient],
        children: [
          { label: 'Search Doctors', path: '/patient/doctors', icon: '🔍', roles: [UserRole.Patient] },
          { label: 'Specialties', path: '/patient/specialties', icon: '🎯', roles: [UserRole.Patient] },
          { label: 'Available Slots', path: '/patient/available-slots', icon: '📅', roles: [UserRole.Patient] }
        ]
      },
      {
        label: 'My Appointments',
        path: '/patient/appointments',
        icon: '📋',
        roles: [UserRole.Patient]
      },
      {
        label: 'Medical Records',
        path: '/patient/medical-records',
        icon: '📁',
        roles: [UserRole.Patient]
      },
      {
        label: 'Profile',
        path: '/patient/profile',
        icon: '👤',
        roles: [UserRole.Patient]
      }
    ],
    [UserRole.Doctor]: [
      {
        label: 'Dashboard',
        path: '/doctor/dashboard',
        icon: '📊',
        roles: [UserRole.Doctor]
      },
      {
        label: 'Appointments',
        path: '/doctor/appointments',
        icon: '📅',
        roles: [UserRole.Doctor],
        children: [
          { label: 'Schedule', path: '/doctor/schedule', icon: '🗓️', roles: [UserRole.Doctor] },
          { label: 'Upcoming Appointments', path: '/doctor/upcoming', icon: '⏰', roles: [UserRole.Doctor] },
          { label: 'Past Appointments', path: '/doctor/history', icon: '📋', roles: [UserRole.Doctor] }
        ]
      },
      {
        label: 'Patients',
        path: '/doctor/patients',
        icon: '👥',
        roles: [UserRole.Doctor]
      },
      {
        label: 'Medical Records',
        path: '/doctor/medical-records',
        icon: '🏥',
        roles: [UserRole.Doctor]
      },
      {
        label: 'Clinic Settings',
        path: '/doctor/clinic-settings',
        icon: '⚙️',
        roles: [UserRole.Doctor]
      }
    ],
    [UserRole.Admin]: [
      {
        label: 'Admin Panel',
        path: '/admin/dashboard',
        icon: '👑',
        roles: [UserRole.Admin]
      },
      {
        label: 'User Management',
        path: '/admin/users',
        icon: '👥',
        roles: [UserRole.Admin],
        children: [
          { label: 'All Users', path: '/admin/users', icon: '👨‍👩‍👧‍👦', roles: [UserRole.Admin] },
          { label: 'Add Doctor', path: '/admin/doctors/create', icon: '➕', roles: [UserRole.Admin] },
          { label: 'Join Requests', path: '/admin/join-requests', icon: '📥', roles: [UserRole.Admin] }
        ]
      },
      {
        label: 'Clinics Management',
        path: '/admin/clinics',
        icon: '🏥',
        roles: [UserRole.Admin]
      },
      {
        label: 'Reports',
        path: '/admin/reports',
        icon: '📈',
        roles: [UserRole.Admin],
        children: [
          { label: 'Financial Reports', path: '/admin/financial-reports', icon: '💰', roles: [UserRole.Admin] },
          { label: 'Usage Reports', path: '/admin/usage-reports', icon: '📊', roles: [UserRole.Admin] },
          { label: 'Doctor Statistics', path: '/admin/doctor-stats', icon: '👨‍⚕️', roles: [UserRole.Admin] }
        ]
      },
      {
        label: 'Settings',
        path: '/admin/settings',
        icon: '⚙️',
        roles: [UserRole.Admin]
      }
    ]
  };

  publicNavItems: NavItem[] = [
    {
      label: 'Home',
      path: '/',
      icon: '🏠'
    },
    {
      label: 'Doctors',
      path: '/doctors',
      icon: '👨‍⚕️',
      children: [
        { label: 'All Doctors', path: '/doctors', icon: '🔍' },
        { label: 'Specialties', path: '/specialties', icon: '🎯' },
        { label: 'Advanced Centers', path: '/medical-centers', icon: '🏥' }
      ]
    },
    {
      label: 'Services',
      path: '/services',
      icon: '💊',
      children: [
        { label: 'Medical Consultation', path: '/services/consultation', icon: '🩺' },
        { label: 'Examinations', path: '/services/examinations', icon: '🔬' },
        { label: 'Laboratory', path: '/services/lab', icon: '🧪' }
      ]
    },
    {
      label: 'About',
      path: '/about',
      icon: 'ℹ️'
    },
    {
      label: 'Contact Us',
      path: '/contact',
      icon: '📞'
    }
  ];

  constructor(
    private router: Router,
    private authService: Auth ,  
    private userStateService: UserState  
  ) {}

  ngOnInit() {
    this.subscriptions.add(
      this.router.events
        .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
        .subscribe(() => {
          this.updateActiveStates();
        })
    );

    this.subscriptions.add(
      this.userStateService.currentUser$.subscribe(user => {
        this.currentUser = user;
        this.updateActiveStates();
      })
    );

    this.loadCurrentUser();
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  private loadCurrentUser() {
    this.currentUser = this.userStateService.getCurrentUser();
    if (!this.currentUser) {
      // ✅ تحديث طريقة جلب المستخدم الحالي
      this.authService.getCurrentUser().subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.currentUser = response.data;
          }
        },
        error: (error) => {
          console.error('Error loading current user:', error);
        }
      });
    }
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    this.isScrolled = window.scrollY > 20;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    
    if (!target.closest('.user-menu') && !target.closest('.user-menu-btn')) {
      this.isUserMenuOpen = false;
    }
    
    if (!target.closest('.mobile-menu') && !target.closest('.mobile-menu-btn')) {
      this.isMobileMenuOpen = false;
    }
  }

  toggleMobileMenu() {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    if (this.isMobileMenuOpen) {
      this.isUserMenuOpen = false;
    }
  }

  toggleUserMenu() {
    this.isUserMenuOpen = !this.isUserMenuOpen;
    if (this.isUserMenuOpen) {
      this.isMobileMenuOpen = false;
    }
  }

  toggleSubMenu(item: NavItem) {
    item.isExpanded = !item.isExpanded;
  }

  closeMobileMenu() {
    this.isMobileMenuOpen = false;
  }

  closeUserMenu() {
    this.isUserMenuOpen = false;
  }

  private updateActiveStates() {
    const currentPath = this.router.url;
    
    this.publicNavItems.forEach(item => {
      item.isActive = currentPath === item.path || 
                     (item.children?.some(child => currentPath === child.path));
      
      if (item.children) {
        item.children.forEach((child: NavItem) => {
          child.isActive = currentPath === child.path;
        });
      }
    });

    if (this.currentUser?.role) {
      const userRole = this.currentUser.role as UserRole;
      const roleItems = this.navItems[userRole];
      
      roleItems?.forEach(item => {
        item.isActive = currentPath === item.path || 
                       (item.children?.some(child => currentPath === child.path));
        
        if (item.children) {
          item.children.forEach((child: NavItem) => {
            child.isActive = currentPath === child.path;
          });
        }
      });
    }
  }

  getCurrentNavItems(): NavItem[] {
    if (!this.currentUser?.role) {
      return this.publicNavItems;
    }
    
    const userRole = this.currentUser.role as UserRole;
    return this.navItems[userRole] || this.publicNavItems;
  }

  navigateTo(path: string) {
    this.router.navigate([path]);
    this.closeMobileMenu();
    this.closeUserMenu();
  }

  hasAccess(item: NavItem): boolean {
    if (!item.roles || !this.currentUser?.role) {
      return true;
    }
    
    const userRole = this.currentUser.role as UserRole;
    return item.roles.includes(userRole);
  }

  login() {
    this.router.navigate(['/auth/login']);
    this.closeUserMenu();
  }

  register() {
    this.router.navigate(['/auth/register']);
    this.closeUserMenu();
  }

  logout() {
    this.authService.logout(); // ✅ التعديل لاستخدام الطريقة الجديدة
    this.closeUserMenu();
  }

  goToProfile() {
    if (this.currentUser?.role) {
      const userRole = this.currentUser.role.toLowerCase() as string;
      this.router.navigate([`/${userRole}/profile`]);
    }
    this.closeUserMenu();
  }

  goToDashboard() {
    if (this.currentUser?.role) {
      const userRole = this.currentUser.role.toLowerCase() as string;
      this.router.navigate([`/${userRole}/dashboard`]);
    }
    this.closeUserMenu();
  }

  getDisplayName(): string {
    if (!this.currentUser) return '';
    return this.currentUser.fullName || this.currentUser.email.split('@')[0];
  }

  getDisplayRole(): string {
    if (!this.currentUser) return 'Guest';
    
    const roles: Record<UserRole, string> = {
      [UserRole.Patient]: 'Patient',
      [UserRole.Doctor]: 'Doctor',
      [UserRole.Admin]: 'System Admin'
    };
    
    return roles[this.currentUser.role as UserRole] || 'User';
  }

  isPatient(): boolean {
    return this.currentUser?.role === UserRole.Patient;
  }

  isDoctor(): boolean {
    return this.currentUser?.role === UserRole.Doctor;
  }

  isAdmin(): boolean {
    return this.currentUser?.role === UserRole.Admin;
  }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated(); // ✅ استخدام الطريقة المحدثة
  }

  getNavItemsByRole(role: string): NavItem[] {
    const userRole = role as UserRole;
    if (userRole in this.navItems) {
      return this.navItems[userRole];
    }
    return this.publicNavItems;
  }

  // ✅ دالة جديدة للتحقق من حالة التحميل
  isLoading(): boolean {
    return this.authService.isAuthenticated() && !this.currentUser;
  }
}