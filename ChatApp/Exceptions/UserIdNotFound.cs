namespace ChatApp.Exceptions;

public class UserIdNotFound() : NotFoundException("User ID not found in claims");