using MySql.Data.MySqlClient;
using Dapper;
using ServerApp.Models;

namespace ServerApp.Services;

public class DatabaseService
{
    private readonly string _connectionString; 
    
    public DatabaseService(IConfiguration config)
     { 
        _connectionString = config.GetConnectionString("SafeVaultDb") 
            ?? throw new ArgumentNullException("SafeVaultDb connection string not found");
    }

    public async Task CreateUser(string username, string passwordHash)
    {
        using var conn = new MySqlConnection(_connectionString);
        await conn.ExecuteAsync("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)",
            new { Username = username, PasswordHash = passwordHash });
    }

    public async Task<User?> GetUser(string username)
    {
        using var conn = new MySqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username",
            new { Username = username });
    }
}
