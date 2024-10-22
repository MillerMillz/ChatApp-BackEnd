using ChatApp.Models;
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
    public class MembershipController : ControllerBase
    {
        private readonly IChatRoomMembershipRepository membershipRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRoomChatRepository roomChatRepository;

        public MembershipController(IChatRoomMembershipRepository _membershipRepository, UserManager<ApplicationUser> _userManager,IRoomChatRepository roomChatRepository)
        {
            membershipRepository = _membershipRepository;
            userManager = _userManager;
            this.roomChatRepository = roomChatRepository;
        }

        [HttpGet]
        [Route("Members/{id}/{isNew}")]
        public async Task<APIResponse<List<ApplicationUser>>> GetMembers(int id,bool isNew)
        {
            APIResponse<List<ApplicationUser>> response = new APIResponse<List<ApplicationUser>>();
            try
            {
                if(isNew)
                {
                     response.Response= await GetUsers(membershipRepository.ListChatRoomMemberships(id));
                }
                else
                {
                    Roomchat RC = roomChatRepository.Get(id);
                    response.Response = await GetUsers(membershipRepository.ListChatRoomMemberships(RC.RoomID));
                }
               
               
                
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
               
            }
            return response;
        }
        public async Task<List<ApplicationUser>> GetUsers(List<ChatRoomMembership> members)
        {
            List<ApplicationUser> users = new();
            foreach(ChatRoomMembership C in members)
            {
                users.Add(await userManager.FindByIdAsync(C.UserId));
            }
            return users;
        }
        [HttpGet]

        public ActionResult<APIResponse<List<ChatRoomMembership>>> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            APIResponse<List<ChatRoomMembership>> response = new APIResponse<List<ChatRoomMembership>>();
            try
            {
                response.Response = membershipRepository.ListChatRoomMemberships(userId);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpGet("{id}")]
        public ActionResult<APIResponse<ChatRoomMembership>> Get(int id)
        {
            APIResponse<ChatRoomMembership> response = new APIResponse<ChatRoomMembership>();
            try
            {
                response.Response = membershipRepository.GetChatRoomMembership(id);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }


        [HttpPost]
        public ActionResult<APIResponse<ChatRoomMembership>> Post([FromBody]ChatRoomMembership membership)
        {
            APIResponse<ChatRoomMembership> response = new APIResponse<ChatRoomMembership>();
            try
            {
                membership.Time = DateTime.Now;
                response.Response = membershipRepository.AddChatRoomMembership(membership);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
      

     
        [HttpDelete("{id}")]
        public ActionResult<APIResponse<ChatRoomMembership>> Delete(int id)
        {
            APIResponse<ChatRoomMembership> response = new APIResponse<ChatRoomMembership>();
            try
            {
                response.Response = membershipRepository.DeleteChatRoomMembership(id);
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
