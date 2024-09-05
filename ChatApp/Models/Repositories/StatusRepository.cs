using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class StatusRepository:IStatusRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StatusRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public Status AddStatus(Status status)
        {
            status.Time = DateTime.Now;
           dbContext.Statuses.Add(status);
            dbContext.SaveChanges();
            return status;
        }

        public Status DeleteStatus(int id)
        {
            Status tobeREMOVED = dbContext.Statuses.Find(id);

            dbContext.Statuses.Remove(tobeREMOVED);
            dbContext.SaveChanges();
            return tobeREMOVED;  
        }

        public Status GetStatus(int id)
        {
            return dbContext.Statuses.Find(id);

        }

        public List<Status> ListStatuses(List<string> friendsIds)
        {
            return dbContext.Statuses.Where(st => friendsIds.Contains(st.UserId))
                                     .OrderBy(e => e.Time)
                                     .ToList();
           
        }
    }
}
