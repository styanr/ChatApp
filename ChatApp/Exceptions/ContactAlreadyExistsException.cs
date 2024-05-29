namespace ChatApp.Exceptions;

public class ContactAlreadyExistsException(string message) : Exception(message);