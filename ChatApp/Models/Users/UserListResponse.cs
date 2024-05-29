namespace ChatApp.Models.Users;

public record UserListResponse(List<UserResponse> Users, int Page, int PageSize, int TotalCount, int TotalPages);