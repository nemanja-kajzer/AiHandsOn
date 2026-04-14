var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<AiHandsOn.Api.Services.IUserService, AiHandsOn.Api.Services.UserService>();
builder.Services.AddSingleton<AiHandsOn.Api.Services.IUserServiceManual, AiHandsOn.Api.Services.UserServiceManual>();
// Register validator
builder.Services.AddSingleton<AiHandsOn.Api.Validation.IRegistrationValidator, AiHandsOn.Api.Validation.RegistrationValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
