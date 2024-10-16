using ChatApp.Models.DataAccessLayer;

namespace ChatApp.Models.Repositories
{
    public class MessageRepository:IMessageRepositiory
    {
        private readonly ApplicationDbContext dbContext;

        public MessageRepository(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public Message AddMessage(Message message, int id,Roomchat roomchat)
        {
            message.Time = DateTime.Now;
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();
            List<string> members = dbContext.ChatRoomMemberships.Where(M => M.RoomId == id).Select(c=>c.UserId).ToList();
            List<Roomchat> chats = dbContext.Roomchats.Where(C => members.Contains(C.OwnerId) && C.OwnerId!=roomchat.OwnerId && C.RoomID==id).Select(X =>  new Roomchat() { Id=X.Id,OwnerId=X.OwnerId,RoomID=id,LastMessageID=message.Id }).ToList();
            roomchat.LastMessageID = message.Id;
            if(roomchat.Id != 0)
            { dbContext.Roomchats.Update(roomchat); }
               
            dbContext.Roomchats.UpdateRange(chats);
            dbContext.SaveChanges();
            List<Roomchat> newChats = members.Where(c => c!=roomchat.OwnerId && !chats.Select(s => s.OwnerId).Contains(c)).Select(s => new Roomchat() { LastMessageID = message.Id, OwnerId = s, RoomID = id }).ToList();
            if(roomchat.Id==0)
            { newChats.Add(roomchat); }
            dbContext.Roomchats.AddRange(newChats);
            dbContext.SaveChanges();
            List<Roomchat> allChats = chats.Concat(newChats).ToList();
            allChats.Add(roomchat);
            List<RoomChatMessage> mess = allChats.Select(c => new RoomChatMessage() { MessageId = message.Id, RoomChatId = c.Id }).ToList();
            dbContext.RoomChatMessages.AddRange(mess);
            dbContext.SaveChanges();
            return message;


            

        }
        public Message AddMessage(Message message,Chat chat)
        {
            message.Time = DateTime.Now;
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();
            List<Chat> chats = addChats(chat,message.Id);
            if (chats.Any())
            {
               if(addChatMessage(chats,message.Id))
                {
                    dbContext.SaveChanges();
                }
            }
            return message;
        }
       
        private bool addChatMessage(List<Chat> chats, int messageIs)
        {
            try
            {
                List<ChatMessage> chatMessages = chats.Select(C => new ChatMessage() { ChatId = C.Id, MessageId = messageIs }).ToList();
                dbContext.ChatMessages.AddRange(chatMessages);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
         private List<Chat> addChats(Chat chat, int messageId)
        {

            try
            {
                Chat userChat = dbContext.Chats.FirstOrDefault(C => C.ownerId == chat.ownerId && C.FriendId == chat.FriendId);

                Chat friendChat = dbContext.Chats.FirstOrDefault(C => C.ownerId == chat.FriendId && C.FriendId == chat.ownerId);

                if (userChat==null)
                {
                    userChat = new Chat()
                    {
                        ownerId = chat.ownerId,
                        FriendId = chat.FriendId,
                        FriendshipId=chat.FriendshipId,
                        LastMessageID = messageId
                    };
                    dbContext.Chats.Add(userChat);
                }
                else
                {
                    userChat.LastMessageID = messageId;
                    dbContext.Chats.Update(userChat);
                }
                if (friendChat == null)
                {
                    friendChat = new Chat()
                    {
                        ownerId = chat.FriendId,
                        FriendId = chat.ownerId,
                        FriendshipId = chat.FriendshipId,
                        LastMessageID = messageId
                    };
                    dbContext.Chats.Add(friendChat);
                }
                else
                {
                    friendChat.LastMessageID = messageId;
                    dbContext.Chats.Update(friendChat);
                }
                dbContext.SaveChanges();
                return new List<Chat>() { userChat,friendChat};
            }
            catch(Exception e)
            {
                return new List<Chat>();
            }

           
        }

        public List<Message> DeleteChat(List<int> idz)
        {
            List<Message> messages = dbContext.Messages.Where(mes => idz.Contains(mes.Id))
                                                       .ToList();
            dbContext.Messages.RemoveRange(messages);
            dbContext.SaveChanges();
            return messages;
        }

        public Message DeleteMessage(int id)
        {
            Message message = dbContext.Messages.Find(id);
          
                dbContext.Messages.Remove(message);
                dbContext.SaveChanges();
           
            return message;
        }

        public Message Get(int id)
        {
            return dbContext.Messages.Find(id);
        }

        public List<Message> GetMessages(List<int> idz)
        {
            List<Message> ret =dbContext.Messages.Where(mes => idz.Contains(mes.Id))
                                     .OrderBy(m=>m.Time)
                                     .ToList();

            return ret;
        }

        public Message Edit(Message message)
        {
            dbContext.Messages.Update(message);
            dbContext.SaveChanges();
            return message;
        }

        public List<Message> Edit(List<Message> messages)
        {
            dbContext.Messages.UpdateRange(messages);
            dbContext.SaveChanges();
            return messages;
        }
    }
}
