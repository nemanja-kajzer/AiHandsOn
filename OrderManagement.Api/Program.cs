namespace OrderManagement.Api;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Simple JWT key for demo purposes
var jwtKey = builder.Configuration["Jwt:Key"] ?? "demo_super_secret_key_which_should_be_long";

builder.Services.AddDbContext<OrdersDbContext>(opts => opts.UseInMemoryDatabase("OrdersDb"));
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton(new TokenService(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Orders.Add(new Order { Product = "Keyboard", Quantity = 1, CreatedAt = DateTime.UtcNow });
    db.Orders.Add(new Order { Product = "Mouse", Quantity = 2, CreatedAt = DateTime.UtcNow });
    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
