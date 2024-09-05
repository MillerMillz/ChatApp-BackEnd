using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly ApplicationDbContext dbContext;

        public FriendshipRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public Friendship AddFriendship(Friendship friendship)
        {
            dbContext.Friendships.Add(friendship);
            dbContext.SaveChanges();
            return friendship;
        }

       
        public Friendship DeleteFriendShip(int id)
        {
            Friendship friendship = dbContext.Friendships.Find(id);
            dbContext.Friendships.Remove(friendship);
            dbContext.SaveChanges();
            return friendship;
   
        }

        public Friendship EditFriendship(Friendship friendship,string id)
        {
            if(friendship.Status=="Blocked")
            {
                dbContext.Blocks.Add(new Block() { FriendshipId = friendship.Id, BlockerId = id });
            }
            if (friendship.Status == "Active")
            {
                Block remove = dbContext.Blocks.FirstOrDefault(B=>B.FriendshipId==friendship.Id);
                dbContext.Blocks.Remove(remove);
            }
            dbContext.Friendships.Update(friendship);
            dbContext.SaveChanges();
            return friendship;
        }

        public Friendship EditFriendship(Friendship friendship)
        {
           
            dbContext.Friendships.Update(friendship);
            dbContext.SaveChanges();
            return friendship;
        }

        

        public Block GetBlock(int id)
        {
            return dbContext.Blocks.FirstOrDefault(B => B.FriendshipId == id);
        }

        public Friendship GetFriendship(int id)
        {
            return dbContext.Friendships.Find(id);
        }
        public List<string> FriendsIdz(string id)
        {
            List<string> ret = new List<string>();
            foreach(Friendship fr in dbContext.Friendships)
            {
                if(fr.User1ID==id||fr.User2ID==id)
                {
                    ret.Add(fr.User1ID == id ? fr.User2ID : fr.User1ID);
                }
            }
            return ret;
        }
        public List<Friendship> ListFriendShips(string id)
        {
          return dbContext.Friendships.Where(fr => fr.User1ID == id || fr.User2ID==id).ToList();
           
        }
    }
}
