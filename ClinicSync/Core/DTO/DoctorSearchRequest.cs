using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
  
    public class DoctorSearchRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        public Guid? SpecialtyId { get; set; }

        [Range(1, 100)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 50)]
        public int PageSize { get; set; } = 9;

        public string? SortBy { get; set; } = "name";
        public bool SortDescending { get; set; } = false;
    }

 
    public class DoctorSearchResult
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public Guid SpecialtyId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal ConsultationFee { get; set; }
        public double? AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string? Bio { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    
    public class PaginatedDoctorsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<DoctorSearchResult>? Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }

}
