using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Result
{
    public record PaginatedResult<T>(
     IReadOnlyList<T> Items,
     int TotalCount,
     int PageNumber,
     int PageSize)

    {        
        public int TotalPages => TotalCount > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
