using System.Security.Claims;
using ChatApp.Entities;
using ChatApp.Exceptions;
using ChatApp.Hubs;
using ChatApp.Managers;
using ChatApp.Mapping;
using ChatApp.Models.ChatRooms;
using ChatApp.Models.Messages;
using ChatApp.Repositories.ChatRooms;
using ChatApp.Repositories.Contacts;
using ChatApp.Repositories.Messages;
using ChatApp.Repositories.Users;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Services.ChatRooms
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRoomService(IChatRoomRepository chatRoomRepository, IUserRepository userRepository,
            IContactRepository contactRepository, IHubContext<ChatHub> hubContext)

        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _contactRepository = contactRepository;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<ChatRoomSummary>> GetAllAsync(Guid userId)
        {
            var directChatRooms = (await _chatRoomRepository.GetDirectChatRoomsAsync(userId)).ToList();
            var groupChatRooms = (await _chatRoomRepository.GetGroupChatRoomsAsync(userId)).ToList();

            var chatRoomSummaries = directChatRooms.Select(dc => dc.ToChatRoomSummary(userId))
                .Concat(groupChatRooms.Select(gc => gc.ToChatRoomSummary()));

            return chatRoomSummaries;
        }

        public async Task<ChatRoomDetails> GetChatAsync(Guid userId, Guid chatId)
        {
            var chatRoom = await _chatRoomRepository.GetChatRoomAsync(chatId);
            if (chatRoom is null)
            {
                throw new ChatRoomNotFoundException(chatId);
            }

            if (chatRoom.Users.All(u => u.Id != userId))
            {
                throw new UnauthorizedAccessException();
            }

            return chatRoom.ToChatRoomDetails(userId);
        }

        public async Task<ChatRoomDetails> CreateDirectChatAsync(Guid userId, Guid otherUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var otherUser = await _userRepository.GetByIdAsync(otherUserId);

            if (user is null || otherUser is null)
            {
                throw new UserNotFoundException(userId);
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
                throw new DirectChatRoomAlreadyExists();
            }

            var directChatRoom = new DirectChatRoom
            {
                User1 = user,
                User2 = otherUser
            };

            await _chatRoomRepository.AddAsync(directChatRoom);

            return directChatRoom.ToChatRoomDetails(userId);
        }

        public async Task<ChatRoomDetails> CreateGroupChatAsync(Guid userId, GroupChatRoomCreate groupChatRoomCreate)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var groupChatRoom = new GroupChatRoom
            {
                Name = groupChatRoomCreate.Name ?? throw new ArgumentNullException(nameof(groupChatRoomCreate.Name)),
                UserList = [user]
            };

            await _chatRoomRepository.AddAsync(groupChatRoom);

            return groupChatRoom.ToChatRoomDetails();
        }

        public async Task<ChatRoomDetails> AddUsersToChatAsync(Guid userId, Guid chatId,
            ChatRoomAddUsers chatRoomAddUsers)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId, userId);
            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                throw new UserNotFoundException(userId);
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

            return chatRoom.ToChatRoomDetails();
        }

        public async Task<ChatRoomDetails> RemoveUserFromChatAsync(Guid userId, Guid chatId, Guid deleteUserId)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId, userId);
            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            var userToDelete = chatRoom.UserList.FirstOrDefault(u => u.Id == deleteUserId);
            
            if (userToDelete is null)
            {
                throw new UserNotFoundException(deleteUserId);
            }
            
            if (userToDelete.Id == userId)
            {
                throw new ArgumentException("Cannot remove self from chat room");
            }

            chatRoom.UserList.Remove(user);

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomDetails();
        }
        
        public async Task<ChatRoomDetails?> LeaveChatAsync(Guid userId, Guid chatId)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId, userId);
            
            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            if (chatRoom.UserList.Count == 1)
            {
                await _chatRoomRepository.DeleteByIdAsync(chatRoom.Id);
                return null;
            }

            chatRoom.UserList.Remove(user);

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomDetails();
        }

        public async Task<ChatRoomDetails> UpdateGroupChatAsync(Guid userId, Guid chatId, ChatRoomUpdate chatRoomUpdate)
        {
            var chatRoom = await GetGroupChatRoomWithUserValidationAsync(chatId, userId);

            var user = chatRoom.UserList.FirstOrDefault(u => u.Id == userId);
            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            chatRoom.Name = chatRoomUpdate.Name ?? chatRoom.Name;
            chatRoom.Description = chatRoomUpdate.Description ?? chatRoom.Description;
            chatRoom.PictureId = chatRoomUpdate.PictureId ?? chatRoom.PictureId;

            await _chatRoomRepository.UpdateAsync(chatRoom);

            return chatRoom.ToChatRoomDetails();
        }

        public async Task<bool> IsUserInChatAsync(Guid userId, Guid chatId)
        {
            var chatRoom = await _chatRoomRepository.GetByIdAsync(chatId);
            if (chatRoom is null)
            {
                throw new ChatRoomNotFoundException(chatId);
            }

            return chatRoom.Users.Any(u => u.Id == userId);
        }

        private async Task<GroupChatRoom> GetGroupChatRoomWithUserValidationAsync(Guid chatId, Guid userId)
        {
            var chatRoom = await _chatRoomRepository.GetGroupChatRoomAsync(chatId);

            if (chatRoom is null)
            {
                throw new ChatRoomNotFoundException(chatId);
            }

            if (chatRoom.UserList.All(u => u.Id != userId))
            {
                throw new UserNotFoundException(userId);
            }

            return chatRoom;
        }

        private async Task<IEnumerable<User>> ValidateUsersExistenceAsync(IEnumerable<Guid> userIds)
        {
            var userIdsList = userIds.ToList();

            var users = (await _userRepository.GetUsersByIdsAsync(userIdsList)).ToList();
            if (users.Count != userIdsList.Count)
            {
                throw new UserNotFoundException();
            }

            return users;
        }

        private async Task<bool> IsUserContactAsync(Guid userId, Guid otherUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var otherUser = await _userRepository.GetByIdAsync(otherUserId);

            if (user is null || otherUser is null)
            {
                throw new UserNotFoundException();
            }

            var contact = await _contactRepository.GetByIdAsync(user.Id, otherUser.Id);
            return contact is not null;
        }
    }
}