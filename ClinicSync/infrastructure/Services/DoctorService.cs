using Core.DTO;
using Core.Entities;
using Core.Services;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(ApplicationDbContext context, ILogger<DoctorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedDoctorsResponse> SearchDoctorsAsync(DoctorSearchRequest request)
        {
            try
            {
                // Base query
                var query = _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Specialty)
                    .Where(d => d.IsApproved && d.User.IsActive);

                // Apply filters
                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var name = request.Name.Trim().ToLower();
                    query = query.Where(d => d.User.FullName.ToLower().Contains(name));
                }

                if (request.SpecialtyId.HasValue)
                {
                    query = query.Where(d => d.SpecialtyId == request.SpecialtyId.Value);
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "experience" => request.SortDescending ?
                        query.OrderByDescending(d => d.YearsOfExperience) :
                        query.OrderBy(d => d.YearsOfExperience),
                    "fee" => request.SortDescending ?
                        query.OrderByDescending(d => d.ConsultationFee) :
                        query.OrderBy(d => d.ConsultationFee),
                    _ => request.SortDescending ?
                        query.OrderByDescending(d => d.User.FullName) :
                        query.OrderBy(d => d.User.FullName)
                };

                // Pagination
                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
                var skipAmount = (request.PageNumber - 1) * request.PageSize;

                var doctors = await query
                    .Skip(skipAmount)
                    .Take(request.PageSize)
                    .Select(d => new DoctorSearchResult
                    {
                        Id = d.Id,
                        FullName = d.User.FullName,
                        Specialty = d.Specialty.Name,
                        SpecialtyId = d.SpecialtyId,
                        ProfilePictureUrl = d.User.ProfilePictureUrl,
                        YearsOfExperience = d.YearsOfExperience,
                        ConsultationFee = d.ConsultationFee,
                        AverageRating = 4.5,
                        TotalReviews = 0,
                        Bio = d.Bio,
                        LicenseNumber = d.LicenseNumber,
                        IsAvailable = true
                    })
                    .ToListAsync();

                return new PaginatedDoctorsResponse
                {
                    Success = true,
                    Message = doctors.Count > 0 ? "Doctors found successfully" : "No doctors found",
                    Data = doctors,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                    HasPrevious = request.PageNumber > 1,
                    HasNext = request.PageNumber < totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors");
                return new PaginatedDoctorsResponse
                {
                    Success = false,
                    Message = "An error occurred while searching doctors"
                };
            }
        }

        public async Task<List<Specialty>> GetSpecialtiesAsync()
        {
            return await _context.Specialties
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<DoctorSearchResult?> GetDoctorByIdAsync(Guid id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Where(d => d.Id == id && d.IsApproved && d.User.IsActive)
                .Select(d => new DoctorSearchResult
                {
                    Id = d.Id,
                    FullName = d.User.FullName,
                    Specialty = d.Specialty.Name,
                    SpecialtyId = d.SpecialtyId,
                    ProfilePictureUrl = d.User.ProfilePictureUrl,
                    YearsOfExperience = d.YearsOfExperience,
                    ConsultationFee = d.ConsultationFee,
                    AverageRating = 4.5,
                    TotalReviews = 0,
                    Bio = d.Bio,
                    LicenseNumber = d.LicenseNumber,
                    IsAvailable = true
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetSearchSuggestionsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<string>();

            var suggestions = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Where(d => d.IsApproved &&
                       (d.User.FullName.ToLower().Contains(query.ToLower()) ||
                        d.Specialty.Name.ToLower().Contains(query.ToLower())))
                .Select(d => $"{d.User.FullName} - {d.Specialty.Name}")
                .Distinct()
                .Take(10)
                .ToListAsync();

            return suggestions;
        }
    }
}
