using Core.DTO;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IDoctorService
    {
        Task<PaginatedDoctorsResponse> SearchDoctorsAsync(DoctorSearchRequest request);
        Task<List<Specialty>> GetSpecialtiesAsync();
        Task<DoctorSearchResult?> GetDoctorByIdAsync(Guid id);
        Task<List<string>> GetSearchSuggestionsAsync(string query);
    }
}
