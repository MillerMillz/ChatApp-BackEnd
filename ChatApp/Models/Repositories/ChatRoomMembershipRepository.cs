using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class ChatRoomMembershipRepository:IChatRoomMembershipRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ChatRoomMembershipRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public ChatRoomMembership AddChatRoomMembership(ChatRoomMembership chatRoomMembership)
        {
            dbContext.ChatRoomMemberships.Add(chatRoomMembership);
            dbContext.SaveChanges();
            return chatRoomMembership;
        }

        public ChatRoomMembership DeleteChatRoomMembership(int id)
        {
            ChatRoomMembership membership = dbContext.ChatRoomMemberships.Find(id);

            dbContext.ChatRoomMemberships.Remove(membership);
            dbContext.SaveChanges();
            return membership;
        }

        public ChatRoomMembership GetChatRoomMembership(int id)
        {
            return dbContext.ChatRoomMemberships.Find(id);
        }

        public List<ChatRoomMembership> ListChatRoomMemberships(string id)
        {
            return dbContext.ChatRoomMemberships.Where(CRM=>CRM.UserId.Equals(id)).ToList();
        }
        public List<ChatRoomMembership> ListChatRoomMemberships(int id)
        {
            return dbContext.ChatRoomMemberships.Where(CRM => CRM.RoomId==id).ToList();
        }
    }
}
