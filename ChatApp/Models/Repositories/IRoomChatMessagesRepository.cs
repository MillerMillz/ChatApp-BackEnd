namespace ChatApp.Models.Repositories
{
    public interface IRoomChatMessagesRepository
    {
        IQueryable<RoomChatMessage> Get(string userID);
        IQueryable<RoomChatMessage> Get(int roomId);
        RoomChatMessage Add(RoomChatMessage roomChatMessage);
        RoomChatMessage Edit(RoomChatMessage roomChatMessage);
        RoomChatMessage Delete(int id);

    }
}
