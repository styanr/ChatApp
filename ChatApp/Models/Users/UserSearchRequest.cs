namespace ChatApp.Models.Users;

public record UserSearchRequest(string? SearchTerm, int Page = 1, int PageSize = 15);
