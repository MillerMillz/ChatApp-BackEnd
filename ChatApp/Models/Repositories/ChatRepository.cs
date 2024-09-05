using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext dbcontext;

        public ChatRepository(ApplicationDbContext _dbcontext)
        {
            dbcontext = _dbcontext;
        }
        public Chat Create(Chat chat)
        {
            dbcontext.Chats.Add(chat);
            dbcontext.SaveChanges();

            return chat;
        }

        public Chat Delete(int id)
        {
            Chat chat = dbcontext.Chats.Find(id);

            dbcontext.Chats.Remove(chat);
            dbcontext.SaveChanges();

            return chat;
        }

        public Chat Get(int id)
        {
            return dbcontext.Chats.Find(id);
        }

        public List<Chat> Get(string userId)
        {
            return dbcontext.Chats.Where(C => C.ownerId == userId).ToList(); 
        }

        public List<Chat> Get(string userId, string friendId)
        {
            return dbcontext.Chats.Where(C => C.ownerId == userId && C.FriendId==friendId).ToList();
        }
    }
}
