import { Gender, UserRole } from "./enums";

export interface User {
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
  profilePictureUrl?: string;
  emailVerified: boolean;
  isActive: boolean;
  createdAt: string;
  
  // Patient specific
  patient?: {
    dateOfBirth?: string;
    gender?: Gender;
    phoneNumber?: string;
    emergencyContact?: string;
  };
  
  // Doctor specific
  doctor?: {
    licenseNumber: string;
    yearsOfExperience: number;
    consultationFee: number;
    bio?: string;
    isApproved: boolean;
    specialty?: {
      id: string;
      name: string;
      description?: string;
    };
  };
}

export interface UserInfo {
  id: string;
  fullName: string;
  email: string;
  role: string;
  profilePictureUrl?: string;
}