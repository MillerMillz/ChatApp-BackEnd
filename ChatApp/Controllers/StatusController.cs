using ChatApp.Models;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/V1/[controller]")]
    public class StatusController : Controller
    {
        private readonly IStatusRepository statusRepository;
        private readonly IFriendshipRepository friendshipRepository;

        public StatusController(IStatusRepository _statusRepository,IFriendshipRepository _friendshipRepository)
        {
            statusRepository = _statusRepository;
            friendshipRepository = _friendshipRepository;
        }

        [HttpGet("{id}")]
      
        public ActionResult<APIResponse<Status>> Get(int id)
        {
            APIResponse<Status> response = new APIResponse<Status>();
            try
            {
                response.Response = statusRepository.GetStatus(id);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpGet]
        public ActionResult<APIResponse<List<Status>>> Get()
        {
           
            APIResponse<List<Status>> response = new APIResponse<List<Status>>();
            try
            {
                string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                List<Friendship> userFriendships = friendshipRepository.ListFriendShips(id);
                List<string> friendsIds = new List<string>();
                foreach(Friendship fr in userFriendships)
                {
                    string idToAdd = fr.User1ID == id ? fr.User2ID : fr.User1ID;
                    friendsIds.Add(idToAdd);
                }
                response.Response = statusRepository.ListStatuses(friendsIds);
                return response;
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpPost]
       
        public ActionResult<APIResponse<Status>> Post(Status status)
        {
            APIResponse<Status> response = new APIResponse<Status>();
            try
            {
                response.Response = statusRepository.AddStatus(status);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }
        [HttpDelete("{id}")]
        public ActionResult<APIResponse<Status>> Delete(int id)
        {
            APIResponse<Status> response = new APIResponse<Status>();
            try
            {
                response.Response = statusRepository.DeleteStatus(id);
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
