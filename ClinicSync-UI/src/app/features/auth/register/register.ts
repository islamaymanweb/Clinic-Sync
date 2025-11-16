/* import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { Auth } from '../../../core/services/auth/auth';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RegisterRequest } from '../../../shared/models/auth';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  registerForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  showConfirmPassword = false;
  isUserExistsError = false;  

 
  passwordRequirements = {
    minLength: 6,
    requireUppercase: true,
    requireLowercase: true,
    requireDigit: true,
    requireNonAlphanumeric: true
  };

  constructor(
    private formBuilder: FormBuilder,
    private authService: Auth ,
    private router: Router
  ) {
    this.registerForm = this.createRegisterForm();
  }

  private createRegisterForm(): FormGroup {
    return this.formBuilder.group({
      fullName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(this.passwordRequirements.minLength),
        this.passwordValidator()
      ]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: [this.passwordMatchValidator(), this.passwordStrengthValidator()]
    });
  }

  
  private passwordValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      const value = control.value || '';
      
      if (!value) return null;

      const errors: any = {};

       
      if (this.passwordRequirements.requireUppercase && !/(?=.*[A-Z])/.test(value)) {
        errors.uppercase = true;
      }

     
      if (this.passwordRequirements.requireLowercase && !/(?=.*[a-z])/.test(value)) {
        errors.lowercase = true;
      }

      
      if (this.passwordRequirements.requireDigit && !/(?=.*\d)/.test(value)) {
        errors.digit = true;
      }
 
      if (this.passwordRequirements.requireNonAlphanumeric && !/(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])/.test(value)) {
        errors.specialChar = true;
      }

      return Object.keys(errors).length ? errors : null;
    };
  }
 
  private passwordMatchValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      const password = control.get('password');
      const confirmPassword = control.get('confirmPassword');

      if (!password || !confirmPassword) {
        return null;
      }

      return password.value === confirmPassword.value ? null : { passwordMismatch: true };
    };
  }

 
  private passwordStrengthValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      const password = control.get('password');
      
      if (!password || !password.value) {
        return null;
      }

      return null;  
    };
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const registerData: RegisterRequest = this.registerForm.value;

    this.authService.register(registerData).subscribe({
      next: (response) => {
        this.loading = false;
        
        if (response.success) {
          this.successMessage = 'Registration successful! Redirecting to dashboard...';
          this.registerForm.reset();
          this.isUserExistsError = false;
           
        } else {
          this.handleRegistrationError(response.message || 'Registration failed');
        }
      },
      error: (error) => {
        this.loading = false;
        this.handleRegistrationError(error.message || 'An error occurred during registration');
      }
    });
  }

  togglePasswordVisibility(field: 'password' | 'confirmPassword'): void {
    if (field === 'password') {
      this.showPassword = !this.showPassword;
    } else {
      this.showConfirmPassword = !this.showConfirmPassword;
    }
  }
 
  getPasswordErrors(): string[] {
    const errors = [];
    const passwordControl = this.password;

    if (passwordControl?.errors) {
      if (passwordControl.errors['required']) {
        errors.push('Password is required');
      }
      if (passwordControl.errors['minlength']) {
        errors.push(`Password must be at least ${this.passwordRequirements.minLength} characters`);
      }
      if (passwordControl.errors['uppercase']) {
        errors.push('Password must contain at least one uppercase letter (A-Z)');
      }
      if (passwordControl.errors['lowercase']) {
        errors.push('Password must contain at least one lowercase letter (a-z)');
      }
      if (passwordControl.errors['digit']) {
        errors.push('Password must contain at least one digit (0-9)');
      }
      if (passwordControl.errors['specialChar']) {
        errors.push('Password must contain at least one special character (!@#$%^&* etc.)');
      }
    }

    return errors;
  }
 
  getPasswordStrength(): number {
    const password = this.password?.value || '';
    let strength = 0;

    if (password.length >= this.passwordRequirements.minLength) strength += 20;
    if (/(?=.*[A-Z])/.test(password)) strength += 20;
    if (/(?=.*[a-z])/.test(password)) strength += 20;
    if (/(?=.*\d)/.test(password)) strength += 20;
    if (/(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?])/.test(password)) strength += 20;

    return strength;
  }

 
  private handleRegistrationError(errorMessage: string): void {
    this.errorMessage = errorMessage;
    
    
    const errorMsg = errorMessage.toLowerCase();
    this.isUserExistsError = 
      errorMsg.includes('already exists') ||
      errorMsg.includes('user already') ||
      errorMsg.includes('email already') ||
      errorMsg.includes('already registered');
  }
 
  goToLogin(): void {
    const email = this.registerForm.get('email')?.value;
    if (email) {
      this.router.navigate(['/auth/login'], {
        queryParams: { email: email }
      });
    } else {
      this.router.navigate(['/auth/login']);
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      this.registerForm.get(key)?.markAsTouched();
    });
  }
 
  get fullName() { return this.registerForm.get('fullName'); }
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
}
 */
