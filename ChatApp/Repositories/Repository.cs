using System.Linq.Expressions;
using ChatApp.Context;
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

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var entity = await _context.FindAsync<T>(id);
        
        return entity;
    }

    public async Task<List<T>?> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        var entities = await _context.Set<T>().Where(predicate).ToListAsync();
        
        return entities;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        
        await SaveAsync();
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        
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