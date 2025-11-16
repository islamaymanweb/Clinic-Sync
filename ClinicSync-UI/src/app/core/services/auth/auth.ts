import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, Observable, tap } from 'rxjs';
import { ApiResponse, AuthResponse } from '../../../shared/models/api';
import { LoginRequest, RegisterRequest, ResetPasswordRequest } from '../../../shared/models/auth';
import { UserInfo } from '../../../shared/models/user';
import { UserState } from './user-state';

@Injectable({
  providedIn: 'root'
})
 export class Auth {
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private apiService: Api,
    private userStateService: UserState,
    private router: Router
  ) {
    this.checkAuthenticationStatus();
  }

  /**
   * تسجيل الدخول - محدث بدون التحقق من البريد
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
   * تسجيل مستخدم جديد - محدث بدون إرسال بريد تحقق
   */
  register(userData: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.register(userData).pipe(
      tap(response => {
        if (response.success) {
          // ✅ تسجيل الدخول تلقائياً بعد التسجيل الناجح
          const loginCredentials: LoginRequest = {
            email: userData.email,
            password: userData.password,
            rememberMe: false
          };
          
          // محاولة تسجيل الدخول تلقائياً
          this.login(loginCredentials).subscribe({
            next: () => {
              // تم التسجيل والدخول تلقائياً
            },
            error: () => {
              // في حالة فشل التسجيل التلقائي، توجيه لصفحة Login
              this.router.navigate(['/auth/login'], {
                queryParams: { email: userData.email, registered: 'true' }
              });
            }
          });
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
        }
        // ✅ لا نمسح البيانات هنا - نتركها للمستدعي
      }),
      catchError(error => {
        // ✅ لا نمسح البيانات هنا - نتركها للمستدعي
        // فقط نرمي الخطأ للمستدعي ليقرر ما يجب فعله
        throw error;
      })
    );
  }

  /**
   * طلب إعادة تعيين كلمة المرور
   */
  forgotPassword(email: string): Observable<ApiResponse<AuthResponse>> {
    return this.apiService.forgotPassword(email).pipe(
      tap(response => {
        if (response.success) {
          // عرض رسالة نجاح
          console.log('Password reset email sent successfully');
        }
      }),
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
          // توجيه لصفحة Login بعد النجاح مع رسالة
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
   * ✅ لا يقوم بتسجيل الخروج تلقائياً عند refresh
   */
  private checkAuthenticationStatus(): void {
    // ✅ التحقق من وجود user في localStorage أولاً
    const currentUser = this.userStateService.getCurrentUser();
    if (currentUser) {
      // ✅ إذا كان هناك user محفوظ، نحدّث حالة المصادقة مباشرة
      this.isAuthenticatedSubject.next(true);
      console.log('✅ User found in storage, authentication restored:', currentUser.email);
      
      // ✅ محاولة تحديث البيانات من API في الخلفية (بدون إجبار)
      this.getCurrentUser().subscribe({
        next: (response) => {
          if (response.success && response.data) {
            // ✅ تحديث بيانات المستخدم إذا نجح الطلب
            this.userStateService.setCurrentUser(response.data);
            this.isAuthenticatedSubject.next(true);
            console.log('✅ User data refreshed from API');
          }
        },
        error: (error) => {
          // ✅ لا نقوم بأي شيء عند خطأ - المستخدم مسجل بالفعل
          console.warn('⚠️ Failed to refresh user data (user still authenticated):', error);
        }
      });
    } else {
      // ✅ فقط إذا لم يكن هناك user محفوظ، نحاول التحقق من API
      console.log('🔍 No user in storage, checking API...');
      this.getCurrentUser().subscribe({
        next: (response) => {
          if (response.success && response.data) {
            // ✅ حفظ بيانات المستخدم
            this.userStateService.setCurrentUser(response.data);
            this.isAuthenticatedSubject.next(true);
            console.log('✅ User authenticated from API');
          } else {
            // ✅ لا يوجد user - نمسح البيانات
            this.clearAuthData();
            console.log('❌ No user found in API');
          }
        },
        error: (error) => {
          // ✅ لا نقوم بتسجيل الخروج - فقط نترك الحالة كما هي
          console.warn('⚠️ Failed to verify authentication (no user in storage):', error);
          this.clearAuthData();
        }
      });
    }
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

  /**
   * التحقق من أي من الأدوار المطلوبة
   */
  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUserValue();
    return user ? roles.includes(user.role) : false;
  }
}