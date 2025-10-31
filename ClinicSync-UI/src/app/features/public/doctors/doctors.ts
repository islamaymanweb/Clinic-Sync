import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { DoctorSearchRequest, DoctorSearchResult, Specialty } from '../../../shared/models/Specialty';
import { Doctor } from '../../../core/services/doctor';
import { Router } from '@angular/router';

@Component({
  selector: 'app-doctors',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './doctors.html',
  styleUrl: './doctors.scss',
})
export class Doctors implements OnInit {
  searchForm: FormGroup;
  specialties: Specialty[] = [];
  doctors: DoctorSearchResult[] = [];
  loading = false;
  
  // Pagination
  currentPage = 1;
  pageSize = 9;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  // Sort options
  sortOptions = [
    { value: 'name', label: 'Name (A-Z)', descending: false },
    { value: 'name', label: 'Name (Z-A)', descending: true },
    { value: 'experience', label: 'Experience (High to Low)', descending: true },
    { value: 'experience', label: 'Experience (Low to High)', descending: false },
    { value: 'fee', label: 'Fee (High to Low)', descending: true },
    { value: 'fee', label: 'Fee (Low to High)', descending: false }
  ];

  constructor(
    private formBuilder: FormBuilder,
    private doctorService: Doctor,
    private router: Router
  ) {
    this.searchForm = this.createSearchForm();
  }

  ngOnInit(): void {
    this.loadSpecialties();
    this.loadDoctors();
  }

  private createSearchForm(): FormGroup {
    return this.formBuilder.group({
      name: [''],
      specialtyId: [''],
      sortBy: ['name'],
      sortDescending: [false]
    });
  }

  private loadSpecialties(): void {
    this.doctorService.getSpecialties().subscribe({
      next: (specialties) => {
        this.specialties = specialties;
      },
      error: (error) => {
        console.error('Error loading specialties:', error);
      }
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadDoctors();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadDoctors();
  }

  private loadDoctors(): void {
    this.loading = true;

    const formValue = this.searchForm.value;
    const searchRequest: DoctorSearchRequest = {
      name: formValue.name,
      specialtyId: formValue.specialtyId,
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortBy: formValue.sortBy,
      sortDescending: formValue.sortDescending
    };

    this.doctorService.getDoctors(searchRequest).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.doctors = response.data;
          this.totalCount = response.totalCount;
          this.totalPages = response.totalPages;
          this.hasPreviousPage = response.hasPrevious;
          this.hasNextPage = response.hasNext;
        } else {
          this.doctors = [];
          this.totalCount = 0;
        }
      },
      error: (error) => {
        this.loading = false;
        console.error('Error loading doctors:', error);
        this.doctors = [];
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadDoctors();
    this.scrollToTop();
  }

  onDoctorSelect(doctor: DoctorSearchResult): void {
    this.router.navigate(['/doctors', doctor.id, 'book']);
  }

  clearFilters(): void {
    this.searchForm.reset({
      name: '',
      specialtyId: '',
      sortBy: 'name',
      sortDescending: false
    });
    this.currentPage = 1;
    this.loadDoctors();
  }

  private scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisiblePages = 5;
    
    let startPage = Math.max(1, this.currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxVisiblePages - 1);
    
    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  getYearsOfExperienceText(years: number): string {
    if (years === 1) return '1 year';
    return `${years} years`;
  }

/*   getStarRating(rating?: number): string {
    if (!rating) return 'No ratings';
    return `${rating.toFixed(1)}/5.0`;
  } */
}