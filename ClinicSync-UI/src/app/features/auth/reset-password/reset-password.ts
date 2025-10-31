import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Auth } from '../../../core/services/auth/auth';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ResetPasswordRequest } from '../../../shared/models/auth';

@Component({
  selector: 'app-reset-password',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss',
})
export class ResetPassword  implements OnInit {
  resetPasswordForm: FormGroup;
  loading = false;
  message = '';
  isSuccess = false;
  showPassword = false;
  showConfirmPassword = false;
  
  private token = '';
  private email = '';

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private authService: Auth,
    private router: Router
  ) {
    this.resetPasswordForm = this.createResetPasswordForm();
  }

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParams['token'] || '';
    this.email = this.route.snapshot.queryParams['email'] || '';

    if (!this.token || !this.email) {
      this.message = 'Invalid reset password link.';
      this.isSuccess = false;
    }
  }

  private createResetPasswordForm(): FormGroup {
    return this.formBuilder.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  private passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const newPassword = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');

    if (!newPassword || !confirmPassword) {
      return null;
    }

    return newPassword.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  onSubmit(): void {
    if (this.resetPasswordForm.invalid || !this.token || !this.email) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    this.message = '';

    const resetData: ResetPasswordRequest = {
      token: this.token,
      email: this.email,
      newPassword: this.resetPasswordForm.get('newPassword')?.value,
      confirmPassword: this.resetPasswordForm.get('confirmPassword')?.value
    };

    this.authService.resetPassword(resetData).subscribe({
      next: (response) => {
        this.loading = false;
        
        if (response.success) {
          this.isSuccess = true;
          this.message = response.message || 'Password reset successfully!';
          this.resetPasswordForm.reset();
          
          // توجيه لصفحة Login بعد 3 ثواني
          setTimeout(() => {
            this.router.navigate(['/auth/login'], {
              queryParams: { message: 'password_reset_success' }
            });
          }, 3000);
        } else {
          this.isSuccess = false;
          this.message = response.message || 'Password reset failed.';
        }
      },
      error: (error) => {
        this.loading = false;
        this.isSuccess = false;
        this.message = error.message || 'An error occurred. Please try again.';
      }
    });
  }

  togglePasswordVisibility(field: 'newPassword' | 'confirmPassword'): void {
    if (field === 'newPassword') {
      this.showPassword = !this.showPassword;
    } else {
      this.showConfirmPassword = !this.showConfirmPassword;
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.resetPasswordForm.controls).forEach(key => {
      this.resetPasswordForm.get(key)?.markAsTouched();
    });
  }

  get newPassword() { return this.resetPasswordForm.get('newPassword'); }
  get confirmPassword() { return this.resetPasswordForm.get('confirmPassword'); }
}