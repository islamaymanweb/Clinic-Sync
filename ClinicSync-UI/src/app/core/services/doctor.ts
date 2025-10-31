import { Injectable } from '@angular/core';
import { DoctorSearchRequest, DoctorSearchResult, PaginatedDoctorsResponse, Specialty } from '../../shared/models/Specialty';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class Doctor {
   private readonly baseUrl = `${environment.apiUrl}/api/doctors`;

  constructor(private http: HttpClient) {}

  getDoctors(request: DoctorSearchRequest): Observable<PaginatedDoctorsResponse> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.name) {
      params = params.set('name', request.name);
    }
    if (request.specialtyId) {
      params = params.set('specialtyId', request.specialtyId);
    }
    if (request.sortBy) {
      params = params.set('sortBy', request.sortBy);
    }
    if (request.sortDescending !== undefined) {
      params = params.set('sortDescending', request.sortDescending.toString());
    }

    return this.http.get<PaginatedDoctorsResponse>(this.baseUrl, { params });
  }

  getSpecialties(): Observable<Specialty[]> {
    return this.http.get<Specialty[]>(`${this.baseUrl}/specialties`);
  }

  getDoctorById(id: string): Observable<DoctorSearchResult> {
    return this.http.get<DoctorSearchResult>(`${this.baseUrl}/${id}`);
  }

  getSearchSuggestions(query: string): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/suggestions`, {
      params: { query }
    });
  }
}