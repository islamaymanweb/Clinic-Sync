  
export interface Specialty {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface DoctorSearchResult {
  id: string;
  fullName: string;
  specialty: string;
  specialtyId: string;
  profilePictureUrl?: string;
  yearsOfExperience: number;
  consultationFee: number;
  averageRating?: number;
  totalReviews: number;
  bio?: string;
  licenseNumber: string;
  isAvailable: boolean;
}

export interface DoctorSearchRequest {
  name?: string;
  specialtyId?: string;
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface PaginatedDoctorsResponse {
  success: boolean;
  message: string;
  data?: DoctorSearchResult[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}