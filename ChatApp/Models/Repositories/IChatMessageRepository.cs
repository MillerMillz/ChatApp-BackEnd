namespace ChatApp.Models.Repositories
{
    public interface IChatMessageRepository
    {
        ChatMessage Create(ChatMessage chatMessage);
        List<ChatMessage> Create(List<ChatMessage> chatMessages);
        List<ChatMessage> GetChatMessagesByChatId(int chatId);
        List<ChatMessage> GetChatMessagesByMessageId(int messageId);
        ChatMessage GetChatMessageById(int id);
        ChatMessage DeleteForMe((int,int) ids);
        List<ChatMessage> DeleteForAll(int messageId);
        List<ChatMessage> ClearChat(int chatID);
    }
}
