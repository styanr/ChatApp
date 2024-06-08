namespace ChatApp.Exceptions;

public class ContactNotFoundException(Guid id) : NotFoundException($"Contact with id {id} was not found");