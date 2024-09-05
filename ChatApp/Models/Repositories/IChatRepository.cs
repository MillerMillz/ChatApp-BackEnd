namespace ChatApp.Models.Repositories
{
    public interface IChatRepository
    {
        public Chat Create(Chat chat);
        public Chat Delete(int id);
        public Chat Get(int id);
        public List<Chat> Get(string userId);
        public List<Chat> Get(string userId,string friendId);
    }
}
