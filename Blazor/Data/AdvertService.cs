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
                "INSERT INTO \"Adverts\" (\"Title\", \"Description\", \"Price\", \"PieceAmount\", \"BoxDimHeight\", \"BoxDimWidth\", \"BoxDimDepth\", \"PuzzleDimHeight\", \"PuzzleDimWidth\", \"Picture\", \"UserId\", \"CreatedAt\", \"IsSold\") VALUES (@title, @description, @price, @pieceamount, @boxdimheight, @boxdimwidth, @boxdimdepth, @puzzledimheight, @puzzledimwidth, @picture, @userid, @createdat, @issold)", 
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
                    u.""Username"", 
                    u.""Email"", 
                    u.""PhoneNumber"", 
                    a.""CreatedAt"", 
                    a.""IsSold"" 
                FROM 
                    ""Advert"" a 
                    LEFT JOIN ""UserAccounts"" u ON a.""UserId"" = u.""UserId"" 
                ORDER BY 
                    a.""CreatedAt"" DESC 
                OFFSET 
                    @offset 
                LIMIT 
                    @limit",
                conn
            );

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
            Console.WriteLine($"Error retrieving adverts: {ex.Message}");
            return adverts;
        }
    }
    
}
