using Npgsql;

public class AdvertService
{
    private readonly string _connectionString;
	public AdvertService()
    {
        _connectionString = ConnectionString.DefaultConnection;
    }

    public async Task AddAdvertAsync(Advert advert)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                @"INSERT INTO 
                    ""Adverts"" (
                        ""Title"",
                        ""Description"", 
                        ""Price"", 
                        ""PieceAmount"", 
                        ""BoxDimHeight"", 
                        ""BoxDimWidth"", 
                        ""BoxDimDepth"", 
                        ""PuzzleDimHeight"", 
                        ""PuzzleDimWidth"", 
                        ""Picture"", 
                        ""UserId"", 
                        ""CreatedAt"", 
                        ""IsSold""
                    ) VALUES (
                        @title, 
                        @description, 
                        @price, 
                        @pieceamount, 
                        @boxdimheight, 
                        @boxdimwidth, 
                        @boxdimdepth, 
                        @puzzledimheight, 
                        @puzzledimwidth, 
                        @picture, 
                        @userid, 
                        @createdat, 
                        @issold
                    )",        
                conn
            );

            cmd.Parameters.AddWithValue("title", advert.Title);
            cmd.Parameters.AddWithValue("description", advert.Description);
            cmd.Parameters.AddWithValue("price", advert.Price);
            cmd.Parameters.AddWithValue("pieceamount", advert.PieceAmount);
            cmd.Parameters.AddWithValue("boxdimheight", advert.BoxDimensions.Height);
            cmd.Parameters.AddWithValue("boxdimwidth", advert.BoxDimensions.Width);
            cmd.Parameters.AddWithValue("boxdimdepth", advert.BoxDimensions.Depth);
            cmd.Parameters.AddWithValue("puzzledimheight", advert.PuzzleDimensions.Height);
            cmd.Parameters.AddWithValue("puzzledimwidth", advert.PuzzleDimensions.Width);
            cmd.Parameters.AddWithValue("picture", (object?)advert.Picture ?? DBNull.Value);
            cmd.Parameters.AddWithValue("userid", advert.User.UserId);
            cmd.Parameters.AddWithValue("createdat", advert.CreatedAt);
            cmd.Parameters.AddWithValue("issold", advert.IsSold);
            await cmd.ExecuteNonQueryAsync();
        } catch (Exception ex)
        {
            Console.WriteLine($"Error adding advert: {ex.Message}");
        }
    }

    public async Task<int> GetAdvertCountAsync()
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                @"SELECT COUNT(*) FROM ""Advert"" WHERE ""IsSold"" = FALSE",
                conn
            );

            var count = (long)await cmd.ExecuteScalarAsync();
            return (int)count;
        } catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving advert count: {ex.Message}");
            return 0;
        }
    }

    public async Task<List<Advert>> GetAllAdvertsAsync(int offset = 0, int limit = 100)
    {
        var adverts = new List<Advert>();
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                @"SELECT 
                    a.""AdvertId"", 
                    a.""Title"", 
                    a.""Description"", 
                    a.""Price"", 
                    a.""PieceAmount"", 
                    a.""BoxDimHeight"", 
                    a.""BoxDimWidth"", 
                    a.""BoxDimDepth"", 
                    a.""PuzzleDimHeight"", 
                    a.""PuzzleDimWidth"", 
                    a.""Picture"", 
                    u.""UserId"",
                    u.""Username"", 
                    u.""Email"", 
                    u.""PhoneNumber"", 
                    a.""CreatedAt"", 
                    a.""IsSold"" 
                FROM 
                    ""Advert"" a 
                    LEFT JOIN ""UserAccounts"" u ON a.""UserId"" = u.""UserId""
                WHERE 
                    a.""IsSold"" = FALSE
                ORDER BY 
                    a.""CreatedAt"" DESC 
                OFFSET 
                    @offset 
                LIMIT 
                    @limit",
                conn
            );

            cmd.Parameters.AddWithValue("offset", offset);
            cmd.Parameters.AddWithValue("limit", limit);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                adverts.Add(new Advert
                {
                    AdvertId = reader.GetInt32(0),
                    Title = reader.GetString(1), 
                    Description = reader.GetString(2),
                    Price = reader.GetDouble(3),
                    PieceAmount = reader.GetInt32(4),
                    BoxDimensions = new Vector3{
                        Height = reader.GetDouble(5),
                        Width = reader.GetDouble(6),
                        Depth = reader.GetDouble(7)
                    },
                    PuzzleDimensions = new Vector2{
                        Height = reader.GetDouble(8),
                        Width = reader.GetDouble(9)
                    },
                    Picture = !reader.IsDBNull(10) ? reader["Picture"] as byte[] : null,
                    User = new User { 
                        UserId = reader.GetInt32(11),
                        Username = reader.GetString(12), 
                        Email = reader.GetString(13), 
                        PhoneNumber = reader.GetString(14) 
                    },
                    CreatedAt = reader.GetDateTime(15),
                    IsSold = reader.GetBoolean(16)
                });
            }
            return adverts;
        } catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving adverts: {ex.Message}");
            return adverts;
        }
    }

    public async Task<Advert> GetAdvertByIdAsync(int advertId)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                @"SELECT 
                    a.""AdvertId"", 
                    a.""Title"", 
                    a.""Description"", 
                    a.""Price"", 
                    a.""PieceAmount"", 
                    a.""BoxDimHeight"", 
                    a.""BoxDimWidth"", 
                    a.""BoxDimDepth"", 
                    a.""PuzzleDimHeight"", 
                    a.""PuzzleDimWidth"", 
                    a.""Picture"", 
                    u.""Username"", 
                    u.""Email"", 
                    u.""PhoneNumber"", 
                    a.""CreatedAt"", 
                    a.""IsSold"" 
                FROM 
                    ""Advert"" a 
                    LEFT JOIN ""UserAccounts"" u ON a.""UserId"" = u.""UserId""
                WHERE 
                    a.""AdvertId"" = @advertId",
                conn
            );

            cmd.Parameters.AddWithValue("advertId", advertId);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Advert
                {
                    AdvertId = reader.GetInt32(0),
                    Title = reader.GetString(1), 
                    Description = reader.GetString(2),
                    Price = reader.GetDouble(3),
                    PieceAmount = reader.GetInt32(4),
                    BoxDimensions = new Vector3{
                        Height = reader.GetDouble(5),
                        Width = reader.GetDouble(6),
                        Depth = reader.GetDouble(7)
                    },
                    PuzzleDimensions = new Vector2{
                        Height = reader.GetDouble(8),
                        Width = reader.GetDouble(9)
                    },
                    Picture = !reader.IsDBNull(10) ? reader["Picture"] as byte[] : null,
                    User = new User { 
                        Username = reader.GetString(11), 
                        Email = reader.GetString(12), 
                        PhoneNumber = reader.GetString(13) 
                    },
                    CreatedAt = reader.GetDateTime(14),
                    IsSold = reader.GetBoolean(15)
                };
            }
            return null;
        } catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving advert by ID: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Advert>> GetAdvertsByUserIdAsync(int userId, int offset = 0, int limit = 100)
    {
        var adverts = new List<Advert>();
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                @"SELECT 
                    a.""AdvertId"", 
                    a.""Title"", 
                    a.""Description"", 
                    a.""Price"", 
                    a.""PieceAmount"", 
                    a.""BoxDimHeight"", 
                    a.""BoxDimWidth"", 
                    a.""BoxDimDepth"", 
                    a.""PuzzleDimHeight"", 
                    a.""PuzzleDimWidth"", 
                    a.""Picture"", 
                    u.""Username"", 
                    u.""Email"", 
                    u.""PhoneNumber"", 
                    a.""CreatedAt"", 
                    a.""IsSold"" 
                FROM 
                    ""Advert"" a 
                    LEFT JOIN ""UserAccounts"" u ON a.""UserId"" = u.""UserId""
                WHERE 
                    a.""UserId"" = @userId
                OFFSET 
                    @offset 
                LIMIT 
                    @limit",
                conn
            );

            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("offset", offset);
            cmd.Parameters.AddWithValue("limit", limit);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                adverts.Add(new Advert
                {
                    AdvertId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Price = reader.GetDouble(3),
                    PieceAmount = reader.GetInt32(4),
                    BoxDimensions = new Vector3
                    {
                        Height = reader.GetDouble(5),
                        Width = reader.GetDouble(6),
                        Depth = reader.GetDouble(7)
                    },
                    PuzzleDimensions = new Vector2
                    {
                        Height = reader.GetDouble(8),
                        Width = reader.GetDouble(9)
                    },
                    Picture = !reader.IsDBNull(10) ? reader["Picture"] as byte[] : null,
                    User = new User
                    {
                        Username = reader.GetString(11),
                        Email = reader.GetString(12),
                        PhoneNumber = reader.GetString(13)
                    },
                    CreatedAt = reader.GetDateTime(14),
                    IsSold = reader.GetBoolean(15)
                });
            }
            return adverts;
        } catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving advert by User ID: {ex.Message}");
            return adverts;
        }
    }

    public async Task<Advert> UpdateAdvertAsync(Advert advert)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                @"UPDATE 
                    ""Adverts"" 
                SET 
                    ""Title"" = @title, 
                    ""Description"" = @description, 
                    ""Price"" = @price, 
                    ""PieceAmount"" = @pieceamount, 
                    ""BoxDimHeight"" = @boxdimheight, 
                    ""BoxDimWidth"" = @boxdimwidth, 
                    ""BoxDimDepth"" = @boxdimdepth, 
                    ""PuzzleDimHeight"" = @puzzledimheight, 
                    ""PuzzleDimWidth"" = @puzzledimwidth, 
                    ""Picture"" = @picture, 
                    ""IsSold"" = @issold 
                WHERE 
                    ""AdvertId"" = @advertId",
                conn
            );

            cmd.Parameters.AddWithValue("advertId", advert.AdvertId);
            cmd.Parameters.AddWithValue("title", advert.Title);
            cmd.Parameters.AddWithValue("description", advert.Description);
            cmd.Parameters.AddWithValue("price", advert.Price);
            cmd.Parameters.AddWithValue("pieceamount", advert.PieceAmount);
            cmd.Parameters.AddWithValue("boxdimheight", advert.BoxDimensions.Height);
            cmd.Parameters.AddWithValue("boxdimwidth", advert.BoxDimensions.Width);
            cmd.Parameters.AddWithValue("boxdimdepth", advert.BoxDimensions.Depth);
            cmd.Parameters.AddWithValue("puzzledimheight", advert.PuzzleDimensions.Height);
            cmd.Parameters.AddWithValue("puzzledimwidth", advert.PuzzleDimensions.Width);
            cmd.Parameters.AddWithValue("picture", (object?)advert.Picture ?? DBNull.Value);
            cmd.Parameters.AddWithValue("issold", advert.IsSold);

            await cmd.ExecuteNonQueryAsync();
            return advert;
        } catch (Exception ex)
        {
            Console.WriteLine($"Error updating advert: {ex.Message}");
            return null;
        }
    }
    
}
