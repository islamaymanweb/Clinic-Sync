import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, OnDestroy, signal, ElementRef, ViewChild, computed } from '@angular/core';
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
export class Navbar implements OnInit, OnDestroy {
  isScrolled = false;
  isMobileMenuOpen = false;
  isUserMenuOpen = false;
  isVisible = false;
  currentUser: UserInfo | null = null;
  indicatorTransform = 'translateX(0) scaleX(0)';
  
  private subscriptions: Subscription = new Subscription();
  private activeLinkIndex = 0;
  
  @ViewChild('mobileSidebar') mobileSidebar!: ElementRef<HTMLElement>;

  navItems: NavItemsMap = {
    [UserRole.Patient]: [
      {
        label: 'Dashboard',
        path: '/patient/dashboard',
        icon: '🏠',
        roles: [UserRole.Patient]
      },
      {
        label: 'Find Doctors',
        path: '/doctors',
        icon: '👨‍⚕️',
        roles: [UserRole.Patient]
      },
      {
        label: 'My Appointments',
        path: '/patient/appointments',
        icon: '📋',
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
        roles: [UserRole.Doctor]
      },
      {
        label: 'Schedule',
        path: '/doctor/schedule',
        icon: '🗓️',
        roles: [UserRole.Doctor]
      },
      {
        label: 'Patients',
        path: '/doctor/patients',
        icon: '👥',
        roles: [UserRole.Doctor]
      },
      {
        label: 'Profile',
        path: '/doctor/profile',
        icon: '👤',
        roles: [UserRole.Doctor]
      }
    ],
    [UserRole.Admin]: [
      {
        label: 'Dashboard',
        path: '/admin/dashboard',
        icon: '👑',
        roles: [UserRole.Admin]
      },
      {
        label: 'Appointments',
        path: '/admin/appointments',
        icon: '📅',
        roles: [UserRole.Admin]
      },
      {
        label: 'Doctors',
        path: '/admin/doctors',
        icon: '👨‍⚕️',
        roles: [UserRole.Admin]
      },
      {
        label: 'Users',
        path: '/admin/users',
        icon: '👥',
        roles: [UserRole.Admin]
      },
      {
        label: 'Reports',
        path: '/admin/reports',
        icon: '📈',
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
        { label: 'Medical Centers', path: '/medical-centers', icon: '🏥' }
      ]
    },
    {
      label: 'Services',
      path: '/services',
      icon: '💊',
      children: [
        { label: 'Consultation', path: '/services/consultation', icon: '🩺' },
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
      label: 'Contact',
      path: '/contact',
      icon: '📞'
    }
  ];

  constructor(
    private router: Router,
    private authService: Auth,
    private userStateService: UserState
  ) {}

  ngOnInit() {
    setTimeout(() => this.isVisible = true, 100);
    
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
    this.setupNavigationIndicator();
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  private loadCurrentUser() {
    this.currentUser = this.userStateService.getCurrentUser();
    if (!this.currentUser) {
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
    this.isScrolled = window.scrollY > 50;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    this.handleUserDropdownClick(event);
    this.handleSidebarClickOutside(event);
  }

  @HostListener('document:keydown.escape')
  onEscapeKey() {
    if (this.isMobileMenuOpen) {
      this.closeMobileMenu();
    }
    if (this.isUserMenuOpen) {
      this.closeUserMenu();
    }
  }

  private handleUserDropdownClick(event: Event) {
    if (!(event.target as HTMLElement).closest('.user-dropdown')) {
      this.closeUserMenu();
    }
  }

  private handleSidebarClickOutside(event: Event) {
    if (this.isMobileMenuOpen && this.mobileSidebar) {
      const target = event.target as HTMLElement;
      const sidebarElement = this.mobileSidebar.nativeElement;
      const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
      
      if (!sidebarElement.contains(target) && !mobileMenuBtn?.contains(target)) {
        this.closeMobileMenu();
      }
    }
  }

  toggleMobileMenu() {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    if (this.isMobileMenuOpen) {
      document.body.style.overflow = 'hidden';
      this.closeUserMenu();
    } else {
      document.body.style.overflow = '';
    }
  }

  closeMobileMenu() {
    this.isMobileMenuOpen = false;
    document.body.style.overflow = '';
  }

  toggleUserMenu() {
    this.isUserMenuOpen = !this.isUserMenuOpen;
    if (this.isUserMenuOpen) {
      this.closeMobileMenu();
    }
  }

  closeUserMenu() {
    this.isUserMenuOpen = false;
  }

  toggleSubMenu(item: NavItem) {
    item.isExpanded = !item.isExpanded;
  }

  onLinkHover(event: MouseEvent, link: NavItem) {
    const currentItems = this.getCurrentNavItems();
    const index = currentItems.findIndex(l => l.path === link.path);
    this.activeLinkIndex = index;
    this.updateNavigationIndicator();
  }

  setupNavigationIndicator() {
    this.updateNavigationIndicator();
  }

  updateNavigationIndicator() {
    const currentItems = this.getCurrentNavItems();
    const linkWidth = 100 / currentItems.length;
    const transform = `translateX(${this.activeLinkIndex * linkWidth}%) scaleX(${linkWidth / 100})`;
    this.indicatorTransform = transform;
  }

  private updateActiveStates() {
    const currentPath = this.router.url;
    
    this.publicNavItems.forEach(item => {
      item.isActive = currentPath === item.path || 
                     (item.children?.some(child => currentPath === child.path));
      
      if (item.children) {
        item.children.forEach(child => {
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
          item.children.forEach(child => {
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

  hasAccess(item: NavItem): boolean {
    if (!item.roles || !this.currentUser?.role) {
      return true;
    }
    
    const userRole = this.currentUser.role as UserRole;
    return item.roles.includes(userRole);
  }

  login() {
    this.router.navigate(['/login']);
    this.closeUserMenu();
    this.closeMobileMenu();
  }

  register() {
    this.router.navigate(['/register']);
    this.closeUserMenu();
    this.closeMobileMenu();
  }

  logout() {
    this.authService.logout();
    this.closeUserMenu();
    this.closeMobileMenu();
  }

  goToProfile() {
    if (this.currentUser?.role) {
      const userRole = this.currentUser.role.toLowerCase() as string;
      this.router.navigate([`/${userRole}/profile`]);
    }
    this.closeUserMenu();
    this.closeMobileMenu();
  }

  goToDashboard() {
    if (this.currentUser?.role) {
      const userRole = this.currentUser.role.toLowerCase() as string;
      this.router.navigate([`/${userRole}/dashboard`]);
    }
    this.closeUserMenu();
    this.closeMobileMenu();
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
    return this.authService.isAuthenticated();
  }
}