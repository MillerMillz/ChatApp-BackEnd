using ChatApp.Models;
using ChatApp.Models.DisplayModels;
using ChatApp.Models.Repositories;
using ChatApp.Models.SignalRModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ChatApp.Hubs
{
    [Authorize]
    public class ChatHub:Hub
    {
        private readonly IDictionary<string, string> _connections;
        private readonly IChatRoomMembershipRepository roomMembershipRepository;
        private readonly IMessageRepositiory messageRepositiory;
        private readonly IChatRoomRepository chatRoomRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly string _botUser;
        private readonly string _botId;
        public ChatHub(IDictionary<string, string> connections,IChatRoomMembershipRepository roomMembershipRepository,IMessageRepositiory _messageRepositiory,IChatRoomRepository chatRoomRepository,UserManager<ApplicationUser> userManager)
        {
            _connections = connections;
            this.roomMembershipRepository = roomMembershipRepository;
            messageRepositiory = _messageRepositiory;
            this.chatRoomRepository = chatRoomRepository;
            this.userManager = userManager;
            _botUser = "Xylem ChatBot";
            _botId = "64f8dd1f-d35f-449f-badf-5a4cff12a65b";
        }
        public async override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            List<int> roomMembershipsIds = roomMembershipRepository.ListChatRoomMemberships(userId).Select(CRM => CRM.RoomId).ToList();
            List<string> roomNames = chatRoomRepository.ListChatRooms(roomMembershipsIds).Select(CR => CR.Name).ToList();

            foreach (string room in roomNames)
            { 
                await Groups.AddToGroupAsync(Context.ConnectionId, room);
                await Clients.Group(room).SendAsync("RecieveBotMessage", _botUser, $"{user.FirstName} is online");
                await Clients.Group(room).SendAsync("RecieveConnectedUsers", _connections.Select(c => c.Value).ToList());
            }
            Console.WriteLine("--> Connection Established"+Context.ConnectionId);
            _connections[userId] = Context.ConnectionId;
            await Clients.Client(Context.ConnectionId).SendAsync("RecieveConnID", Context.ConnectionId);
            List<string> users = _connections.Select(c => c.Value).ToList();
            await Clients.All.SendAsync("RecieveConnectedUsers", users);
            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {

            if (_connections.TryGetValue(Context.ConnectionId, out string userId))
            {
                ApplicationUser user = await userManager.FindByIdAsync(userId);
                _connections.Remove(userId);
                List<int> roomMembershipsIds = roomMembershipRepository.ListChatRoomMemberships(userId).Select(CRM => CRM.RoomId).ToList();
                List<string> roomNames = chatRoomRepository.ListChatRooms(roomMembershipsIds).Select(CR => CR.Name).ToList();

                foreach (string room in roomNames)
                {
                    await Clients.Group(room).SendAsync("RecieveBotMessage", _botUser, $"{user.FirstName} went offline",DateTime.Now);
                    await Clients.Group(room).SendAsync("RecieveConnectedUsers", _connections.Select(c => c.Value).ToList());
                }
                List<string> users = _connections.Select(c => c.Value).ToList();
                await Clients.All.SendAsync("RecieveConnectedUsers", users);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessageAsync(string userId, MessageDisplayModel message)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (_connections.TryGetValue(userId, out string connID))
            {
                

                await Clients.Client(connID).SendAsync("RecieveMessage",message);
            }
        }
        public async Task SendMessageToGroupAsync(int roomId)
        {
            ChatRoom room = chatRoomRepository.GetChatRoom(roomId);
            await Clients.Group(room.Name).SendAsync("RecieveMessage", room.Name, "refresh");
           
        }
        public async Task JoinRoom(string room)
        {
            string userId = Context.UserIdentifier;
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Clients.Group(room).SendAsync("RecieveMessage", _botUser, $"{user.FirstName} has joined {room}");

        }
        public async Task ConnectUsers()
        {
             List<string> users = _connections.Select(c => c.Value).ToList();
            await Clients.Client(Context.ConnectionId).SendAsync("RecieveConnectedUsers", users);
        }
     
    }
}
