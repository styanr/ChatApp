using ChatApp.Context;
using ChatApp.Entities;

namespace ChatApp.Repositories.Messages;

public class MessageRepository(ChatDbContext context) : Repository<Message>(context), IMessageRepository;