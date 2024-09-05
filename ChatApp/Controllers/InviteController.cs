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
    public class InviteController : ControllerBase
    {
        private readonly IInviteRepository inviteRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<ChatHub> hubContext;

        public InviteController(IInviteRepository _inviteRepository,UserManager<ApplicationUser> userManager, IHubContext<ChatHub> hubContext)
        {
            inviteRepository = _inviteRepository;
            this.userManager = userManager;
            this.hubContext = hubContext;
        }
        [HttpGet]
        public async Task<APIResponse<List<InviteDisplayModel>>> Get()
        {
            APIResponse<List<InviteDisplayModel>> response = new APIResponse<List<InviteDisplayModel>>();
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                List<InviteDisplayModel> models = new();
                foreach (Invite inv in inviteRepository.ListInvites(userID))
                {
                    InviteDisplayModel model = new InviteDisplayModel() { Invite = inv, User = await userManager.FindByIdAsync(inv.SenderId) };
                    model.User.Image = model.User.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, model.User.FilePath);
                    models.Add(model);
                }
                response.Response = models;
              
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                
            }
             return response;
        }
        [HttpGet]
        [Route("sentInvites")]
        public async Task<APIResponse<List<InviteDisplayModel>>> GetInvites()
        {
            APIResponse<List<InviteDisplayModel>> response = new APIResponse<List<InviteDisplayModel>>();
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                List<InviteDisplayModel> models = new();
                foreach (Invite inv in inviteRepository.SentInvites(userID))
                {
                    InviteDisplayModel model = new InviteDisplayModel() { Invite = inv, User = await userManager.FindByIdAsync(inv.ReciepientID) };
                    model.User.Image = model.User.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, model.User.FilePath);
                    models.Add(model);
                }
                response.Response = models;
              
                
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                
            }
            return response;
        }

        [HttpGet("{id}")]
        public ActionResult<APIResponse<Invite>> Get(int id)
        {
            APIResponse<Invite> response = new APIResponse<Invite>();
            try
            {
                response.Response = inviteRepository.GetInvite(id);
                return response;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }


        [HttpPost]
        public async Task<APIResponse<Invite>> Post([FromBody] Invite invite)
        {
            APIResponse<Invite> response = new APIResponse<Invite>();
            try
            {
                invite.Time = DateTime.Now;
                response.Response = inviteRepository.AddInvite(invite);
                await hubContext.Clients.User(invite.ReciepientID).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Invites");
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
        public async Task<APIResponse<Invite>> Delete(int id)
        {
            APIResponse<Invite> response = new APIResponse<Invite>();
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                response.Response = inviteRepository.DeleteInvite(id);
                await hubContext.Clients.User(response.Response.ReciepientID==userId? response.Response.SenderId: response.Response.ReciepientID).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Invites");
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
