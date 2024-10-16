using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Models.DataAccessLayer
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole,string>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatRoomMembership> ChatRoomMemberships { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Roomchat> Roomchats { get; set; }
        public DbSet<RoomChatMessage> RoomChatMessages { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<StatusView> StatusViews { get; set; }



    }
}
