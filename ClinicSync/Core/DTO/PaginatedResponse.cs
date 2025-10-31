using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    // DTOs/Responses/PaginatedResponse.cs
    public class PaginatedResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<T>? Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}
