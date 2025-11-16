import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, throwError } from 'rxjs';
import { Auth } from '../services/auth/auth';
import { CookieService } from 'ngx-cookie-service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(Auth);
  const cookieService = inject(CookieService);

  // ✅ محاولة قراءة JWT token من الـ cookie
  const token = cookieService.get('ClinicSync.Auth');
  
  // ✅ Logging للتصحيح
  if (req.url.includes('/api/appointments') || req.url.includes('/api/auth/me')) {
    console.log('🔐 Auth Interceptor:', {
      url: req.url,
      hasToken: !!token,
      tokenLength: token?.length || 0,
      tokenPreview: token ? token.substring(0, 20) + '...' : 'none'
    });
  }
  
  // إعداد الطلب بالهيدرز المطلوبة
  const headers: { [key: string]: string } = {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  };

  // ✅ إضافة JWT token في Authorization header إذا كان موجوداً
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const authReq = req.clone({
    withCredentials: true,
    setHeaders: headers
  });

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Session expired → توجيه المستخدم لتسجيل الدخول
        authService.logout();
        router.navigate(['/auth/login'], {
          queryParams: {
            returnUrl: router.routerState.snapshot.url,
            sessionExpired: 'true'
          }
        });
        return throwError(() => new Error('Session expired. Please login again.'));
      }

      if (error.status === 403) {
        router.navigate(['/auth/access-denied']);
      }

      if (error.status >= 500) {
        console.error('Server error:', error);
      }

      return throwError(() => error);
    })
  );
};