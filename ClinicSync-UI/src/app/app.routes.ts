import { Routes } from '@angular/router';
import { Home } from './shared/layout/home/home';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Doctors } from './features/public/doctors/doctors';

export const routes: Routes = [
      { path: '', component: Home },
{ path: 'login', component: Login },
{ path: 'register', component: Register },
     /*    {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(r => r.authRoutes)
  } */
{
    path: 'doctors',component:Doctors
      
  },
         { path: '**', redirectTo: '' },
];
