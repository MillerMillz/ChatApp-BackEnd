using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class ChatMessageRepository:IChatMessageRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ChatMessageRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

       

        public ChatMessage Create(ChatMessage chatMessage)
        {
            dbContext.ChatMessages.Add(chatMessage);
            dbContext.SaveChanges();
            return chatMessage;
        }
        
        public List<ChatMessage> Create(List<ChatMessage> chatMessages)
        {
            dbContext.ChatMessages.AddRange(chatMessages);
            dbContext.SaveChanges();
            return chatMessages;
        }

        public List<ChatMessage> DeleteForAll(int messageId)
        {
            List<ChatMessage> chatMessages = dbContext.ChatMessages.Where(CM => CM.MessageId == messageId).ToList();
            dbContext.ChatMessages.RemoveRange(chatMessages);
            dbContext.SaveChanges();
            return chatMessages;
        }

        public ChatMessage DeleteForMe((int,int) ids)
        {
            ChatMessage chatMessage = (ChatMessage)dbContext.ChatMessages.Where(CM => CM.MessageId == ids.Item1 && CM.ChatId == ids.Item2);
            dbContext.ChatMessages.Remove(chatMessage);
            return chatMessage;
        }
        public List<ChatMessage> ClearChat(int chatID)
        {
            List<ChatMessage> chatMessages = dbContext.ChatMessages.Where(CM => CM.ChatId == chatID).ToList();
            dbContext.ChatMessages.RemoveRange(chatMessages);
            dbContext.SaveChanges();
            return chatMessages;
        }
        public ChatMessage GetChatMessageById(int id)
        {
            return dbContext.ChatMessages.Find(id);
        }


        public List<ChatMessage> GetChatMessagesByChatId(int chatId)
        {
            return dbContext.ChatMessages.Where(CM => CM.ChatId == chatId).ToList();
        }

        public List<ChatMessage> GetChatMessagesByMessageId(int messageId)
        {
            return dbContext.ChatMessages.Where(CM => CM.MessageId == messageId).ToList();
        }
    }
}
