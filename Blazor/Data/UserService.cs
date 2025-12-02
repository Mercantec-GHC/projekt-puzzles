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
        string hashedPassword = hashPassword(password);
        // Implement user login logic here using _connectionString
        return new User(); // Placeholder
    }
}