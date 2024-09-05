namespace ChatApp.Models.Repositories
{
    public interface IRoomChatRepository
    {
        Roomchat Get(int id);
        List<Roomchat> Get(string OwnerId);
        Roomchat Add(Roomchat roomchat);
        Roomchat Edit(Roomchat roomchat);
        Roomchat Delete(int Id);
    }
}
