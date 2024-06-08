namespace ChatApp.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid id) : base($"User with id {id} not found")
    {
    }

    public UserNotFoundException() : base("User not found")
    {
    }
}
