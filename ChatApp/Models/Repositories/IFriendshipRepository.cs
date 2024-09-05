namespace ChatApp.Models.Repositories
{
    public interface IFriendshipRepository
    {
        public Friendship GetFriendship(int id);
        public Friendship AddFriendship(Friendship friendship);
        public Friendship EditFriendship(Friendship friendship);
        public Friendship EditFriendship(Friendship friendship,string id);
        public Friendship DeleteFriendShip(int id);
        public List<Friendship> ListFriendShips(string id);
        public List<string> FriendsIdz(string id);
        public Block GetBlock(int id);
    }
}
