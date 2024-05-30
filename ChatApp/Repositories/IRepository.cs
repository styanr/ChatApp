using System.Linq.Expressions;
using ChatApp.Models.PagedResult;

namespace ChatApp.Repositories;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(params object[] keyValues);
    Task<PagedResult<T>> GetAllAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null);
    
    Task<bool> ExistsAsync(params object[] keyValues);
    Task UpdateAsync(T entity);
    Task DeleteByIdAsync(params object[] keyValues);
    Task SaveAsync();
}