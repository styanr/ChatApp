using ChatApp.Models;
using ChatApp.Models.Messages;

namespace ChatApp.Services.Messages;

public interface IMessageService
{
    Task<IEnumerable<MessageResponse>> GetMessagesAsync(Guid chatRoomId, Guid userId, PagedRequest request);
    Task<MessageResponse> CreateMessageAsync(Guid chatRoomId, Guid userId, MessageCreate message);
    Task<MessageResponse> EditMessageAsync(Guid messageId, Guid userId, MessageUpdate message);
    Task<(Guid, Guid)> DeleteMessageAsync(Guid messageId, Guid userId);
}