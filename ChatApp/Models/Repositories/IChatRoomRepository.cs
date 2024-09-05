namespace ChatApp.Models.Repositories
{
    public interface IChatRoomRepository
    {
        public ChatRoom GetChatRoom(int id);
        public List<ChatRoom> ListChatRooms();
        public List<ChatRoom> ListChatRooms(List<int> iDs);
        public ChatRoom AddChatRoom(ChatRoom chatRoom);
        public ChatRoom DeleteChatRoom(int id);
        public ChatRoom EditChatRoom(ChatRoom chatRoom);
    }
}
