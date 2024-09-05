using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class StatusViewRepository:IStatusViewRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StatusViewRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public StatusView AddStatusView(StatusView statusView)
        {
            statusView.Time = DateTime.Now;
            dbContext.StatusViews.Add(statusView);
            dbContext.SaveChanges();
            return statusView;
        }

        public List<StatusView> GetAllStatusViews(int id)
        {
            return dbContext.StatusViews.Where(sv=>sv.StatusId==id).ToList();
        }
    }
}
