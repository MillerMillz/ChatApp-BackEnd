using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class ChatRoomRepository:IChatRoomRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ChatRoomRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public ChatRoom AddChatRoom(ChatRoom chatRoom)
        {
            dbContext.ChatRooms.Add(chatRoom);
            dbContext.SaveChanges();
             return chatRoom;
        }

        public ChatRoom DeleteChatRoom(int id)
        {
            ChatRoom chatRoom = dbContext.ChatRooms.Find(id);
           
            dbContext.ChatRooms.Remove(chatRoom);
            dbContext.SaveChanges();
            return chatRoom;
        }

        public ChatRoom EditChatRoom(ChatRoom chatRoom)
        {
            dbContext.ChatRooms.Update(chatRoom);
            dbContext.SaveChanges();
            return chatRoom;
        }

        public ChatRoom GetChatRoom(int id)
        {
            ChatRoom ret = dbContext.ChatRooms.Find(id);
            return ret;
        }

        public List<ChatRoom> ListChatRooms()
        {
            return dbContext.ChatRooms.ToList();
        }

        public List<ChatRoom> ListChatRooms(List<int> iDs)
        {
            return dbContext.ChatRooms.Where(CR=>iDs.Contains(CR.Id))
                                      .ToList();
        }
    }
}
