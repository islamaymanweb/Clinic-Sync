import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, Observable, tap } from 'rxjs';
import { ApiResponse, AuthResponse } from '../../../shared/models/api';
import { LoginRequest, RegisterRequest, ResetPasswordRequest, VerifyEmailRequest } from '../../../shared/models/auth';
import { UserInfo } from '../../../shared/models/user';
import { UserState } from './user-state';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private apiService: Api ,
    private userStateService: UserState ,
    private router: Router
  ) {
    this.checkAuthenticationStatus();
  }

  /**
   * تسجيل الدخول - محدث للتعامل مع الكوكيز
   */
  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.login(credentials).pipe(
      tap(response => {
        if (response.success && response.data?.user) {
          this.userStateService.setCurrentUser(response.data.user);
          this.isAuthenticatedSubject.next(true);
          
          // التوجيه التلقائي حسب الدور
          this.redirectBasedOnRole(response.data.user.role);
        }
      }),
      catchError(error => {
        this.handleAuthError(error);
        throw error;
      })
    );
  }

  /**
   * تسجيل مستخدم جديد
   */
  register(userData: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.register(userData).pipe(
      tap(response => {
        if (response.success) {
          // لا نقوم بتسجيل الدخول تلقائياً بعد التسجيل
          // يبقى المستخدم ينتظر التحقق من البريد
        }
      }),
      catchError(error => {
        this.handleAuthError(error);
        throw error;
      })
    );
  }

  /**
   * تسجيل الخروج - محدث لمسح الكوكيز
   */
  logout(): void {
    this.apiService.logout().subscribe({
      next: () => {
        this.clearAuthData();
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        console.error('Logout error:', error);
        // حتى في حالة الخطأ، نمسح البيانات المحلية
        this.clearAuthData();
        this.router.navigate(['/auth/login']);
      }
    });
  }

  /**
   * جلب بيانات المستخدم الحالي - محدث للتحقق من الكوكيز
   */
  getCurrentUser(): Observable<ApiResponse<UserInfo>> {
    return this.apiService.getCurrentUser().pipe(
      tap(response => {
        if (response.success && response.data) {
          this.userStateService.setCurrentUser(response.data);
          this.isAuthenticatedSubject.next(true);
        } else {
          this.clearAuthData();
        }
      }),
      catchError(error => {
        this.clearAuthData();
        throw error;
      })
    );
  }

  /**
   * تحقق من البريد الإلكتروني
   */
  verifyEmail(data: VerifyEmailRequest): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.verifyEmail(data).pipe(
      tap(response => {
        if (response.success) {
          // بعد التحقق الناجح، يمكن توجيه المستخدم لصفحة Login
          setTimeout(() => {
            this.router.navigate(['/auth/login'], {
              queryParams: { message: 'email_verified' }
            });
          }, 3000);
        }
      }),
      catchError(error => {
        this.handleAuthError(error);
        throw error;
      })
    );
  }

  /**
   * طلب إعادة تعيين كلمة المرور
   */
  forgotPassword(email: string): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.forgotPassword(email).pipe(
      catchError(error => {
        this.handleAuthError(error);
        throw error;
      })
    );
  }

  /**
   * تعيين كلمة مرور جديدة
   */
  resetPassword(data: ResetPasswordRequest): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.resetPassword(data).pipe(
      tap(response => {
        if (response.success) {
          // توجيه لصفحة Login بعد النجاح
          setTimeout(() => {
            this.router.navigate(['/auth/login'], {
              queryParams: { message: 'password_reset_success' }
            });
          }, 2000);
        }
      }),
      catchError(error => {
        this.handleAuthError(error);
        throw error;
      })
    );
  }

  /**
   * التحقق من حالة المصادقة - محدث للكوكيز
   */
  private checkAuthenticationStatus(): void {
    this.getCurrentUser().subscribe({
      next: (response) => {
        if (!response.success || !response.data) {
          this.clearAuthData();
        }
      },
      error: () => this.clearAuthData()
    });
  }

  /**
   * التوجيه حسب الدور
   */
  private redirectBasedOnRole(role: string): void {
    const routes: { [key: string]: string } = {
      'Patient': '/patient/dashboard',
      'Doctor': '/doctor/dashboard', 
      'Admin': '/admin/dashboard'
    };
    
    const targetRoute = routes[role] || '/auth/login';
    this.router.navigate([targetRoute]);
  }

  /**
   * مسح بيانات المصادقة - محدث
   */
  private clearAuthData(): void {
    this.userStateService.clearCurrentUser();
    this.isAuthenticatedSubject.next(false);
    
    // لا نحتاج لمسح الكوكيز يدوياً لأن الـ Backend يتولى ذلك
  }

  /**
   * معالجة أخطاء المصادقة - محدث
   */
  private handleAuthError(error: any): void {
    console.error('Auth error:', error);
    
    if (error.status === 401) {
      this.clearAuthData();
      this.router.navigate(['/auth/login'], {
        queryParams: { sessionExpired: 'true' }
      });
    }
  }

  /**
   * التحقق من صلاحية المستخدم
   */
  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  /**
   * الحصول على المستخدم الحالي
   */
  getCurrentUserValue(): UserInfo | null {
    return this.userStateService.getCurrentUser();
  }

  /**
   * التحقق من دور المستخدم
   */
  hasRole(role: string): boolean {
    const user = this.getCurrentUserValue();
    return user?.role === role;
  }
}