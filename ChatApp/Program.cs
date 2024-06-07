using System.Text;
using ChatApp.Context;
using ChatApp.Hubs;
using ChatApp.Managers;
using ChatApp.Repositories.ChatRooms;
using ChatApp.Repositories.Contacts;
using ChatApp.Repositories.Messages;
using ChatApp.Repositories.Users;
using ChatApp.Services.Auth;
using ChatApp.Services.Blobs;
using ChatApp.Services.ChatRooms;
using ChatApp.Services.Contacts;
using ChatApp.Services.Messages;
using ChatApp.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
});

// TODO: Store JWT configuration securely
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings is null)
{
    throw new InvalidOperationException("JwtSettings configuration is missing");
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(jwtSettings);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddTransient<JwtUtil>();

builder.Services.AddSingleton<IBlobService, BlobService>();

builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContactService, ContactService>();

builder.Services.AddTransient<FileRestrictionsManager>();
builder.Services.AddTransient<FileProcessor>();
builder.Services.AddSingleton<IProfilePictureService, ProfilePictureService>();
builder.Services.AddSingleton<IFileService, FileService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
{
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
    options.AddSignalRSwaggerGen();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["Azurite:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["Azurite:queue"], preferMsi: true);
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("chat");

app.Run();
