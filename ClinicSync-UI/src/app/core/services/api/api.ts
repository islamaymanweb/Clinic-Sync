import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginRequest, RegisterRequest, ResetPasswordRequest } from '../../../shared/models/auth';
import { ApiResponse, AuthResponse } from '../../../shared/models/api';
import { API_ENDPOINTS } from './endpoints';
import { UserInfo } from '../../../shared/models/user';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
/* export class Api {
 
  private buildUrl(endpoint: string): string {
    return `${this.apiUrl}${endpoint}`;
  }

  // 🔄 الطرق المعدلة لاستخدام البيئة والكوكيز

  get<T>(endpoint: string, params?: any): Observable<T> {
    const url = this.buildUrl(endpoint);
     console.log('API Request:', url, params);  
    return this.http.get<T>(url, { 
      params,
      withCredentials: true  
    });
  }

  post<T>(endpoint: string, body: any): Observable<T> {
    const url = this.buildUrl(endpoint);
    return this.http.post<T>(url, body, {
      withCredentials: true  
    });
  }

  put<T>(endpoint: string, body: any): Observable<T> {
    const url = this.buildUrl(endpoint);
    return this.http.put<T>(url, body, {
      withCredentials: true
    });
  }

  delete<T>(endpoint: string): Observable<T> {
    const url = this.buildUrl(endpoint);
    return this.http.delete<T>(url, {
      withCredentials: true
    });
  }

  // 🔐 طرق المصادقة - محدثة لاستخدام البيئة والكوكيز
  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.login, credentials);
  }

  register(userData: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.register, userData);
  }

  logout(): Observable<ApiResponse<any>> {
    return this.post(environment.endpoints.auth.logout, {});
  }

  getCurrentUser(): Observable<ApiResponse<UserInfo>> {
    return this.get(environment.endpoints.auth.me);
  }

 

  forgotPassword(email: string): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.forgotPassword, { email });
  }

  resetPassword(data: ResetPasswordRequest): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.resetPassword, data);
  }

 

 
  createDoctor(doctorData: any): Observable<ApiResponse<any>> {
    return this.post(environment.endpoints.admin.doctors, doctorData);
  }

  getUsers(): Observable<ApiResponse<any>> {
    return this.get(environment.endpoints.admin.users);
  }
} */
export class Api {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * بناء URL كامل باستخدام البيئة
   */
  private buildUrl(endpoint: string): string {
    return `${this.apiUrl}${endpoint}`;
  }

  // 🔄 الطرق المعدلة لاستخدام البيئة والكوكيز
  get<T>(endpoint: string, params?: any): Observable<T> {
    const url = this.buildUrl(endpoint);
    return this.http.get<T>(url, { 
      params,
      withCredentials: true  
    });
  }

  post<T>(endpoint: string, body: any): Observable<T> {
    const url = this.buildUrl(endpoint);
    return this.http.post<T>(url, body, {
      withCredentials: true  
    });
  }

  put<T>(endpoint: string, body: any): Observable<T> {
    const url = this.buildUrl(endpoint);
    return this.http.put<T>(url, body, {
      withCredentials: true
    });
  }

  delete<T>(endpoint: string): Observable<T> {
    const url = this.buildUrl(endpoint);
    return this.http.delete<T>(url, {
      withCredentials: true
    });
  }

  // 🔐 طرق المصادقة - محدثة بدون دوال التحقق من البريد
  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.login, credentials);
  }

  register(userData: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.register, userData);
  }

  logout(): Observable<ApiResponse<any>> {
    return this.post(environment.endpoints.auth.logout, {});
  }

  getCurrentUser(): Observable<ApiResponse<UserInfo>> {
    return this.get(environment.endpoints.auth.me);
  }

  forgotPassword(email: string): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.forgotPassword, { email });
  }

  resetPassword(data: ResetPasswordRequest): Observable<ApiResponse<AuthResponse>> {
    return this.post(environment.endpoints.auth.resetPassword, data);
  }

  // 🛠️ طرق الإدارة
  createDoctor(doctorData: any): Observable<ApiResponse<any>> {
    return this.post(environment.endpoints.admin.doctors, doctorData);
  }

  getUsers(): Observable<ApiResponse<any>> {
    return this.get(environment.endpoints.admin.users);
  }
}