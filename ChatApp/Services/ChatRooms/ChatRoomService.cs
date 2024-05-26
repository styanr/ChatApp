using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Mapping;
using ChatApp.Models.Messages;
using ChatApp.Repositories.ChatRooms;
using ChatApp.Repositories.Messages;
using ChatApp.Repositories.Users;

namespace ChatApp.Services.ChatRooms
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatRoomService(IChatRoomRepository chatRoomRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ChatRoomSummary>> GetAllAsync(Guid userId)
        {
            var directChatRoomsTask = await _chatRoomRepository.GetDirectChatRoomsAsync(userId);
            var groupChatRoomsTask = await _chatRoomRepository.GetGroupChatRoomsAsync(userId);
            
            var chatRoomSummaries = directChatRoomsTask.Select(dc => dc.ToChatRoomSummary(userId))
                .Concat(groupChatRoomsTask.Select(gc => gc.ToChatRoomSummary()));
            return chatRoomSummaries;
        }

        public async Task<ChatRoomSummary> CreateGroupChatAsync(Guid userId, ChatRoomCreate chatRoomCreate)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var groupChatRoom = new GroupChatRoom
            {
                Name = chatRoomCreate.Name ?? throw new ArgumentNullException(nameof(chatRoomCreate.Name)),
                PictureUrl = chatRoomCreate.PictureUrl,
                UserList = [user]
            };

            await _chatRoomRepository.AddAsync(groupChatRoom);

            return groupChatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> AddUsersToChatAsync(Guid userId, Guid chatId, ChatRoomAddUsers chatRoomAddUsers)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId);
            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);
            
            if (user is null)
            {
                throw new UserNotFoundException("User not found in chat room");
            }
            
            var users = await ValidateUsersExistenceAsync(chatRoomAddUsers.UserIds);

            chatRoom.UserList = chatRoom.UserList.Union(users).ToList();

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> RemoveUserFromChatAsync(Guid userId, Guid chatId, Guid deleteUserId)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId);
            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                throw new UserNotFoundException("User not found in chat room");
            }
            
            var userToDelete = chatRoom.UserList.FirstOrDefault(u => u.Id == deleteUserId);
            if (userToDelete is null)
            {
                throw new UserNotFoundException("User to delete not found in chat room");
            }

            chatRoom.UserList.Remove(user);

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> UpdateGroupChatAsync(Guid userId, Guid chatId, ChatRoomUpdate chatRoomUpdate)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId);
            
            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);
            if (user is null)
            {
                throw new UserNotFoundException("User not found in chat room");
            }

            chatRoom.Name = chatRoomUpdate.Name ?? chatRoom.Name;
            chatRoom.Description = chatRoomUpdate.Description ?? chatRoom.Description;
            chatRoom.PictureUrl = chatRoomUpdate.PictureUrl ?? chatRoom.PictureUrl;

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomSummary();
        }
        
        public async Task<bool> IsUserInChatAsync(Guid userId, Guid chatId)
        {
            var chatRoom = await _chatRoomRepository.GetByIdAsync(chatId);
            if (chatRoom is null)
            {
                throw new ChatRoomNotFoundException("Chat room not found");
            }

            return chatRoom.Users.Any(u => u.Id == userId);
        }

        private async Task<GroupChatRoom> GetGroupChatRoomWithUserValidationAsync(Guid chatId)
        {
            var chatRoom = await _chatRoomRepository.GetGroupChatRoomAsync(chatId);
            
            if (chatRoom is null)
            {
                throw new ChatRoomNotFoundException("Chat room not found");
            }
            
            if (chatRoom.UserList.All(u => u.Id != chatId))
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
