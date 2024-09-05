namespace ChatApp.Models.DisplayModels
{
    public class ChatRoomDisplayModel
    {
        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Bio { get; set; }
            public string AdminId { get; set; }
            public string? FilePath { get; set; }
            public string? Image { get; set; }
        }
        public class Request
        {
            public string Name { get; set; }
            public string? Bio { get; set; }
            public string AdminId { get; set; }
             public IFormFile? Image { get; set; }
        }
    }
}
