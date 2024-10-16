using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class RoomChatRepository : IRoomChatRepository
    {
        private readonly ApplicationDbContext dbContext;

        public RoomChatRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public Roomchat Add(Roomchat roomchat)
        {
            dbContext.Roomchats.Add(roomchat);
            dbContext.SaveChanges();
            return roomchat;
        }

        public Roomchat Delete(int Id)
        {
            Roomchat roomchat = dbContext.Roomchats.Find(Id);

            dbContext.Roomchats.Remove(roomchat);
            dbContext.SaveChanges();
            return roomchat;
        }

        public Roomchat Edit(Roomchat roomchat)
        {
            dbContext.Roomchats.Update(roomchat);
            dbContext.SaveChanges();
            return roomchat;
        }

        public Roomchat Get(int id)
        {
            return dbContext.Roomchats.Find(id);
        }

        public List<Roomchat> Get(string OwnerId)
        {
            return dbContext.Roomchats.Where(RC => RC.OwnerId == OwnerId).ToList();
        }

        public Roomchat Get(int id, string OwnerId)
        {
            return dbContext.Roomchats.FirstOrDefault(RM => RM.RoomID == id & RM.OwnerId == OwnerId);
        }
    }
}
