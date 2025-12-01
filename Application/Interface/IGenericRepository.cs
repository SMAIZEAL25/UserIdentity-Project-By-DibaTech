using Application.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IGenericRepository <T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id); // For Guid-based entities
        Task<IReadOnlyList<T>> GetAllAsync();

        // Pagination + Filtering + Sorting
        Task<PaginatedResult<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "");

        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        //Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
      
        //Task<T> GetAsync(int? id);
    }
}
