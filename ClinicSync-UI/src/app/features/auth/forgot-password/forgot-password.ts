import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Auth } from '../../../core/services/auth/auth';

@Component({
  selector: 'app-forgot-password',
  imports: [],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss',
})
export class ForgotPassword {
  forgotPasswordForm: FormGroup;
  loading = false;
  message = '';
  isSuccess = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: Auth
  ) {
    this.forgotPasswordForm = this.createForgotPasswordForm();
  }

  private createForgotPasswordForm(): FormGroup {
    return this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotPasswordForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    this.message = '';

    const email = this.forgotPasswordForm.get('email')?.value;

    this.authService.forgotPassword(email).subscribe({
      next: (response) => {
        this.loading = false;
        
        if (response.success) {
          this.isSuccess = true;
          this.message = response.message || 'If the email exists, a reset link has been sent.';
          this.forgotPasswordForm.reset();
        } else {
          this.isSuccess = false;
          this.message = response.message || 'Failed to send reset email.';
        }
      },
      error: (error) => {
        this.loading = false;
        this.isSuccess = false;
        this.message = error.message || 'An error occurred. Please try again.';
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.forgotPasswordForm.controls).forEach(key => {
      this.forgotPasswordForm.get(key)?.markAsTouched();
    });
  }

  get email() { return this.forgotPasswordForm.get('email'); }
}