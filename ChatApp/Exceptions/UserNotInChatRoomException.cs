namespace ChatApp.Exceptions;

public class UserNotInChatRoomException(Guid userId, Guid chatRoomId) : Exception($"User with id {userId} is not in chat room with id {chatRoomId}");