namespace ChatApp.Models.Repositories
{
    public interface IStatusViewRepository
    {
        public StatusView AddStatusView(StatusView statusView);
        List<StatusView> GetAllStatusViews(int id);
    }
}
