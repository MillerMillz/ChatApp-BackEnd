namespace ChatApp.Models.Repositories
{
    public interface IStatusRepository
    {
        public List<Status> ListStatuses(List<string> friendsIds);
        public Status AddStatus(Status status);
        public Status DeleteStatus(int id);
        public Status GetStatus(int id);
    }
}
