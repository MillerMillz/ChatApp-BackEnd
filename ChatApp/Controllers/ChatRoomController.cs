using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Models.DisplayModels;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("api/V1/[controller]")]
    [ApiController]
    public class ChatRoomController : ControllerBase
    {
        private readonly IChatRoomRepository chatRoomRepository;
        private readonly IChatRoomMembershipRepository roomMembershipRepository;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IHubContext<ChatHub> hubContext;

        public ChatRoomController(IChatRoomRepository _chatRoomRepository, IChatRoomMembershipRepository _roomMembershipRepository,IWebHostEnvironment _hostEnvironment,IHubContext<ChatHub> hubContext)
        {
            chatRoomRepository = _chatRoomRepository;
            roomMembershipRepository = _roomMembershipRepository;
            hostEnvironment = _hostEnvironment;
            this.hubContext = hubContext;
        }
      
        [HttpGet]
        public ActionResult<APIResponse<List<ChatRoomDisplayModel.Response>>> Get()
        {
            APIResponse<List<ChatRoomDisplayModel.Response>> response = new APIResponse<List<ChatRoomDisplayModel.Response>>();
            try
            {
                response.Response = chatRoomRepository.ListChatRooms().Select((CR) => {
                    return new ChatRoomDisplayModel.Response() { Id=CR.Id,
                    AdminId=CR.AdminId,
                    Bio=CR.Bio,
                    Name=CR.Name,
                    FilePath=CR.FilePath,
                    Image=CR.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, CR.FilePath)
                    };
                }).ToList();
                return response;
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpGet("{id}")]
        public ActionResult<APIResponse<ChatRoomDisplayModel.Response>> Get(int id)
        {
            APIResponse<ChatRoomDisplayModel.Response> response = new APIResponse<ChatRoomDisplayModel.Response>();
            try
            {  
                ChatRoom room = chatRoomRepository.GetChatRoom(id);
                response.Response = new()
                {
                    Id = room.Id,
                    AdminId = room.AdminId,
                    Bio = room.Bio,
                    Name = room.Name,
                    FilePath = room.FilePath,
                    Image = room.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, room.FilePath)
                };
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }

       
        [HttpGet]
        [Route("Joined")]
        public ActionResult<APIResponse<List<ChatRoomDisplayModel.Response>>> JoinedRooms()
        {
           
            APIResponse<List<ChatRoomDisplayModel.Response>> response = new APIResponse<List<ChatRoomDisplayModel.Response>>();
            try
            {
                string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                List<int> iDs = roomMembershipRepository.ListChatRoomMemberships(id).Select(CRM => CRM.RoomId).ToList();
                response.Response = chatRoomRepository.ListChatRooms(iDs).Select((CR) => {
                    return new ChatRoomDisplayModel.Response()
                    {
                        Id = CR.Id,
                        AdminId = CR.AdminId,
                        Bio = CR.Bio,
                        Name = CR.Name,
                        FilePath = CR.FilePath,
                        Image = CR.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, CR.FilePath)
                    };
                }).ToList();
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }

        
        [HttpPost]
        public ActionResult<APIResponse<ChatRoom>> Post([FromForm] ChatRoomDisplayModel.Request model)
        {
            APIResponse<ChatRoom> response = new APIResponse<ChatRoom>();
            try
            {
                string userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                ChatRoom chatRoom = chatRoomRepository.AddChatRoom(new ChatRoom() {Name=model.Name,AdminId=model.AdminId,Bio=model.Bio,FilePath=saveImage(model.Image) });
                response.Response = chatRoom;
                ChatRoomMembership membership = roomMembershipRepository.AddChatRoomMembership(new ChatRoomMembership() { RoomId = chatRoom.Id, UserId = userId, Time = DateTime.Now });
                hubContext.Clients.All.SendAsync("RecieveMessage", DateTime.Now.ToString(), "Rooms");
                return response;
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
                return response;
            }
        }
        private string saveImage(IFormFile file)
        {

            bool filePathExists = System.IO.Directory.Exists(Path.Combine(hostEnvironment.WebRootPath, $"DisplayPictures"));
            if (!filePathExists)
            {
                System.IO.Directory.CreateDirectory(Path.Combine(hostEnvironment.WebRootPath, $"DisplayPictures"));
            }
            string? filePath = null, uniqueName = null, returnString = "No file";

            if (file != null)
            {
                string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, $"DisplayPictures");
                uniqueName = $"{Guid.NewGuid().ToString()}_{file.FileName}";
                filePath = Path.Combine(uploadFolder, uniqueName);
                returnString = Path.Combine($"DisplayPictures", uniqueName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            return returnString;
        }

        [HttpPut]
        public ActionResult<APIResponse<ChatRoom>> Put( [FromForm] ChatRoom chatRoom)
        {
            APIResponse<ChatRoom> response = new APIResponse<ChatRoom>();
            try
            {
                response.Response = chatRoomRepository.EditChatRoom(chatRoom);
                hubContext.Clients.All.SendAsync("RecieveMessage", DateTime.Now.ToString(), "Rooms");
                return response;
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
                return response;
            }
        }

        
        [HttpDelete("{id}")]
        public ActionResult<APIResponse<ChatRoom>> Delete(int id)
        {
            APIResponse<ChatRoom> response = new APIResponse<ChatRoom>();
            try
            {
                response.Response = chatRoomRepository.DeleteChatRoom(id);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
    }
}
