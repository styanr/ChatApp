using System.Linq.Expressions;
using ChatApp.Entities;
using ChatApp.Models.PagedResult;

namespace ChatApp.Repositories.Contacts;

public interface IContactRepository : IRepository<Contact>
{
    Task<Contact?> GetByIdWithIncludesAsync(Guid userId, Guid contactId);
    Task<PagedResult<Contact>> GetAllWithIncludesAsync(int page, int pageSize, Expression<Func<Contact, bool>>? predicate = null);
    Task<List<Contact>> GetByUserIdAsync(Guid userId);
}