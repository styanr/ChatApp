using ChatApp.Models;
using ChatApp.Models.Users;

namespace ChatApp.Services.Contacts;

public interface IContactService
{
    Task<UserListResponse> GetContactsAsync(Guid userId, PagedRequest pagedRequest);
    Task<UserResponse> AddContactAsync(Guid userId, UserContactAdd userContactAdd);
    Task RemoveContactAsync(Guid userId, Guid contactId);
    Task<UserResponse> UpdateContactAsync(Guid userId, Guid contactId, UserContactUpdate userUpdateContact);
}