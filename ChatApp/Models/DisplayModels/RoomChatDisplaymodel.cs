namespace ChatApp.Models.DisplayModels
{
    public class RoomChatDisplaymodel
    {
        public int Id { get; set; }
        public ChatRoomDisplayModel.Response ChatRoom { get; set; }
        public Message LastMessage { get; set; }
    }
}
