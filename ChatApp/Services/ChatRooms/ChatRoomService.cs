using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Managers;
using ChatApp.Mapping;
using ChatApp.Models.Messages;
using ChatApp.Repositories.ChatRooms;
using ChatApp.Repositories.Contacts;
using ChatApp.Repositories.Messages;
using ChatApp.Repositories.Users;

namespace ChatApp.Services.ChatRooms
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IContactRepository _contactRepository;

        public ChatRoomService(IChatRoomRepository chatRoomRepository, IUserRepository userRepository,
            IContactRepository contactRepository, IUserManager userManager)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _contactRepository = contactRepository;
        }

        public async Task<IEnumerable<ChatRoomSummary>> GetAllAsync(Guid userId)
        {
            var directChatRooms = (await _chatRoomRepository.GetDirectChatRoomsAsync(userId)).ToList();
            var groupChatRooms = (await _chatRoomRepository.GetGroupChatRoomsAsync(userId)).ToList();

            var chatRoomSummaries = directChatRooms.Select(dc => dc.ToChatRoomSummary(userId))
                .Concat(groupChatRooms.Select(gc => gc.ToChatRoomSummary()));

            return chatRoomSummaries;
        }

        public async Task<ChatRoomSummary> GetChatAsync(Guid userId, Guid chatId)
        {
            var chatRoom = await _chatRoomRepository.GetByIdAsync(chatId);
            if (chatRoom is null)
            {
                throw new ChatRoomNotFoundException("Chat room not found");
            }

            if (chatRoom.Users.All(u => u.Id != userId))
            {
                throw new UserNotFoundException("User not found in chat room");
            }

            return chatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> CreateDirectChatAsync(Guid userId, Guid otherUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var otherUser = await _userRepository.GetByIdAsync(otherUserId);

            if (user is null || otherUser is null)
            {
                throw new UserNotFoundException("User not found");
            }

            if (user.Id == otherUser.Id)
            {
                throw new ArgumentException("Cannot create chat with self");
            }

            if (!await IsUserContactAsync(userId, otherUserId))
            {
                throw new ArgumentException("User is not a contact");
            }

            if (await _chatRoomRepository.DirectChatRoomExists(userId, otherUserId))
            {
                throw new ArgumentException("Direct chat room already exists");
            }

            var directChatRoom = new DirectChatRoom
            {
                User1 = user,
                User2 = otherUser
            };

            await _chatRoomRepository.AddAsync(directChatRoom);

            return directChatRoom.ToChatRoomSummary(userId);
        }

        public async Task<ChatRoomSummary> CreateGroupChatAsync(Guid userId, GroupChatRoomCreate groupChatRoomCreate)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var groupChatRoom = new GroupChatRoom
            {
                Name = groupChatRoomCreate.Name ?? throw new ArgumentNullException(nameof(groupChatRoomCreate.Name)),
                PictureUrl = groupChatRoomCreate.PictureUrl,
                UserList = [user]
            };

            await _chatRoomRepository.AddAsync(groupChatRoom);

            return groupChatRoom.ToChatRoomSummary();
        }

        public async Task<ChatRoomSummary> AddUsersToChatAsync(Guid userId, Guid chatId,
            ChatRoomAddUsers chatRoomAddUsers)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId);
            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                throw new UserNotFoundException("User not found in chat room");
            }

            var users = await ValidateUsersExistenceAsync(chatRoomAddUsers.UserIds);

            var usersList = users.ToList();

            if (usersList.Any(u => u.Id == userId))
            {
                throw new ArgumentException("Cannot add self to chat room");
            }

            foreach (var contact in usersList)
            {
                if (!await IsUserContactAsync(userId, contact.Id))
                {
                    throw new ArgumentException("User is not a contact");
                }
            }

            chatRoom.UserList = chatRoom.UserList.Union(usersList).Distinct().ToList();

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

        private async Task<bool> IsUserContactAsync(Guid userId, Guid otherUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var otherUser = await _userRepository.GetByIdAsync(otherUserId);

            if (user is null || otherUser is null)
            {
                throw new UserNotFoundException("User not found");
            }

            var contact = await _contactRepository.GetByIdAsync(user.Id, otherUser.Id);
            return contact is not null;
        }
    }
}