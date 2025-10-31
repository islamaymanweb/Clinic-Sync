import { UserInfo } from "./user";

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  expires?: string;
  user?: UserInfo;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}