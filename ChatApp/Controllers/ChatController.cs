using ChatApp.Models;
using ChatApp.Models.DisplayModels;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("api/V1/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository chatRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMessageRepositiory messageRepository;
        private readonly IChatMessageRepository chatMessageRepository;

        public ChatController(IChatRepository _chatRepository, UserManager<ApplicationUser> _userManager,IMessageRepositiory _messageRepository,IChatMessageRepository chatMessageRepository)
        {
            chatRepository = _chatRepository;
            userManager = _userManager;
            messageRepository = _messageRepository;
            this.chatMessageRepository = chatMessageRepository;
        }

        [HttpGet]
        public async Task<APIResponse<List<ChatDisplayModel>>> Get()
        {
            APIResponse<List<ChatDisplayModel>> response = new APIResponse<List<ChatDisplayModel>>();
            try
            {
                
                string userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value; 
                response.Response = await convertToDiplayModel(chatRepository.Get(userId));
                return response;
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
                return response;
            }
        }
        private  async Task<List<ChatDisplayModel>> convertToDiplayModel(List<Chat> chats)
        {
            ChatDisplayModel dip;
            List<ChatDisplayModel> ret = new List<ChatDisplayModel>();
            foreach (Chat c in chats)
            {
                dip = new ChatDisplayModel()
                {
                    User = await userManager.FindByIdAsync(c.FriendId),
                    LastMessage = messageRepository.Get(c.LastMessageID),
                    Id = c.Id,
                    FriendshipId=c.FriendshipId,
                    UnreadMessages = messageRepository.GetMessages(chatMessageRepository.GetChatMessagesByChatId(c.Id).Select(CM => CM.MessageId).ToList()).Where(M => M.Viewed == false).Count()
                };
                dip.User.Image = dip.User.FilePath=="No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, dip.User.FilePath);
                ret.Add(dip);
            }
            return ret.OrderByDescending(CDM=>CDM.LastMessage.Time).ToList();
        }


        [HttpGet("{id}")]
        public async Task<APIResponse<ChatDisplayModel>> Get(int id)
        {
            APIResponse<ChatDisplayModel> response = new APIResponse<ChatDisplayModel>();
            try
            {
                Chat chat = chatRepository.Get(id);
                response.Response = new ChatDisplayModel()
                {
                    Id = chat.Id,
                    LastMessage = messageRepository.Get(chat.LastMessageID),
                    User = await userManager.FindByIdAsync(chat.FriendId),
                    FriendshipId = chat.FriendshipId,
                    UnreadMessages = messageRepository.GetMessages(chatMessageRepository.GetChatMessagesByChatId(chat.Id).Select(CM => CM.MessageId).ToList()).Where(M => M.Viewed == false).Count()
                };
                response.Response.User.Image= response.Response.User.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, response.Response.User.FilePath);
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }

       
        [HttpPost]
        public ActionResult<APIResponse<List<Chat>>> Post([FromBody] Chat chat)
        {
            APIResponse<List<Chat>> response = new APIResponse<List<Chat>>();
            try
            {
                List<Chat> userChats = chatRepository.Get(chat.ownerId, chat.FriendId);
                List<Chat> friendChats = chatRepository.Get(chat.FriendId, chat.ownerId);
                Chat userChat = userChats.Count == 0 ? chatRepository.Create(new Chat() { LastMessageID = chat.LastMessageID, FriendId = chat.FriendId, ownerId = chat.ownerId }) : userChats[0];
                Chat friendChat = friendChats.Count == 0 ? chatRepository.Create(new Chat() { LastMessageID = chat.LastMessageID, FriendId = chat.ownerId, ownerId = chat.FriendId }) : friendChats[0];

                response.Response = new List<Chat>() { userChat, friendChat };
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }

       
        [HttpDelete("{id}")]
        public ActionResult<APIResponse<Chat>> Delete(int id)
        {
            APIResponse<Chat> response = new APIResponse<Chat>();
            try
            {
                response.Response = chatRepository.Delete(id);
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
