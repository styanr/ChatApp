using ChatApp.Entities;
using ChatApp.Models.PagedResult;
using ChatApp.Models.Users;

namespace ChatApp.Mapping;

public static class ContactExtensions
{
    public static UserResponse ToUserResponse(this Contact contact)
    {
        return new UserResponse(contact.ContactId, contact.ContactUser.Handle, contact.CustomName, contact.ContactUser.Bio, contact.ContactUser.ProfilePictureId, true);
    }
    
    public static UserListResponse ToUserListResponse(this PagedResult<Contact> pagedResult)
    {
        var userResponses = pagedResult.Results.Select(c => c.ToUserResponse()).ToList();

        return new UserListResponse(
            userResponses,
            pagedResult.CurrentPage,
            pagedResult.PageSize,
            pagedResult.RowCount,
            pagedResult.PageCount);
    }
}