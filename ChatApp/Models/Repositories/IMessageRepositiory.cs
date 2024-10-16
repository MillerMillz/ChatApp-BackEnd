namespace ChatApp.Models.Repositories
{
    public interface IMessageRepositiory
    {
        public List<Message> GetMessages(List<int> idz);
        Message Get(int id);
        public Message AddMessage(Message message, Chat chat);
        Message AddMessage(Message message, int id,Roomchat rooomchaat);
        public Message DeleteMessage(int id);
        public List<Message> DeleteChat(List<int> idz);
        Message Edit(Message message);
        List<Message> Edit(List<Message> messages);
    }
}
