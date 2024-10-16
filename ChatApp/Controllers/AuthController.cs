using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApp.Hubs;
using ChatApp.Models.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly IInviteRepository inviteRepository;
        private readonly IFriendshipRepository friendshipRepository;
        //await hubContext.Clients.User(chat.FriendId).SendAsync("RecieveMessage", DateTime.Now.ToString(), "Messages");
        public AuthController(SignInManager<ApplicationUser> _signInManager,IWebHostEnvironment _hostEnvironment, IHubContext<ChatHub> hubContext,
            UserManager<ApplicationUser> _userManager,IConfiguration _config,IInviteRepository inviteRepository,IFriendshipRepository friendshipRepository)
        {
            signInManager = _signInManager;
            hostEnvironment = _hostEnvironment;
            this.hubContext = hubContext;
            userManager = _userManager;
            config = _config;
            this.inviteRepository = inviteRepository;
            this.friendshipRepository = friendshipRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<APIResponse<RegisterModel.Response>> Register([FromForm]RegisterModel.Request request)
        {
            var user = new ApplicationUser()
                {
                    UserName = request.Email,
                    Email = request.Email,
                    Bio = request.Bio,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    FilePath = saveImage(request.File),
                };
         
          
            var result = await userManager.CreateAsync(user, request.Password);
            var response = new APIResponse<RegisterModel.Response>();
            try
            {
                if (result.Succeeded)
                {
                    response.Response = new RegisterModel.Response()
                    {
                        Email = request.Email
                    };
                    await hubContext.Clients.All.SendAsync("RecieveMessage", DateTime.Now.ToString(), "Users");
                }
                else
                {
                    response.Errors.AddRange(result.Errors.Select(error => error.Description));
                   
                }
            }
            catch(Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }
        private string saveImage(IFormFile file)
        {

            bool filePathExists = System.IO.Directory.Exists(Path.Combine(hostEnvironment.WebRootPath,$"DisplayPictures"));
            if(!filePathExists)
            {
                System.IO.Directory.CreateDirectory(Path.Combine(hostEnvironment.WebRootPath, $"DisplayPictures"));
            }
            string? filePath = null, uniqueName=null, returnString =  "No file";

            if(file!=null)
            {
                string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, $"DisplayPictures");
                uniqueName = $"{Guid.NewGuid().ToString()}_{file.FileName}";
                filePath = Path.Combine(uploadFolder, uniqueName);
                returnString = Path.Combine($"DisplayPictures", uniqueName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));
            }
         
            return returnString;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<APIResponse<LoginModel.Response>> Login(LoginModel.Request request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            var response =new APIResponse<LoginModel.Response>();

            if(user==null)
            {
                response.Errors.Add("Email not found!!");
                return response;
            }
            var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if(result.Succeeded)
            {
                response.Response = new LoginModel.Response()
                {
                    User = user,
                };
            }
            else
            {
                response.Errors.Add("Invalid Password");
            }

            return response;
        }
        [HttpPost]
        [Route("SignOut")]
        public async Task<APIResponse<ApplicationUser>> SignOut()
        {
            APIResponse<ApplicationUser> response = new APIResponse<ApplicationUser>()
            {
                Response= await userManager.GetUserAsync(User)
             };
            await signInManager.SignOutAsync();

            return response;
        }
        [HttpGet]
        [Route("GetCurrentUser")]
        public async Task<APIResponse<ApplicationUser>> GetCurrentUser()
        {
            var user = await userManager.GetUserAsync(User);

            APIResponse<ApplicationUser> response = new APIResponse<ApplicationUser>();

            if (user == null)
            {
                response.Errors.Add("User not found");
            }
            else
            {
                response.Response = user;
            }
           
            return response;
        }
        [HttpGet]
        [Route("Users")]
        public async Task<APIResponse<List<ApplicationUser>>> Users()
        {
            APIResponse<List<ApplicationUser>> response = new();
            try
            {
                string userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                List<string> idz = inviteRepository.ListInvites(userId).Select(inv => inv.SenderId).Concat(inviteRepository.SentInvites(userId).Select(inv=>inv.ReciepientID)).ToList();
                List<string> idz2 = friendshipRepository.FriendsIdz(userId);
                List<ApplicationUser> users = userManager.Users.Where(user => !idz.Contains(user.Id) && !idz2.Contains(user.Id) && user.Id != userId && user.Id !="64f8dd1f-d35f-449f-badf-5a4cff12a65b").ToList();
                foreach(ApplicationUser user in users)
                {
                    user.Image = user.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, user.FilePath);
               
                }
                response.Response = users;  
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("Users")]
        public async Task<APIResponse<List<string>>> Users([FromBody]List<string> ids)
        {
            APIResponse<List<string>> response = new();
            try
            {
                response.Response = userManager.Users.Where(u => ids.Contains(u.Id)).Select(u => u.FirstName).ToList();
            }
            catch (Exception e)
            {

                response.Errors.Add(e.Message);
            }
            return response;
        }
        [HttpPost]
        [Route("WebLogin")]
        public async Task<APIResponse<Token>> WebLogin([FromBody] LoginModel.Request request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            APIResponse<Token> response = new APIResponse<Token>();

            if (user == null)
            {
                response.Errors.Add("User not found, Wrong email... Please re-enter email");
               
            }
            else
            { 
                var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);

                if (result.Succeeded)
                {
                    var token = GenerateToken(user);
                    response.Response = new Token()
                    {
                        token = token,
                    };
                   
                }
                else
                {
                    response.Errors.Add("Incorrect password");
                }
            }

            return response;

        }

        [HttpGet]
        [Route("GetCurrentWebUser")]
        public async Task<APIResponse<ApplicationUser>> GetCurrentWebUser()
        {

            string userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value; 
            APIResponse<ApplicationUser> response = new APIResponse<ApplicationUser>();
            var user = await userManager.FindByIdAsync(userId);

            if (user!=null)
            {
                user.Image = user.FilePath == "No file" ? null : String.Format("{0}://{1}{2}/{3}", Request.Scheme, Request.Host, Request.PathBase, user.FilePath);
                response.Response = user;

            }
            else
            {
                response.Errors.Add("User not found");
            }

            return response;
        }

        private JwtSecurityToken Verify(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidAudience= config["Jwt:Audience"],
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidateLifetime=true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey=true,
                    ValidateIssuer=true,
                    ValidateAudience=true,
                },out SecurityToken validateToken) ;

                return (JwtSecurityToken)validateToken;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private string GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.FirstName),
                new Claim(ClaimTypes.Surname,user.LastName)
            };

            var token = new JwtSecurityToken(
                config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                expires:DateTime.Now.AddDays(3),
                signingCredentials:credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    } 
    
}
