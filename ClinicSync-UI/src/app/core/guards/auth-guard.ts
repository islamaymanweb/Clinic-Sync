import { CanActivateFn, Router } from '@angular/router';
import { Auth } from '../services/auth/auth';
import { inject } from '@angular/core';
import { map, take } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
 const authService = inject(Auth );
  const router = inject(Router);

  return authService.isAuthenticated$.pipe(
    take(1),
    map(isAuthenticated => {
      if (isAuthenticated) {
        // تحقق من الأدوار المطلوبة
        const requiredRoles = route.data?.['roles'] as string[] | undefined;
        if (requiredRoles && requiredRoles.length > 0) {
          const user = authService.getCurrentUserValue();
          if (user && requiredRoles.includes(user.role)) {
            return true;
          } else {
            return router.createUrlTree(['/auth/access-denied']);
          }
        }
        return true;
      }

      // المستخدم غير مسجل → توجهه لصفحة الدخول
      return router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url }
      });
    })
  );
};