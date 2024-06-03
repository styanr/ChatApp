using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Models.PagedResult;
using ChatApp.Models.Users;
using ChatApp.Repositories.Users;
using ChatApp.Repositories.Contacts;

namespace ChatApp.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IContactRepository _contactRepository;

        public UserService(IUserRepository userRepository, IContactRepository contactRepository)
        {
            _userRepository = userRepository;
            _contactRepository = contactRepository;
        }

        public async Task<UserListResponse> GetUsersForUserAsync(UserSearchRequest request, Guid userId)
        {
            var pagedUsers = await SearchUsersForUserAsync(request, userId);

            pagedUsers.Results.RemoveAll(user => user.Id == userId);

            var userListResponse = await MapPagedResultToUserListResponse(pagedUsers, userId);
            return userListResponse;
        }

        public async Task<UserResponse> GetUserForUserAsync(Guid id, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            var userResponse = await MapUserToUserResponse(user, userId);
            return userResponse;
        }

        public async Task<UserResponse> GetUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            var userResponse = MapUserToUserResponse(user);
            return userResponse;
        }

        public async Task<UserResponse> UpdateUserAsync(Guid id, UserUpdate request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }
            
            user.DisplayName = request.DisplayName;
            user.Bio = request.Bio ?? string.Empty;
            user.ProfilePictureUrl = request.ProfilePictureUrl ?? string.Empty;
            user.Handle = request.Handle ?? string.Empty;
            
            await _userRepository.UpdateAsync(user);
            
            return MapUserToUserResponse(user);
        }

        private async Task<PagedResult<User>> SearchUsersForUserAsync(UserSearchRequest request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return await _userRepository.GetAllAsync(request.Page, request.PageSize);
            }

            // Fetch users matching the search term in their handle or display name
            var globalUsers = await _userRepository.GetAllAsync(request.Page, request.PageSize, user =>
                (!string.IsNullOrEmpty(user.Handle) &&
                 user.Handle.Contains(request.SearchTerm)) ||
                (!string.IsNullOrEmpty(user.DisplayName) &&
                 user.DisplayName.Contains(request.SearchTerm)));

            // Fetch contacts for the current user and filter by custom name
            var contacts = await _contactRepository.GetByUserIdAsync(userId);
            var matchingContacts = contacts.Where(contact =>
                contact.CustomName.Contains(request.SearchTerm)).ToList();

            // Combine results, ensuring no duplicates
            var allUsers = globalUsers.Results.Union(matchingContacts.Select(contact => contact.ContactUser)).ToList();

            // Apply pagination manually as we have a combined result set
            var pagedResult = new PagedResult<User>
            {
                Results = allUsers.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                RowCount = allUsers.Count
            };

            return pagedResult;
        }

        private async Task<UserListResponse> MapPagedResultToUserListResponse(PagedResult<User> pagedUsers, Guid userId)
        {
            var users = new List<UserResponse>();
            foreach (var user in pagedUsers.Results)
            {
                var userResponse = await MapUserToUserResponse(user, userId);
                users.Add(userResponse);
            }
            
            return new UserListResponse
            (
                users.ToList(),
                pagedUsers.CurrentPage,
                pagedUsers.PageSize,
                pagedUsers.RowCount,
                pagedUsers.PageCount
            );
        }

        // userId is the ID of the user making the request. It is used to determine if the user is a contact of the user being fetched
        private async Task<UserResponse> MapUserToUserResponse(User user, Guid userId)
        {
            var contact = await _contactRepository.GetByIdWithIncludesAsync(userId, user.Id);
            return new UserResponse
            (
                user.Id,
                user.Handle,
                contact?.CustomName ?? user.DisplayName,
                user.Bio,
                user.ProfilePictureUrl,
                contact != null
            );
        }
        
        private UserResponse MapUserToUserResponse(User user)
        {
            return new UserResponse
            (
                user.Id,
                user.Handle,
                user.DisplayName,
                user.Bio,
                user.ProfilePictureUrl,
                false
            );
        }
    }
}
