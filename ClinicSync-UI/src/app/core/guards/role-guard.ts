import { ActivatedRouteSnapshot, CanActivateFn, Router, UrlTree } from '@angular/router';
import { UserState } from '../services/auth/user-state';
import { inject } from '@angular/core';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state): boolean | UrlTree => {
  const userStateService = inject(UserState );
  const router = inject(Router);

  const requiredRoles = route.data['roles'] as string[];

  // 🧩 لو مفيش Roles محددة → السماح بالوصول
  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  // ✅ تحقق من صلاحيات المستخدم
  const hasRequiredRole = userStateService.hasAnyRole(requiredRoles);

  if (hasRequiredRole) {
    return true;
  }

  // 🚫 توجيه المستخدم لصفحة "Access Denied"
  return router.createUrlTree(['/auth/access-denied']);
};
