using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Models.DisplayModels;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("api/V1/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipRepository friendshipRepository;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IInviteRepository inviteRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public FriendshipController(IFriendshipRepository _friendshipRepository,IHubContext<ChatHub> hubContext,IInviteRepository inviteRepository,UserManager<ApplicationUser> userManager)
        {
            friendshipRepository = _friendshipRepository;
            this.hubContext = hubContext;
            this.inviteRepository = inviteRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<APIResponse<List<FriendDisplayModel>>> Get()
        {
            APIResponse<List<FriendDisplayModel>> response = new APIResponse<List<FriendDisplayModel>>();
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                List<Friendship> friendships = friendshipRepository.ListFriendShips(userID);
                List<FriendDisplayModel> models = new();
                foreach (Friendship fr in friendships)
                {
                    FriendDisplayModel model = new FriendDisplayModel() { Friendship = fr, User = await userManager.FindByIdAsync(fr.User1ID==userID?fr.User2ID:fr.User1ID) };
                    model.User.Image = model.User.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, model.User.FilePath);
                    models.Add(model);
                }
               response.Response=models;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                
            }
           return response;
        }

        
        [HttpGet("{id}")]
        public ActionResult<APIResponse<Friendship>> Get(int id)
        {
            APIResponse<Friendship> response = new APIResponse<Friendship>();
            try
            {
                response.Response = friendshipRepository.GetFriendship(id);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }

       
        [HttpPost("{id}")]
        public async Task<APIResponse<Friendship>> Post(int id, [FromBody] Friendship friendship)
        {
            APIResponse<Friendship> response = new APIResponse<Friendship>();
            try
            {
                friendship.Time = DateTime.Now;
                string userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                response.Response = friendshipRepository.AddFriendship(friendship);
                inviteRepository.DeleteInvite(id);
                await hubContext.Clients.User(friendship.User1ID==userId? friendship.User2ID :friendship.User1ID).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Friends");

            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
               
            }
            return response;
        }
        [HttpGet]
        [Route("Block/{id}")]
        public ActionResult<APIResponse<Block>> Block(int id)
        {
            APIResponse<Block> response = new();
            try
            {
                response.Response = friendshipRepository.GetBlock(id);

            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
               
            }
            return response;
        }
        
        [HttpPut]
        public async Task<APIResponse<Friendship>> Put( [FromBody] Friendship friendship)
        {
            APIResponse<Friendship> response = new APIResponse<Friendship>();
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                response.Response = friendshipRepository.EditFriendship(friendship,userID);
                await hubContext.Clients.User(friendship.User1ID == userID ? friendship.User2ID : friendship.User1ID).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Friends");
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }

        // DELETE api/<FriendshipController>/5
        [HttpDelete("{id}")]
        public ActionResult<APIResponse<Friendship>> Delete(int id)
        {
            APIResponse<Friendship> response = new APIResponse<Friendship>();
            try
            {
                response.Response = friendshipRepository.DeleteFriendShip(id);
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
