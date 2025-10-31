import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Auth } from '../../../core/services/auth/auth';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginRequest } from '../../../shared/models/auth';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login  implements OnInit {
  loginForm: FormGroup;
  loading = false;
  errorMessage = '';
  returnUrl = '';
  showPassword = false;
  sessionExpired = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: Auth ,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.createLoginForm();
  }

  ngOnInit(): void {
    // الحصول على returnUrl من query parameters
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
    this.sessionExpired = this.route.snapshot.queryParams['sessionExpired'] === 'true';

    // إذا كان المستخدم مسجلاً بالفعل، توجيه للصفحة المناسبة
    if (this.authService.isAuthenticated()) {
      this.redirectToDashboard();
    }

    // عرض رسالة إذا انتهت الجلسة
    if (this.sessionExpired) {
      this.errorMessage = 'Your session has expired. Please login again.';
    }
  }

  private createLoginForm(): FormGroup {
    return this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const loginData: LoginRequest = this.loginForm.value;

    this.authService.login(loginData).subscribe({
      next: (response) => {
        this.loading = false;
        
        if (response.success) {
          this.handleLoginSuccess();
        } else {
          this.errorMessage = response.message || 'Login failed';
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.message || 'An error occurred during login';
      }
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  private handleLoginSuccess(): void {
    // إذا كان هناك returnUrl، نعود إليه
    if (this.returnUrl) {
      this.router.navigateByUrl(this.returnUrl);
    } else {
      this.redirectToDashboard();
    }
  }

  private redirectToDashboard(): void {
    const user = this.authService.getCurrentUserValue();
    if (user) {
      const routes: { [key: string]: string } = {
        'Patient': '/patient/dashboard',
        'Doctor': '/doctor/dashboard',
        'Admin': '/admin/dashboard'
      };
      
      const targetRoute = routes[user.role] || '/auth/login';
      this.router.navigate([targetRoute]);
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      this.loginForm.get(key)?.markAsTouched();
    });
  }

  // Getters for template access
  get email() { return this.loginForm.get('email'); }
  get password() { return this.loginForm.get('password'); }
  get rememberMe() { return this.loginForm.get('rememberMe'); }
}