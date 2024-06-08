namespace ChatApp.Exceptions;


public class ChatRoomNotFoundException(Guid id) : NotFoundException($"Chat room with id {id} was not found");