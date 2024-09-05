using ChatApp.Models;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("api/V1/[controller]")]
    [ApiController]
    public class ChatMessageController : ControllerBase
    {
        private readonly IChatMessageRepository chatMessageRepository;

        public ChatMessageController(IChatMessageRepository _chatMessageRepository)
        {
            chatMessageRepository = _chatMessageRepository;
        }
/*
        // GET: api/<ChatMessageController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

        // GET api/<ChatMessageController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ChatMessageController>
        [HttpPost]
        public ActionResult<APIResponse<ChatMessage>> Post([FromBody] ChatMessage chatMessage)
        {
            APIResponse<ChatMessage> response = new();
            try
            {
                response.Response= chatMessageRepository.Create(chatMessage);
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }

       /* // PUT api/<ChatMessageController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }*/

        // DELETE api/<ChatMessageController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
