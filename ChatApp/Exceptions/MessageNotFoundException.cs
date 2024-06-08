namespace ChatApp.Exceptions;

public class MessageNotFoundException(Guid id) : NotFoundException($"Message with id {id} was not found");