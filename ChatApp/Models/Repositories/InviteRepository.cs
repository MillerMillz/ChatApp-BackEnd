using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class InviteRepository:IInviteRepository
    {
        private readonly ApplicationDbContext dbContext;

        public InviteRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public Invite AddInvite(Invite invite)
        {
            dbContext.Invites.Add(invite);
            dbContext.SaveChanges();
            return invite;
        }

        public Invite DeleteInvite(int id)
        {
            Invite invite = dbContext.Invites.Find(id);

            dbContext.Invites.Remove(invite);
            dbContext.SaveChanges();
            return invite;
        }

        public Invite GetInvite(int id)
        {
            return dbContext.Invites.Find(id);
        }

        public List<Invite> ListInvites(string id)
        {
            return dbContext.Invites.Where(Inv => Inv.ReciepientID==id).ToList();
        }
        public List<Invite> SentInvites(string id)
        {
            return dbContext.Invites.Where(Inv => Inv.SenderId == id).ToList();
        }
    }
}
