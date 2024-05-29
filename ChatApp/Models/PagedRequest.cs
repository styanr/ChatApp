namespace ChatApp.Models;

public record PagedRequest(int Page = 1, int PageSize = 15);