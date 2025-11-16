import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth-guard';

export const authRoutes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./login/login').then(c => c.Login ),
    canActivate: [authGuard] // منع الوصول إذا كان المستخدم مسجلاً
  },
  {
    path: 'register',
    loadComponent: () => import('./register/register').then(c => c.Register),
    canActivate: [authGuard]
  },
   
  {
    path: 'forgot-password',
    loadComponent: () => import('./forgot-password/forgot-password').then(c => c.ForgotPassword)
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./reset-password/reset-password').then(c => c.ResetPassword)
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  }
];