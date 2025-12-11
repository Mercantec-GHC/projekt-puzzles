using System.Security.Cryptography;
using System.Text;
using Npgsql;

public class UserService
{
    private readonly string _connectionString;
    public UserService()
    {
        _connectionString = ConnectionString.DefaultConnection;
    }

    private string hashPassword(string password)
    {
        // A SHA256 hash is a string with the length of 64 characters in a hexadecimal format
        // here we create the method to convert any text to this format
        using var sha = SHA256.Create();

        // here we take the password and convert it to a byte array
        // then instantly run the convertion on the password byte array to a hash byte array
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

        // then finally we convert our SHA256 converted password to Base64 which takes all bits in the size of 6-bit per character
        // and turn them into readable characters without losing any data
        return Convert.ToBase64String(bytes);
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO \"UserAccounts\" (\"Username\", \"PassHash\", \"Email\", \"PhoneNumber\") VALUES (@name, @passhash, @email, @phonenumber)",
                conn
            );
            user.PassHash = hashPassword(user.Password);
            cmd.Parameters.AddWithValue("name", user.Username);
            cmd.Parameters.AddWithValue("passhash", user.PassHash);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("phonenumber", user.PhoneNumber);

            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            return false;
        }
    }

    public async Task<User> LoginUserAsync(string username, string password)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                @"SELECT 
                    u.""UserId"", 
                    u.""Username"", 
                    u.""Email"", 
                    u.""PhoneNumber"", 
                    u.""PassHash"", 
                    u.""CreatedAt"" 
                FROM 
                    ""UserAccounts"" u
                WHERE 
                    u.""Username"" = @name",
                conn
            );
            cmd.Parameters.AddWithValue("name", username);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var hash = reader.GetString(4);
                
                if (hash == hashPassword(password))
                {
                    return new User
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        PhoneNumber = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(5)
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging in user: {ex.Message}");
        }
        return null;
    }

    public async Task UpdateUserAsync(User user, string? password = null)
    {
        List<string> updateFields = new List<string>();
        if (user.Password != null && password != null)
        {
            if (await LoginUserAsync(user.Username, password) == null)
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }
            updateFields.Add($@"""PassHash"" = @passhash");
        }
        if (user.Email != null)
        {
            updateFields.Add($@"""Email"" = @email");
        }
        if (user.PhoneNumber != null)
        {
            updateFields.Add($@"""PhoneNumber"" = @phonenumber");
        }

        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                $@"UPDATE 
                    ""UserAccounts"" 
                SET 
                    {string.Join(", ", updateFields)} 
                WHERE 
                    ""UserId"" = @userid",
                conn
            );
            if (user.Password != null)
            {
                user.PassHash = hashPassword(user.Password);
                cmd.Parameters.AddWithValue("passhash", user.PassHash);
            }
            if (user.Email != null)
            {
                cmd.Parameters.AddWithValue("email", user.Email);
            }
            if (user.PhoneNumber != null)
            {
                cmd.Parameters.AddWithValue("phonenumber", user.PhoneNumber);
            }
            cmd.Parameters.AddWithValue("userid", user.UserId);

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
        }
    }
}