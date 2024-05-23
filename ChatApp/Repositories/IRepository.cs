using System.Linq.Expressions;

namespace ChatApp.Repositories;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<List<T>?> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task UpdateAsync(T entity);
    Task DeleteByIdAsync(int id);
    Task SaveAsync();
}