using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Mapping;
using ChatApp.Models;
using ChatApp.Models.PagedResult;
using ChatApp.Models.Users;
using ChatApp.Repositories.Contacts;
using ChatApp.Repositories.Users;

namespace ChatApp.Services.Contacts;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IUserRepository _userRepository;

    public ContactService(IContactRepository contactRepository, IUserRepository userRepository)
    {
        _contactRepository = contactRepository;
        _userRepository = userRepository;
    }

    public async Task<UserListResponse> GetContactsAsync(Guid userId, PagedRequest pagedRequest)
    {
        var contacts = await _contactRepository.GetAllWithIncludesAsync(pagedRequest.Page, pagedRequest.PageSize, c => c.UserId == userId);
        
        var userResponses = contacts.ToUserListResponse();
        
        return userResponses;
    }

    public async Task<UserResponse> AddContactAsync(Guid userId, UserContactAdd userContactAdd)
    {
        if (userId == userContactAdd.ContactUserId)
        {
            throw new UnauthorizedAccessException("User cannot add themselves as a contact");
        }
        
        var userContact = await _userRepository.GetByIdAsync(userContactAdd.ContactUserId);
        
        if (userContact is null)
        {
            throw new UserNotFoundException(userContactAdd.ContactUserId);
        }
        
        var existingContact = await _contactRepository.GetByIdWithIncludesAsync(userId, userContactAdd.ContactUserId);
        
        if (existingContact is not null)
        {
            throw new ContactAlreadyExistsException();
        }
        
        var contact = new Contact
        {
            UserId = userId,
            ContactId = userContactAdd.ContactUserId,
            CustomName = userContact.DisplayName
        };
        
        await _contactRepository.AddAsync(contact);
        
        return contact.ToUserResponse();
    }

    public async Task RemoveContactAsync(Guid userId, Guid contactId)
    {
        var contact = await _contactRepository.GetByIdWithIncludesAsync(userId, contactId);
        
        if (contact == null)
        {
            throw new ContactNotFoundException(contactId);
        }
        
        if (contact.UserId != userId)
        {
            throw new UnauthorizedAccessException("User is not authorized to remove this contact");
        }
        
        await _contactRepository.DeleteByIdAsync(userId, contactId);
        
    }
    
    public async Task<UserResponse> UpdateContactAsync(Guid userId, Guid contactId, UserContactUpdate userUpdateContact)
    {
        var contact = await _contactRepository.GetByIdWithIncludesAsync(userId, contactId);
        
        if (contact == null)
        {
            throw new ContactNotFoundException(contactId);
        }
        
        if (contact.UserId != userId)
        {
            throw new UnauthorizedAccessException("User is not authorized to update this contact");
        }
        
        contact.CustomName = userUpdateContact.DisplayName;
        
        await _contactRepository.UpdateAsync(contact);

        return contact.ToUserResponse();
    }

}