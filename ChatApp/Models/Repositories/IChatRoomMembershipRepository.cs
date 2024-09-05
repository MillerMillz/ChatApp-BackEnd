namespace ChatApp.Models.Repositories         
{
    public interface IChatRoomMembershipRepository
    {
        public ChatRoomMembership GetChatRoomMembership(int id);
        public List<ChatRoomMembership> ListChatRoomMemberships(string id);
        public List<ChatRoomMembership> ListChatRoomMemberships(int id);
        public ChatRoomMembership AddChatRoomMembership(ChatRoomMembership chatRoomMembership);
        public ChatRoomMembership DeleteChatRoomMembership(int  idz);
    }
}
