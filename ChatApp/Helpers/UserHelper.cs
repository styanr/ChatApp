using System.Security.Claims;
using ChatApp.Exceptions;

namespace ChatApp.Helpers;

public static class UserHelper
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            throw new UserNotFoundException("User not found");
        }

        return Guid.Parse(userId);
    }
}