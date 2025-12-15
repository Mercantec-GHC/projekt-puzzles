using System.Security.Cryptography;
using System.Text;
using Npgsql;

/// <summary>
/// Provides user-related operations such as registration, authentication, and updating user information.
/// Handles password hashing and database interactions for user accounts.
/// </summary>
public class UserService
{
    /// <summary>
    /// The connection string used to connect to the database.
    /// </summary>
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class using the default connection string.
    /// </summary>
    public UserService()
    {
        _connectionString = ConnectionString.DefaultConnection;
    }

    /// <summary>
    /// Hashes a password using SHA256 and encodes it as a Base64 string.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <returns>The hashed password as a Base64-encoded string.</returns>
    private string hashPassword(string password)
    {
        // A SHA256 hash is a string with the length of 64 characters in a hexadecimal format
        // Here we create the method to convert any text to this format
        using var sha = SHA256.Create();

        // Convert the password to a byte array and compute the hash
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

        // Convert the hash byte array to a Base64 string for storage
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Asynchronously creates a new user in the database.
    /// </summary>
    /// <param name="user">The <see cref="User"/> object containing user details to be created.</param>
    /// <returns>True if the user was created successfully; otherwise, false.</returns>
    public async Task<bool> CreateUserAsync(User user)
    {
        try
        {
            // Open a new database connection
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            // Prepare the SQL command for inserting a new user
            using var cmd = new NpgsqlCommand(
                "INSERT INTO \"UserAccounts\" (\"Username\", \"PassHash\", \"Email\", \"PhoneNumber\") VALUES (@name, @passhash, @email, @phonenumber)",
                conn
            );
            // Hash the user's password before storing
            user.PassHash = hashPassword(user.Password);
            cmd.Parameters.AddWithValue("name", user.Username);
            cmd.Parameters.AddWithValue("passhash", user.PassHash);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("phonenumber", user.PhoneNumber);

            // Execute the command asynchronously
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Log the error and return false
            Console.WriteLine($"Error creating user: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Asynchronously attempts to log in a user by verifying the username and password.
    /// </summary>
    /// <param name="username">The username of the user attempting to log in.</param>
    /// <param name="password">The plain text password to verify.</param>
    /// <returns>The <see cref="User"/> object if authentication is successful; otherwise, null.</returns>
    public async Task<User> LoginUserAsync(string username, string password)
    {
        try
        {
            // Open a new database connection
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            // Prepare the SQL command to select the user by username
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
                // Retrieve the stored password hash
                var hash = reader.GetString(4);
                // Compare the stored hash with the hash of the provided password
                if (hash == hashPassword(password))
                {
                    // Return the user object if authentication is successful
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
            // Log the error
            Console.WriteLine($"Error logging in user: {ex.Message}");
        }
        // Return null if authentication fails
        return null;
    }

    /// <summary>
    /// Asynchronously updates the specified user's information in the database. 
    /// Optionally verifies the current password before updating the password hash.
    /// </summary>
    /// <param name="user">The <see cref="User"/> object containing updated user information.</param>
    /// <param name="password">
    /// The current password of the user, required if the password is being updated. 
    /// If provided, the method verifies the password before updating the password hash.
    /// </param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the provided current password is incorrect when attempting to update the password.
    /// </exception>
    public async Task UpdateUserAsync(User user, string? password = null)
    {
        // Build a list of fields to update based on provided user data
        List<string> updateFields = new List<string>();
        if (user.Password != null && password != null)
        {
            // Verify the current password before allowing password update
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
            // Open a new database connection
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            // Prepare the SQL command for updating user fields
            using var cmd = new NpgsqlCommand(
                $@"UPDATE 
                    ""UserAccounts"" 
                SET 
                    {string.Join(", ", updateFields)} 
                WHERE 
                    ""UserId"" = @userid",
                conn
            );
            // Add parameters for fields being updated
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

            // Execute the update command asynchronously
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error updating user: {ex.Message}");
        }
    }
}