namespace ChatApp.Models.DisplayModels
{
    public class ChatDisplayModel
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public Message LastMessage { get; set; }
        public int FriendshipId { get; set; }
        public int UnreadMessages { get; set; }
    }
}
