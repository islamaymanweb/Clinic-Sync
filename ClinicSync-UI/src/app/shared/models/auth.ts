export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
}
export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  email: string;
  newPassword: string;
  confirmPassword: string;
}

export interface CreateDoctorRequest {
  fullName: string;
  email: string;
  specialtyId: string;
  licenseNumber: string;
  yearsOfExperience: number;
  consultationFee: number;
  bio?: string;
}