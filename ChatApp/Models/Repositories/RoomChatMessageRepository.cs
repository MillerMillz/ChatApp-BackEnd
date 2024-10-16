using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class RoomChatMessageRepository : IRoomChatMessagesRepository
    {
        private readonly ApplicationDbContext dbContext;

        public RoomChatMessageRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public RoomChatMessage Add(RoomChatMessage roomChatMessage)
        {
           dbContext.RoomChatMessages.Add(roomChatMessage);
            dbContext.SaveChanges();
            return roomChatMessage;
        }

        public RoomChatMessage Delete(int id)
        {
            RoomChatMessage rcm = dbContext.RoomChatMessages.Find(id);
            dbContext.RoomChatMessages.Remove(rcm);
            dbContext.SaveChanges();
            return rcm;
        }

        public RoomChatMessage Edit(RoomChatMessage roomChatMessage)
        {
            dbContext.RoomChatMessages.Update(roomChatMessage);
            dbContext.SaveChanges();
            return roomChatMessage;
        }

        public IQueryable<RoomChatMessage> Get(string userID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<RoomChatMessage> Get(int roomId)
        {
            return dbContext.RoomChatMessages.Where(rcm => rcm.RoomChatId == roomId);
        }
    }
}
