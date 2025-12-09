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
                    ""Advert"" (
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
                @"SELECT 
                    COUNT(*) 
                FROM 
                    ""Advert"" 
                WHERE 
                    ""IsSold"" = FALSE",
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

    public async Task<List<Advert>> GetAllAdvertsAsync(int offset = 0, int limit = 100, bool? isSold = false, string? searchTerm = null)
    {
        var adverts = new List<Advert>();
        List<string> conditions = new List<string>();
        List<List<string>> OrConditions = new List<List<string>>();

        string whereString = "";

        if (isSold.HasValue)
        {
            conditions.Add($@"a.""IsSold"" = @isSold");
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            OrConditions.Add(new List<string>
            {
                $@"a.""Title"" ILIKE '%' || @searchTerm || '%'",
                $@"a.""Description"" ILIKE '%' || @searchTerm || '%'"
            });
        }
        
        conditions.AddRange(OrConditions.Select(orGroup => "(" + string.Join(" OR ", orGroup) + ")"));
        whereString = string.Join(" AND ", conditions);

        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                $@"SELECT 
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
                    {whereString}
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

            if (isSold.HasValue)
            {
                cmd.Parameters.AddWithValue("isSold", isSold.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                cmd.Parameters.AddWithValue("searchTerm", searchTerm);
            }

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
                        UserId = reader.GetInt32(11),
                        Username = reader.GetString(12), 
                        Email = reader.GetString(13), 
                        PhoneNumber = reader.GetString(14) 
                    },
                    CreatedAt = reader.GetDateTime(15),
                    IsSold = reader.GetBoolean(16)
                };
            }
            return null;
        } catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving advert by ID: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Advert>> GetAdvertsByUserAsync(string username, int offset = 0, int limit = 100)
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
                    u.""Username"" = @username
                ORDER BY 
                    a.""IsSold"" ASC,
                    a.""CreatedAt"" DESC
                OFFSET 
                    @offset 
                LIMIT 
                    @limit",
                conn
            );

            cmd.Parameters.AddWithValue("username", username);
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

            //updates the advert by id when edited
            using var cmd = new NpgsqlCommand(
                @"UPDATE 
                    ""Advert"" 
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


    public async Task DeleteAdvertAsync(int advertId)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                @"DELETE FROM 
                    ""Advert"" 
                WHERE 
                    ""AdvertId"" = @advertId",
                conn
            );

            cmd.Parameters.AddWithValue("advertId", advertId);
            await cmd.ExecuteNonQueryAsync();
        } catch (Exception ex)
        {
            Console.WriteLine($"Error deleting advert: {ex.Message}");
        }
    }
}