using UserApi.Endpoints;
using UserApi.Repositories;
using UserApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGroup("/users")
    .WithTags("Users")
    .MapUserEndpoints();

app.Run();
