using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Mapping;
using ChatApp.Models.Messages;
using ChatApp.Repositories.ChatRooms;
using ChatApp.Repositories.Users;

namespace ChatApp.Services.ChatRooms
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatRoomService(IChatRoomRepository chatRoomRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ChatRoomSummary>> GetAllAsync()
        {
            var userGuid = GetCurrentUserId();

            var directChatRoomsTask = _chatRoomRepository.GetDirectChatRoomsAsync(userGuid);
            var groupChatRoomsTask = _chatRoomRepository.GetGroupChatRoomsAsync(userGuid);

            await Task.WhenAll(directChatRoomsTask, groupChatRoomsTask);

            var chatRoomSummaries = directChatRoomsTask.Result.Select(dc => dc.ToChatRoomSummary(userGuid))
                .Concat(groupChatRoomsTask.Result.Select(gc => gc.ToChatRoomSummary()));

            return chatRoomSummaries;
        }

        public async Task<ChatRoomSummary> CreateGroupChatAsync(ChatRoomCreate chatRoomCreate)
        {
            var userGuid = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userGuid);

            var groupChatRoom = new GroupChatRoom
            {
                Name = chatRoomCreate.Name ?? throw new ArgumentNullException(nameof(chatRoomCreate.Name)),
                PictureUrl = chatRoomCreate.PictureUrl,
                Users = [user]
            };

            await _chatRoomRepository.AddAsync(groupChatRoom);

            return groupChatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> AddUsersToChatAsync(Guid chatId, ChatRoomAddUsers chatRoomAddUsers)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId);
            var users = await ValidateUsersExistenceAsync(chatRoomAddUsers.UserIds);

            chatRoom.Users = chatRoom.Users.Union(users).ToList();

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> RemoveUserFromChatAsync(Guid chatId, Guid userId)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId);
            var user = chatRoom.Users.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                throw new UserNotFoundException("User not found in chat room");
            }

            chatRoom.Users.Remove(user);

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> UpdateGroupChatAsync(Guid chatId, ChatRoomUpdate chatRoomUpdate)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId);

            chatRoom.Name = chatRoomUpdate.Name ?? chatRoom.Name;
            chatRoom.Description = chatRoomUpdate.Description ?? chatRoom.Description;
            chatRoom.PictureUrl = chatRoomUpdate.PictureUrl ?? chatRoom.PictureUrl;

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomSummary();
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                throw new UserNotFoundException("User not found");
            }

            return Guid.Parse(userId);
        }

        private async Task<GroupChatRoom> GetGroupChatRoomWithUserValidationAsync(Guid chatId)
        {
            var chatRoom = await _chatRoomRepository.GetGroupChatRoomAsync(chatId);
            if (chatRoom is null)
            {
                throw new ChatRoomNotFoundException("Chat room not found");
            }

            var userGuid = GetCurrentUserId();
            if (chatRoom.Users.All(u => u.Id != userGuid))
            {
                throw new UserNotFoundException("User not found in chat room");
            }

            return chatRoom;
        }

        private async Task<IEnumerable<User>> ValidateUsersExistenceAsync(IEnumerable<Guid> userIds)
        {
            var userIdsList = userIds.ToList();
            
            var users = (await _userRepository.GetUsersByIdsAsync(userIdsList)).ToList();
            if (users.Count != userIdsList.Count)
            {
                throw new UserNotFoundException("Some users not found");
            }

            return users;
        }
    }
}