import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { Auth } from '../../../core/services/auth/auth';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RegisterRequest } from '../../../shared/models/auth';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  registerForm: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  showConfirmPassword = false;
  isUserExistsError = false;
  termsAccepted = false;

  // متطلبات كلمة المرور
  passwordRequirements = {
    minLength: 6,
    requireUppercase: true,
    requireDigit: true
  };

  constructor(
    private formBuilder: FormBuilder,
    private authService: Auth,
    private router: Router
  ) {
    this.registerForm = this.createRegisterForm();
  }

  private createRegisterForm(): FormGroup {
    return this.formBuilder.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(this.passwordRequirements.minLength),
        this.passwordValidator()
      ]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: [this.passwordMatchValidator()]
    });
  }

  /**
   * التحقق من قوة كلمة المرور
   */
  private passwordValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      const value = control.value || '';
      
      if (!value) return null;

      const errors: any = {};

      // حرف كبير على الأقل
      if (this.passwordRequirements.requireUppercase && !/(?=.*[A-Z])/.test(value)) {
        errors.uppercase = true;
      }

      // رقم على الأقل
      if (this.passwordRequirements.requireDigit && !/(?=.*\d)/.test(value)) {
        errors.digit = true;
      }

      return Object.keys(errors).length ? errors : null;
    };
  }

  /**
   * التحقق من تطابق كلمتي المرور
   */
  private passwordMatchValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      const password = control.get('password');
      const confirmPassword = control.get('confirmPassword');

      if (!password || !confirmPassword) {
        return null;
      }

      return password.value === confirmPassword.value ? null : { passwordMismatch: true };
    };
  }

  onSubmit(): void {
    if (this.registerForm.invalid || !this.termsAccepted) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const registerData: RegisterRequest = this.registerForm.value;

    this.authService.register(registerData).subscribe({
      next: (response) => {
        this.loading = false;
        
        if (response.success) {
          this.successMessage = 'Your medical account has been created successfully!';
          this.registerForm.reset();
          this.isUserExistsError = false;
          
          // توجيه تلقائي بعد التسجيل الناجح
          setTimeout(() => {
            this.router.navigate(['/patient/dashboard']);
          }, 2000);
        } else {
          this.handleRegistrationError(response.message || 'Registration failed');
        }
      },
      error: (error) => {
        this.loading = false;
        this.handleRegistrationError(error.message || 'An error occurred during registration');
      }
    });
  }

  togglePasswordVisibility(field: 'password' | 'confirmPassword'): void {
    if (field === 'password') {
      this.showPassword = !this.showPassword;
    } else {
      this.showConfirmPassword = !this.showConfirmPassword;
    }
  }

  onTermsChange(event: any): void {
    this.termsAccepted = event.target.checked;
  }

  /**
   * الحصول على رسائل خطأ كلمة المرور
   */
  getPasswordErrors(): string[] {
    const errors = [];
    const passwordControl = this.password;

    if (passwordControl?.errors) {
      if (passwordControl.errors['required']) {
        errors.push('Password is required');
      }
      if (passwordControl.errors['minlength']) {
        errors.push(`Password must be at least ${this.passwordRequirements.minLength} characters`);
      }
      if (passwordControl.errors['uppercase']) {
        errors.push('Password must contain at least one uppercase letter');
      }
      if (passwordControl.errors['digit']) {
        errors.push('Password must contain at least one number');
      }
    }

    return errors;
  }

  /**
   * حساب قوة كلمة المرور
   */
  getPasswordStrength(): number {
    const password = this.password?.value || '';
    let strength = 0;

    if (password.length >= this.passwordRequirements.minLength) strength += 40;
    if (/(?=.*[A-Z])/.test(password)) strength += 30;
    if (/(?=.*\d)/.test(password)) strength += 30;

    return Math.min(strength, 100);
  }

  /**
   * معالجة أخطاء التسجيل
   */
  private handleRegistrationError(errorMessage: string): void {
    this.errorMessage = errorMessage;
    
    const errorMsg = errorMessage.toLowerCase();
    this.isUserExistsError = 
      errorMsg.includes('already exists') ||
      errorMsg.includes('user already') ||
      errorMsg.includes('email already') ||
      errorMsg.includes('already registered');
  }

  /**
   * الانتقال إلى صفحة تسجيل الدخول
   */
  goToLogin(): void {
    const email = this.registerForm.get('email')?.value;
    if (email) {
      this.router.navigate(['/auth/login'], {
        queryParams: { email: email }
      });
    } else {
      this.router.navigate(['/auth/login']);
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      this.registerForm.get(key)?.markAsTouched();
    });
  }

  // Getters for template access
  get fullName() { return this.registerForm.get('fullName'); }
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
}