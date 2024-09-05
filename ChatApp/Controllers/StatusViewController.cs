using ChatApp.Models;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace ChatApp.Controllers
{
    [Authorize]
    [Route("api/V1/[controller]")]
    [ApiController]
    public class StatusViewController : ControllerBase
    {
        private readonly IStatusViewRepository statusViewRepository;

        public StatusViewController(IStatusViewRepository _statusViewRepository)
        {
            statusViewRepository = _statusViewRepository;
        }

        
        [HttpGet("{id}")]
        public ActionResult<APIResponse<List<StatusView>>> Get(int id)
        {
            APIResponse<List<StatusView>> response = new APIResponse<List<StatusView>>();
            try
            {
                response.Response = statusViewRepository.GetAllStatusViews(id);
                return response;
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }

         
        [HttpPost]
        public ActionResult<APIResponse<StatusView>> Post([FromBody] StatusView statusView)
        {
            APIResponse<StatusView> response = new APIResponse<StatusView>();
            try
            {
                response.Response = statusViewRepository.AddStatusView(statusView);
                return response;
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
                return response;
            }
        }

      
    }
}
