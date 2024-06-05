using ChatApp.Entities;
using ChatApp.Exceptions;
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
        
        var userResponses = MapPagedResultToUserListResponse(contacts);
        
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
            throw new UserNotFoundException("User not found");
        }
        
        var existingContactUser = await _userRepository.GetByIdAsync(userContactAdd.ContactUserId);
        
        if (existingContactUser == null)
        {
            throw new UserNotFoundException("Contact user not found");
        }
        
        var existingContact = await _contactRepository.GetByIdWithIncludesAsync(userId, userContactAdd.ContactUserId);
        
        if (existingContact is not null)
        {
            throw new ContactAlreadyExistsException("Contact already exists");
        }
        
        var contact = new Contact
        {
            UserId = userId,
            ContactId = userContactAdd.ContactUserId,
            CustomName = userContact.DisplayName
        };
        
        await _contactRepository.AddAsync(contact);
        
        return MapContactToUserResponse(contact);
    }

    public async Task RemoveContactAsync(Guid userId, Guid contactId)
    {
        var contact = await _contactRepository.GetByIdWithIncludesAsync(userId, contactId);
        
        if (contact == null)
        {
            throw new ContactNotFoundException("Contact not found");
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
            throw new ContactNotFoundException("Contact not found");
        }
        
        if (contact.UserId != userId)
        {
            throw new UnauthorizedAccessException("User is not authorized to update this contact");
        }
        
        contact.CustomName = userUpdateContact.DisplayName;
        
        await _contactRepository.UpdateAsync(contact);
        
        return MapContactToUserResponse(contact);
    }
    
    // TODO: Move these methods to a mapper class
    private UserResponse MapContactToUserResponse(Contact contact)
    {
        return new UserResponse(contact.ContactId, contact.ContactUser.Handle, contact.CustomName, contact.ContactUser.Bio, contact.ContactUser.ProfilePictureId, true);
    }

    private UserListResponse MapPagedResultToUserListResponse(PagedResult<Contact> pagedResult)
    {
        var userResponses = pagedResult.Results.Select(MapContactToUserResponse).ToList();

        return new UserListResponse(
            userResponses,
            pagedResult.CurrentPage,
            pagedResult.PageSize,
            pagedResult.RowCount,
            pagedResult.PageCount);
    }

}