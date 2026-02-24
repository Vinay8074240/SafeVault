using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ServerApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("SafeVaultDb");

// JWT settings
var key = builder.Configuration["Jwt:Key"] ?? "super_secret_key_123";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "SafeVault";

// Register DatabaseService
builder.Services.AddScoped<DatabaseService>();

// Enable CORS for ClientApp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp",
        policy => policy.WithOrigins("http://localhost:5190")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowClientApp");
app.UseAuthentication();
app.UseAuthorization();

// Register endpoint
app.MapPost("/api/auth/register", async (UserRegister model, DatabaseService db) =>
{
    var hashed = BCrypt.Net.BCrypt.HashPassword(model.Password);
    await db.CreateUser(model.Username, hashed);
    return Results.Ok("User registered successfully");
});

// Login endpoint
app.MapPost("/api/auth/login", async (UserLogin model, DatabaseService db) =>
{
    var user = await db.GetUser(model.Username);
    if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();
    var keyBytes = Encoding.UTF8.GetBytes(key);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim("username", user.Username),
            new Claim("role", user.Role)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = issuer,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
});

// Protected endpoint
app.MapGet("/api/secure-data", [Microsoft.AspNetCore.Authorization.Authorize] () =>
{
    return new { Secret = "This is protected data only for logged-in users." };
});

app.Run();

record UserRegister(string Username, string Password);
record UserLogin(string Username, string Password);
