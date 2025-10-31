import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, throwError } from 'rxjs';
import { Auth } from '../services/auth/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
     const router = inject(Router);
  const authService = inject(Auth );

  // إعداد الطلب بالهيدرز المطلوبة
  const authReq = req.clone({
    withCredentials: true,
    setHeaders: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    }
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