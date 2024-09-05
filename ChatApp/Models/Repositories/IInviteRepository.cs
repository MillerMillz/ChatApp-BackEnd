namespace ChatApp.Models.Repositories
{
    public interface IInviteRepository
    {
        public Invite AddInvite(Invite invite);
        public Invite DeleteInvite(int id);
        public Invite GetInvite(int id);
        public List<Invite> ListInvites(string id);
        public List<Invite> SentInvites(string id);
    }
}
