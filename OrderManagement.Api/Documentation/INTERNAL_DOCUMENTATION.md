# OrderManagement.Api - Internal Developer Documentation

## Table of Contents

1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Project Structure](#project-structure)
4. [Environment Setup](#environment-setup)
5. [Building and Running](#building-and-running)
6. [Architecture](#architecture)
7. [Database](#database)
8. [Authentication](#authentication)
9. [Adding New Features](#adding-new-features)
10. [Testing](#testing)
11. [Deployment](#deployment)
12. [Troubleshooting](#troubleshooting)
13. [Code Style and Conventions](#code-style-and-conventions)

---

## Project Overview

**OrderManagement.Api** is a RESTful ASP.NET Core Web API designed for order management operations. The project follows modern .NET development practices with clean architecture principles, JWT-based authentication, and Entity Framework Core for data persistence.

**Key Characteristics:**
- Target Framework: **.NET 9**
- Language Version: **C# 13.0**
- Architecture Pattern: **Clean Architecture** with separation of concerns
- Nullable Reference Types: **Enabled**
- Implicit Usings: **Enabled**

---

## Technology Stack

### Core Framework
- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core 8.0** - ORM for data access
- **JWT Bearer Authentication** - Security mechanism

### Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.14 | JWT token validation and authentication |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.14 | In-memory database for data persistence |
| Microsoft.EntityFrameworkCore.Design | 8.0.14 | EF Core design-time services |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger/OpenAPI documentation |

### Development Tools
- Visual Studio Community 2026 (18.4.3) or later
- .NET 9 SDK
- Git for version control

---

## Project Structure

```
OrderManagement.Api/
├── Controllers/
│   └── OrdersController.cs          # HTTP endpoint handlers
├── Models/
│   └── Order.cs                     # Data model
├── Services/
│   ├── OrderService.cs              # Business logic for orders
│   └── TokenService.cs              # JWT token generation
├── Documentation/
│   ├── API_DOCUMENTATION.md         # External API documentation
│   └── INTERNAL_DOCUMENTATION.md    # This file
├── OrdersDbContext.cs               # EF Core DbContext
├── Program.cs                       # Application configuration
└── OrderManagement.Api.csproj       # Project file

```

### Directory Responsibilities

- **Controllers/** - HTTP request handling, routing, and response formatting
- **Models/** - Domain models representing data structures
- **Services/** - Business logic, database operations, and token generation
- **Documentation/** - API and developer documentation

---

## Environment Setup

### Prerequisites

1. **Install .NET 9 SDK**
   ```powershell
   # Verify installation
   dotnet --version
   # Should output: 9.x.x
   ```

2. **Visual Studio Setup**
   - Install Visual Studio Community 2026 or later
   - Include ASP.NET and web development workload
   - Include .NET desktop development workload

3. **Git Configuration**
   ```powershell
   git config user.name "Your Name"
   git config user.email "your.email@company.com"
   ```

### Project Setup

1. **Clone the Repository**
   ```powershell
   git clone https://github.com/nemanja-kajzer/AiHandsOn.git
   cd AiHandsOn
   ```

2. **Restore Dependencies**
   ```powershell
   cd OrderManagement.Api
   dotnet restore
   ```

3. **Build the Project**
   ```powershell
   dotnet build
   ```

### Configuration

The application uses configuration from multiple sources (in order of precedence):

1. **appsettings.json** - Default configuration
2. **appsettings.{Environment}.json** - Environment-specific overrides
3. **User Secrets** - Local development secrets (Development only)
4. **Environment Variables** - System/container environment variables

**Important Configuration Keys:**

```json
{
  "Jwt": {
    "Key": "your_secret_key_should_be_long_and_secure"
  }
}
```

**Setting User Secrets (Development):**
```powershell
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your_actual_secret_key"
```

---

## Building and Running

### Development Environment

1. **Build the Solution**
   ```powershell
   dotnet build
   ```

2. **Run the Application**
   ```powershell
   dotnet run
   ```
   The API will be available at `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP)

3. **Swagger UI**
   - Open browser to `https://localhost:5001/swagger/index.html`
   - Swagger UI provides interactive API testing

### Running with Different Configurations

```powershell
# Development
dotnet run --environment Development

# Staging
dotnet run --environment Staging

# Production
dotnet run --environment Production
```

### Building for Release

```powershell
dotnet build --configuration Release

# Create deployment package
dotnet publish -c Release -o ./publish
```

---

## Architecture

### Layered Architecture

```
Presentation Layer
    ↓
OrdersController (HTTP endpoints)
    ↓
Business Logic Layer
    ↓
OrderService (business operations)
    ↓
Data Access Layer
    ↓
OrdersDbContext / EF Core
    ↓
Database (In-Memory)
```

### Service Layer Pattern

**OrderService.cs** encapsulates business logic:
- `GetAllAsync()` - Retrieves all orders
- `GetByIdAsync(int id)` - Retrieves a specific order
- `CreateAsync(Order order)` - Creates new order
- `DeleteAsync(int id)` - Deletes an order

### Dependency Injection

Dependencies are registered in `Program.cs`:

```csharp
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton(new TokenService(jwtKey));
builder.Services.AddDbContext<OrdersDbContext>(opts => 
    opts.UseInMemoryDatabase("OrdersDb"));
```

**Scope Guidelines:**
- **Scoped** - OrderService (one instance per HTTP request)
- **Singleton** - TokenService (shared across entire application)
- **DbContext** - Scoped (by default with AddDbContext)

---

## Database

### EF Core Configuration

The project uses **Entity Framework Core 8.0** with an **in-memory database** for simplified local development.

### OrdersDbContext

```csharp
public sealed class OrdersDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
}
```

### Data Model: Order

```csharp
public sealed class Order
{
    public int Id { get; set; }
    public required string Product { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Field Details:**
- `Id` - Primary key (auto-incremented)
- `Product` - Required string field (cannot be null)
- `Quantity` - Order quantity
- `CreatedAt` - Timestamp of order creation

### Database Initialization

The database is seeded with sample data on application startup:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Orders.Add(new Order { Product = "Keyboard", Quantity = 1, CreatedAt = DateTime.UtcNow });
    db.Orders.Add(new Order { Product = "Mouse", Quantity = 2, CreatedAt = DateTime.UtcNow });
    db.SaveChanges();
}
```

### Switching to Persistent Database

To use SQL Server or another provider:

1. **Install EF Core Provider**
   ```powershell
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   ```

2. **Update Program.cs**
   ```csharp
   builder.Services.AddDbContext<OrdersDbContext>(opts => 
       opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

3. **Create Migrations**
   ```powershell
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

## Authentication

### JWT Implementation

The project uses **JWT (JSON Web Tokens)** for stateless authentication.

### TokenService

Generates JWT tokens with the following specifications:

```csharp
public class TokenService(string key)
{
    public string GenerateToken(string user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

**Token Characteristics:**
- **Algorithm:** HS256 (HMAC with SHA-256)
- **Expiration:** 1 hour from generation
- **Claims:** User name claim
- **Signing Key:** Configured via Jwt:Key setting

### Authentication Flow

1. **Get Token**
   ```
   POST /orders/token?user=johndoe
   ```
   Response: `{ "token": "eyJhbGc..." }`

2. **Use Token in Requests**
   ```
   Authorization: Bearer eyJhbGc...
   ```

3. **Validation**
   - Token signature is verified against the configured key
   - Token expiration is checked
   - Claims are extracted and available to the request

### Securing Endpoints

Endpoints are protected with the `[Authorize]` attribute:

```csharp
[HttpGet]
[Authorize]
public async Task<IActionResult> GetAll()
{
    // Only authenticated requests reach here
}
```

### Security Best Practices

1. **JWT Key Management**
   - Use a strong, randomly generated key (minimum 256 bits)
   - Store in secure configuration (User Secrets for dev, Key Vault for production)
   - Never commit secrets to version control

2. **Token Expiration**
   - Current setting: 1 hour (adjust as needed)
   - Shorter expiration = better security
   - Consider implementing refresh tokens for longer sessions

3. **HTTPS Enforcement**
   - Always use HTTPS in production
   - Configure in Program.cs: `app.UseHttpsRedirection();`

---

## Adding New Features

### Adding a New Endpoint

1. **Add Model** (if needed)
   ```csharp
   // Models/NewEntity.cs
   public sealed class NewEntity
   {
       public int Id { get; set; }
       public required string Name { get; set; }
   }
   ```

2. **Add DbSet** to OrdersDbContext
   ```csharp
   public DbSet<NewEntity> NewEntities => Set<NewEntity>();
   ```

3. **Create Service**
   ```csharp
   // Services/NewEntityService.cs
   public sealed class NewEntityService(OrdersDbContext db)
   {
       public async Task<List<NewEntity>> GetAllAsync() => 
           await db.NewEntities.AsNoTracking().ToListAsync();
   }
   ```

4. **Register Service** in Program.cs
   ```csharp
   builder.Services.AddScoped<NewEntityService>();
   ```

5. **Create Controller**
   ```csharp
   // Controllers/NewEntitiesController.cs
   [ApiController]
   [Route("[controller]")]
   public sealed class NewEntitiesController(NewEntityService service) : ControllerBase
   {
       [HttpGet]
       [Authorize]
       public async Task<IActionResult> GetAll()
       {
           var list = await service.GetAllAsync();
           return Ok(list);
       }
   }
   ```

### Adding a New Service

1. **Create Service Class** in Services/ folder
2. **Implement methods** following the pattern of OrderService
3. **Register in Program.cs** with appropriate lifetime
4. **Inject into Controller** via constructor

### Adding Validation

Use data annotations or FluentValidation:

```csharp
public sealed class Order
{
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public required string Product { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
```

---

## Testing

### Unit Testing (Future Implementation)

Recommended testing framework: **xUnit** or **NUnit**

1. **Create Test Project**
   ```powershell
   dotnet new xunit -n OrderManagement.Api.Tests
   dotnet add OrderManagement.Api.Tests reference OrderManagement.Api
   ```

2. **Test Service Example**
   ```csharp
   public class OrderServiceTests
   {
       [Fact]
       public async Task GetAllAsync_ReturnsAllOrders()
       {
           // Arrange
           var options = new DbContextOptionsBuilder<OrdersDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDb")
               .Options;
           
           using (var context = new OrdersDbContext(options))
           {
               context.Orders.Add(new Order { Product = "Test", Quantity = 1 });
               context.SaveChanges();
           }
           
           // Act & Assert
           using (var context = new OrdersDbContext(options))
           {
               var service = new OrderService(context);
               var result = await service.GetAllAsync();
               Assert.Single(result);
           }
       }
   }
   ```

### Running Tests

```powershell
dotnet test
```

### Integration Testing

Use `WebApplicationFactory` for testing the full API:

```csharp
public class OrdersControllerTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_RequiresAuthorization()
    {
        var response = await _client.GetAsync("/orders");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}
```

---

## Deployment

### Pre-Deployment Checklist

- [ ] All tests passing
- [ ] Code reviewed and merged to main branch
- [ ] Security vulnerabilities resolved
- [ ] Configuration updated for target environment
- [ ] Database migrations tested
- [ ] API documentation updated
- [ ] Logs configured appropriately

### Deployment Steps

1. **Prepare Release Build**
   ```powershell
   dotnet publish -c Release -o ./publish
   ```

2. **Configure for Environment**
   - Update `appsettings.Production.json`
   - Set environment variables
   - Configure logging

3. **Database Considerations**
   - Switch from in-memory to persistent database
   - Run migrations: `dotnet ef database update`
   - Backup existing database

4. **Deploy to Hosting Platform**
   - Azure App Service
   - Docker container
   - IIS server
   - Other hosting options

### Docker Deployment

1. **Create Dockerfile** in project root
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
   WORKDIR /src
   COPY . .
   RUN dotnet publish -c Release -o /app/publish

   FROM mcr.microsoft.com/dotnet/aspnet:9.0
   WORKDIR /app
   COPY --from=build /app/publish .
   EXPOSE 80
   ENTRYPOINT ["dotnet", "OrderManagement.Api.dll"]
   ```

2. **Build Image**
   ```powershell
   docker build -t ordermanagement-api:latest .
   ```

3. **Run Container**
   ```powershell
   docker run -p 5000:80 ordermanagement-api:latest
   ```

---

## Troubleshooting

### Common Issues

#### Issue: "Unable to connect to database"
**Solution:** Verify connection string in configuration and ensure database server is accessible.

#### Issue: "JWT token validation failed"
**Solution:** 
- Verify `Jwt:Key` is correctly configured
- Ensure token hasn't expired
- Check token format includes "Bearer " prefix

#### Issue: "Port 5000/5001 already in use"
**Solution:**
```powershell
# Find process using port
netstat -ano | findstr :5001

# Kill process (replace PID)
taskkill /PID <PID> /F
```

#### Issue: "Swagger UI not loading"
**Solution:**
- Verify Swashbuckle package is installed
- Check `app.UseSwagger()` and `app.UseSwaggerUI()` are called in Program.cs
- Ensure running in Development environment or configure for other environments

### Debugging

**Enable Detailed Logging**
```csharp
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Debug);
});
```

**Using Visual Studio Debugger**
1. Set breakpoints in code
2. Press F5 to start debugging
3. Use Debug menu for step over/into/out operations

---

## Code Style and Conventions

### C# Conventions

1. **Naming**
   - Classes: PascalCase (e.g., `OrderService`)
   - Methods: PascalCase (e.g., `GetAllAsync`)
   - Properties: PascalCase
   - Fields: camelCase with underscore prefix (e.g., `_service`)
   - Parameters: camelCase (e.g., `userId`)
   - Constants: UPPER_SNAKE_CASE

2. **Sealed Classes**
   - Use `sealed` keyword on classes that shouldn't be inherited from
   - Improves runtime performance and security

3. **Async Patterns**
   - Use `async`/`await` for I/O-bound operations
   - Suffix async methods with `Async` (e.g., `GetAllAsync`)
   - Return `Task` or `Task<T>`, not `void`

4. **Nullable Reference Types**
   - Use `required` keyword for required properties
   - Use `?` for nullable types
   - Avoid null reference exceptions

5. **Implicit Usings**
   - Common namespaces are automatically imported
   - Reduces file header boilerplate

### Code Example

```csharp
namespace OrderManagement.Api.Services;

public sealed class OrderService(OrdersDbContext db)
{
    private readonly OrdersDbContext _db = db;

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders.AsNoTracking().ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders.FindAsync(id);
    }
}
```

### Project File Practices

- Use implicit namespaces and usings
- Enable nullable reference types
- Target latest stable .NET version
- Keep package versions updated

---

## Additional Resources

### Official Documentation
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JWT Authentication](https://datatracker.ietf.org/doc/html/rfc7519)
- [C# Documentation](https://docs.microsoft.com/dotnet/csharp)

### Learning Resources
- Microsoft Learn - ASP.NET Core training
- GitHub - .NET Samples and best practices
- Channel 9 - Video tutorials

---

## Contact and Support

For questions about development practices or project setup:
- Review the API_DOCUMENTATION.md for API contract details
- Check existing issues on GitHub for similar problems
- Reach out to the team lead for architectural decisions

**Last Updated:** January 2024  
**Target Framework:** .NET 9  
**C# Version:** 13.0
