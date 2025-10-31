import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Auth } from '../../../core/services/auth/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-verify-email',
  imports: [CommonModule],
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.scss',
})
export class VerifyEmail implements OnInit {
  loading = false;
  status: 'verifying' | 'success' | 'error' | 'initial' = 'initial';
  message = '';
  email = '';
  token = '';
  countdown = 5;

  constructor(
    private route: ActivatedRoute,
    private authService: Auth ,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParams['token'] || '';
    this.email = this.route.snapshot.queryParams['email'] || '';

    if (this.token && this.email) {
      this.verifyEmail();
    } else {
      this.status = 'initial';
      this.message = 'Please check your email for the verification link.';
    }
  }

  verifyEmail(): void {
    this.loading = true;
    this.status = 'verifying';
    
    this.authService.verifyEmail({ token: this.token, email: this.email }).subscribe({
      next: (response) => {
        this.loading = false;
        
        if (response.success) {
          this.status = 'success';
          this.message = response.message || 'Email verified successfully!';
          this.startCountdown();
        } else {
          this.status = 'error';
          this.message = response.message || 'Email verification failed.';
        }
      },
      error: (error) => {
        this.loading = false;
        this.status = 'error';
        this.message = error.message || 'An error occurred during verification.';
      }
    });
  }

  resendVerification(): void {
    // Implementation for resend verification email
    this.message = 'Verification email sent! Please check your inbox.';
  }

  private startCountdown(): void {
    const interval = setInterval(() => {
      this.countdown--;
      
      if (this.countdown <= 0) {
        clearInterval(interval);
        this.router.navigate(['/auth/login']);
      }
    }, 1000);
  }
}