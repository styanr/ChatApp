using System.Linq.Expressions;
using ChatApp.Context;
using ChatApp.Models.PagedResult;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ChatDbContext _context;

    public Repository(ChatDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(T entity)
    {
        _context.Add(entity);
        
        await SaveAsync();
    }

    public async Task<T?> GetByIdAsync(params object[] keyValues)
    {
        var entity = await _context.FindAsync<T>(keyValues);
        
        return entity;
    }
    
    public async Task<PagedResult<T>> GetAllAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null)
    {
        var entities = predicate == null
            ? _context.Set<T>()
            : _context.Set<T>().Where(predicate);
        
        var pagedResult = await entities.GetPagedAsync(page, pageSize);
        
        
        return pagedResult;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        
        await SaveAsync();
    }

    public async Task DeleteByIdAsync(params object[] keyValues)
    {
        var entity = await GetByIdAsync(keyValues);
        
        if (entity != null)
        {
            _context.Remove(entity);
            
            await SaveAsync();
        }
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}