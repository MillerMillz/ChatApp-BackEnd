using ChatApp.Models;
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
    public class MembershipController : ControllerBase
    {
        private readonly IChatRoomMembershipRepository membershipRepository;

        public MembershipController(IChatRoomMembershipRepository _membershipRepository)
        {
            membershipRepository = _membershipRepository;
        }

        [HttpGet]
        
        public ActionResult<APIResponse<List<ChatRoomMembership>>> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            APIResponse<List<ChatRoomMembership>> response = new APIResponse<List<ChatRoomMembership>>();
            try
            {
                response.Response= membershipRepository.ListChatRoomMemberships(userId);
                return response;
            }
            catch(Exception e)
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
