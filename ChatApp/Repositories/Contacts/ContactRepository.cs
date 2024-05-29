using System.Linq.Expressions;
using ChatApp.Context;
using ChatApp.Entities;
using ChatApp.Models.PagedResult;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories.Contacts;

public class ContactRepository(ChatDbContext context) : Repository<Contact>(context), IContactRepository
{
    public async Task<Contact?> GetByIdWithIncludesAsync(Guid userId, Guid contactId)
    {
        var contact = await _context.Contacts
            .GetQueryableWithIncludes()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ContactId == contactId);

        return contact;       
    }

    public async Task<PagedResult<Contact>> GetAllWithIncludesAsync(int page, int pageSize, Expression<Func<Contact, bool>>? predicate = null)
    {
        var query = _context.Contacts
            .GetQueryableWithIncludes();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }
        
        return await query.GetPagedAsync(page, pageSize);
    }

    public async Task<List<Contact>> GetByUserIdAsync(Guid userId)
    {
        var contacts = await _context.Contacts
            .GetQueryableWithIncludes()
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return contacts;
    }
}

public static class QueryablePagedResultExtension
{
    public static IQueryable<Contact> GetQueryableWithIncludes(this IQueryable<Contact> query)
    {
        return query.Include(c => c.User)
            .Include(c => c.ContactUser);
    }
}