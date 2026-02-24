using MySql.Data.MySqlClient;
using Dapper;

public class DatabaseService
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public DatabaseService(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("SafeVaultDb");
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

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User";
}
