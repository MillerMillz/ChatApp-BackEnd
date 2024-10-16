namespace ChatApp.Models.Repositories
{
    public interface IRoomChatRepository
    {
        Roomchat Get(int id);
        Roomchat Get(int id,string OwnerId);
        List<Roomchat> Get(string OwnerId);
        Roomchat Add(Roomchat roomchat);
        Roomchat Edit(Roomchat roomchat);
        Roomchat Delete(int Id);
    }
}
