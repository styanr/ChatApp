namespace ChatApp.Exceptions;

public class UserNotAuthorException(Guid userId, Guid messageId) : Exception($"User with id {userId} is not the author of message with id {messageId}");