using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext dbcontext;
        private readonly IFriendshipRepository friendshipRepository;

        public ChatRepository(ApplicationDbContext _dbcontext,IFriendshipRepository friendshipRepository)
        {
            dbcontext = _dbcontext;
            this.friendshipRepository = friendshipRepository;
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

        public Chat Get(string userId, string friendId)
        {
            Friendship fr = dbcontext.Friendships.FirstOrDefault(F => (F.User1ID == userId && F.User2ID == friendId) || (F.User1ID == friendId && F.User2ID == userId));
            Chat chat= dbcontext.Chats.FirstOrDefault(C => C.ownerId == userId && C.FriendId==friendId);

            if(chat!=null && (chat.FriendshipId==0&&fr.Id!=0))
            {
                chat.FriendshipId = fr.Id;
                dbcontext.Chats.Update(chat);
                dbcontext.SaveChanges();
            }
            return chat;
        }
    }
}
