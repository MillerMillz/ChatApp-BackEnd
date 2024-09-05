using ChatApp.Models;
using ChatApp.Models.DisplayModels;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("api/V1/[controller]")]
    [ApiController]
    public class RoomChatController : ControllerBase
    {
        private readonly IRoomChatRepository roomChatRepository;
        private readonly IMessageRepositiory messageRepositiory;
        private readonly IChatRoomRepository chatRoomRepository;

        public RoomChatController(IRoomChatRepository _roomChatRepository,IMessageRepositiory _messageRepositiory,IChatRoomRepository _chatRoomRepository)
        {
            roomChatRepository = _roomChatRepository;
            messageRepositiory = _messageRepositiory;
            chatRoomRepository = _chatRoomRepository;
        }

        // GET: api/<RoomChatController>
        [HttpGet]
        public ActionResult<APIResponse<List<RoomChatDisplaymodel>>> Get()
        {
            string userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            APIResponse<List<RoomChatDisplaymodel>> response = new();
            try
            {
                response.Response = convertToDisplayModel(roomChatRepository.Get(userId));
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }

        // GET api/<RoomChatController>/5
        [HttpGet("{id}")]
        public ActionResult<APIResponse<RoomChatDisplaymodel>> Get(int id)
        {
            APIResponse<RoomChatDisplaymodel> response = new();
            try
            {
                Roomchat model = roomChatRepository.Get(id);
                ChatRoom room = chatRoomRepository.GetChatRoom(model.RoomID);
                response.Response = new RoomChatDisplaymodel()
                {
                    Id = model.Id,
                    ChatRoom = new ChatRoomDisplayModel.Response() { Id =room.Id,
                    AdminId=room.AdminId,
                    Bio=room.Bio,
                    Name=room.Name,
                    FilePath=room.FilePath,
                    Image = room.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase,room.FilePath)
                    } ,
                    LastMessage = messageRepositiory.Get(model.LastMessageID)
                };
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }

        private List<RoomChatDisplaymodel> convertToDisplayModel(List<Roomchat> roomchats)
        {
            return roomchats.Select((RC)=> {
                ChatRoom room = chatRoomRepository.GetChatRoom(RC.RoomID);
                return new RoomChatDisplaymodel() { Id = RC.Id, ChatRoom =  new ChatRoomDisplayModel.Response()
                {
                    Id=room.Id,AdminId=room.AdminId,Bio=room.Bio,Name=room.Name,FilePath=room.FilePath,Image= room.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, room.FilePath)
                }, LastMessage = messageRepositiory.Get(RC.LastMessageID) }; }).OrderBy(rc=>rc.LastMessage.Time).ToList();
        }

        // POST api/<RoomChatController>
        [HttpPost]
        public ActionResult<APIResponse<Roomchat>> Post([FromBody] Roomchat roomchat)
        {
            APIResponse<Roomchat> response = new();
            try
            {
                response.Response = roomChatRepository.Add(roomchat);
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);

            }
            return response; 
        }

        // PUT api/<RoomChatController>/5
        [HttpPut]
        public ActionResult<APIResponse<Roomchat>> Put([FromBody] Roomchat roomchat)
        {
            APIResponse<Roomchat> response = new();
            try
            {
                response.Response = roomChatRepository.Edit(roomchat);
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);

            }
            return response;
        }

        // DELETE api/<RoomChatController>/5
        [HttpDelete("{id}")]
        public ActionResult<APIResponse<Roomchat>> Delete(int id)
        {
            APIResponse<Roomchat> response = new();
            try
            {
                response.Response = roomChatRepository.Delete(id);
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);

            }
            return response;
        }
    }
}
