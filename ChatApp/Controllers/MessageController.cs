using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Models.DisplayModels;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("api/V1/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMessageRepositiory messageRepository;
        private readonly IChatMessageRepository chatMessageRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IRoomChatRepository roomChatRepository;
        private readonly IChatRoomRepository chatRoomRepository;
        private readonly IChatRepository chatRepository;
        private readonly IRoomChatMessagesRepository roomChatMessagesRepository;

        public MessageController(IMessageRepositiory _messageRepository,IChatMessageRepository _chatMessageRepository, UserManager<ApplicationUser> _userManager,IHubContext<ChatHub> hubContext,IRoomChatRepository roomChatRepository,IChatRoomRepository chatRoomRepository,IChatRepository chatRepository,IRoomChatMessagesRepository roomChatMessagesRepository)
        {
            messageRepository = _messageRepository;
            chatMessageRepository = _chatMessageRepository;
            userManager = _userManager;
            this.hubContext = hubContext;
            this.roomChatRepository = roomChatRepository;
            this.chatRoomRepository = chatRoomRepository;
            this.chatRepository = chatRepository;
            this.roomChatMessagesRepository = roomChatMessagesRepository;
        }
       
        [HttpGet("{id}")]
        public async Task<APIResponse<List<MessageDisplayModel>>> Get(int id)
        {
            APIResponse<List<MessageDisplayModel>> response = new APIResponse<List<MessageDisplayModel>>();
            try
            {
                List<int> idz = chatMessageRepository.GetChatMessagesByChatId(id).Select(CM => CM.MessageId).ToList();
                response.Response = await AddUser(messageRepository.GetMessages(idz));
                return response;
            }
            catch(Exception e)
            {
                
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpGet("RoomMessages/{id}")]
        public async Task<APIResponse<List<MessageDisplayModel>>> GetRoomMessages(int id)
        {
            APIResponse<List<MessageDisplayModel>> response = new APIResponse<List<MessageDisplayModel>>();
            try
            {
                List<int> idz = roomChatMessagesRepository.Get(id).Select(CM => CM.MessageId).ToList();
                response.Response = await AddUser(messageRepository.GetMessages(idz));
                return response;
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
                return response;
            }
        }

        private async Task<List<MessageDisplayModel>> AddUser(List<Message> messages)
        {
            List<MessageDisplayModel> list = new();
            MessageDisplayModel add;
            foreach(Message mes in messages)
            {
                add = new MessageDisplayModel()
                {
                    Id = mes.Id,
                    ReplyID = mes.ReplyID,
                    FilePath = mes.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, mes.FilePath),
                    TruePath = mes.FilePath,
                    MessageContent =mes.MessageContent,
                    MessageType=mes.MessageType,
                    Time=mes.Time,
                    Viewed=mes.Viewed,
                    User=await userManager.FindByIdAsync(mes.SenderID)
                };
                list.Add(add);
            }
            return(list);
        }
        [HttpPut("{id}/{isGroupChat}")]
        public async Task<APIResponse<List<Message>>> Put(int id,bool isGroupChat)
        {
          
            APIResponse<List<Message>> response = new();
            try
            {
                Chat chat = chatRepository.Get(id);
                List<int> idz = new List<int>();
                if(isGroupChat)
                {
                    idz = roomChatMessagesRepository.Get(id).Select(CM => CM.MessageId).ToList();
                }
                else
                {
                    idz = chatMessageRepository.GetChatMessagesByChatId(id).Select(CM => CM.MessageId).ToList();
                }
                List <Message> edit = messageRepository.GetMessages(idz).Where(M => M.Viewed == false).ToList();
               
                foreach(Message m in edit)
                {
                    m.Viewed = true;
                }
                response.Response = messageRepository.Edit(edit);
                if (isGroupChat)
                {
                   // await hubContext.Clients.Group(chat.FriendId).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Messages");
                }
                else
                {
                    await hubContext.Clients.User(chat.FriendId).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Messages");
                    await hubContext.Clients.User(chat.ownerId).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Messages");
                }

            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }
        [HttpPut]
        public ActionResult<APIResponse<Message>> Put([FromBody]Message message)
        {
            APIResponse<Message> response = new();
            try
            {
                response.Response = messageRepository.Edit(message);
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }
        [HttpPost("{id}/{isNew}")]
        public async Task<APIResponse<MessageDisplayModel>> Post(int id,bool isNew,[FromBody] Message message)
        {
            APIResponse<MessageDisplayModel> response = new();
            try
            {
                string userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                Roomchat roomchat;
                ChatRoom chatRoom;
                Message mes;
                if(isNew)
                { 
                    chatRoom = chatRoomRepository.GetChatRoom(id);
                    mes = messageRepository.AddMessage(message, chatRoom.Id, new Roomchat() { OwnerId=userId,RoomID=id});
                }
                else
                {
                    roomchat = roomChatRepository.Get(id);
                     chatRoom = chatRoomRepository.GetChatRoom(roomchat.RoomID);
                     mes = messageRepository.AddMessage(message, chatRoom.Id,roomchat);
                }
               
                response.Response = new MessageDisplayModel()
                {
                    Id = mes.Id,
                    MessageContent = mes.MessageContent,
                    MessageType = mes.MessageType,
                    Time = mes.Time,
                    ReplyID = mes.ReplyID,
                    FilePath = mes.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, mes.FilePath),
                    Viewed = mes.Viewed,
                    User = await userManager.FindByIdAsync(mes.SenderID),
                    TruePath = mes.FilePath
                };

             
                await hubContext.Clients.Group(chatRoom.Name).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Messages");
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }
        [HttpPost]
        public async Task<APIResponse<MessageDisplayModel>> Post([FromBody]MessageRequestModel model)
        {
           
            APIResponse<MessageDisplayModel> response = new APIResponse<MessageDisplayModel>();
            try
            {
                Message mes = messageRepository.AddMessage(model.Message,model.Chat);
                response.Response = new MessageDisplayModel()
                {
                    Id = mes.Id,
                    ReplyID = mes.ReplyID,
                    FilePath = mes.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, mes.FilePath),
                    MessageContent = mes.MessageContent,
                    MessageType = mes.MessageType,
                    Time = mes.Time,
                    Viewed = mes.Viewed,
                    User = await userManager.FindByIdAsync(mes.SenderID),
                    TruePath=mes.FilePath
                };
                await hubContext.Clients.User(model.Chat.FriendId).SendAsync("RecieveMessage", DateTime.Now.ToString(),"Messages");
                return response;
            }catch(Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpDelete("{id}")]
        public ActionResult<APIResponse<List<ChatMessage>>> Delete(int id)
        {
            APIResponse<List<ChatMessage>> response = new APIResponse<List<ChatMessage>>();
            try
            {
                
                response.Response = chatMessageRepository.ClearChat(id);
                chatRepository.Delete(id);
                return response;
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpDelete]
        [Route("remove/{id}")]
        public ActionResult<APIResponse<Message>> DeleteMessage(int id)
        {
            APIResponse<Message> response = new APIResponse<Message>();
            try
            {
                
                response.Response = messageRepository.DeleteMessage(id);
                return response;
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message+",item associated with id: " +id+" could not be found");
                return response;
            }
        }
    }
}
