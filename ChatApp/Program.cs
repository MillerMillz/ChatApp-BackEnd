using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Models.DataAccessLayer;
using ChatApp.Models.Repositories;
using ChatApp.Models.SignalRModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;

    options.User.RequireUniqueEmail = true;

});
builder.Services.AddAuthentication(op=>
    { op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {

            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if(!string.IsNullOrEmpty(accessToken)&&(path.StartsWithSegments("/ChatHub")))
                {
                    context.Token = accessToken;
                };
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddControllers();
builder.Services.AddScoped<IMessageRepositiory, MessageRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
builder.Services.AddScoped<IChatRoomMembershipRepository, ChatRoomMembershipRepository>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IInviteRepository, InviteRepository>();
builder.Services.AddScoped<IRoomChatRepository, RoomChatRepository>();
builder.Services.AddScoped<IRoomChatMessagesRepository, RoomChatMessageRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IStatusViewRepository, StatusViewRepository>();
builder.Services.AddSingleton<IDictionary<string,string>>(opts => new Dictionary<string,string>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(
     builder=>
     {
         builder.WithOrigins("http://localhost:3000","http://192.168.0.29:3000")
         .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
     }
    
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request path: {context.Request.Path}");
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        Console.WriteLine("Authorization header found"+context.Request.Headers.Authorization);

    }
    else
    {
        Console.WriteLine("Authorization header missing");
    }
    await next.Invoke();
});

app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        Console.WriteLine("User authenticated");
    }
    else
    {
        Console.WriteLine("User not authenticated");
    }
    await next.Invoke();
});

app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine("User authorized");
    await next.Invoke();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/ChatHub");
    endpoints.MapControllers();
});

app.Run();
