using Npgsql;

/// <summary>
/// Provides operations for managing adverts, including creation, retrieval, updating, and deletion.
/// Handles SQL query construction and parameterization for flexible advert filtering and sorting.
/// </summary>
public class AdvertService
{
    /// <summary>
    /// The connection string used to connect to the database.
    /// </summary>
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdvertService"/> class using the default connection string.
    /// </summary>
    public AdvertService()
    {
        // Fetches the connection string from its class
        _connectionString = ConnectionString.DefaultConnection;
    }

    /// <summary>
    /// Constructs a SQL WHERE clause string based on provided filter parameters.
    /// </summary>
    /// <param name="isSold">Filter by sold status.</param>
    /// <param name="searchTerm">Filter by search term in title or description.</param>
    /// <param name="username">Filter by username.</param>
    /// <param name="advertId">Filter by advert ID.</param>
    /// <param name="minPrice">Minimum price filter.</param>
    /// <param name="maxPrice">Maximum price filter.</param>
    /// <param name="minPieceAmount">Minimum piece amount filter.</param>
    /// <param name="maxPieceAmount">Maximum piece amount filter.</param>
    /// <returns>A SQL WHERE clause string.</returns>
    private string GetWhereClause(
        // has default values
        bool? isSold = false, 
        string? searchTerm = null, 
        string? username = null, 
        int? advertId = null,
        double? minPrice = null,
        double? maxPrice = null,
        int? minPieceAmount = null,
        int? maxPieceAmount = null
    )
    {
        List<string> conditions = new List<string>();
        List<List<string>> OrConditions = new List<List<string>>();

        string whereString = "";

        // Add conditions for each filter parameter if provided
        if (isSold.HasValue)
        {
            conditions.Add($@"a.""IsSold"" = @isSold");
        }

        // Add OR conditions for search term in title or description
        if (!string.IsNullOrEmpty(searchTerm))
        {
            OrConditions.Add(new List<string>
            // % means text can continue in that direction and || is works like + string
            {
                $@"a.""Title"" ILIKE '%' || @searchTerm || '%'",
                $@"a.""Description"" ILIKE '%' || @searchTerm || '%'"
            });
        }
        if (!string.IsNullOrEmpty(username))
        {
            conditions.Add($@"u.""Username"" = @username");
        }
        if (advertId.HasValue)
        {
            conditions.Add($@"a.""AdvertId"" = @advertId");
        }
        if (minPrice.HasValue)
        {
            // >= and <= is used to return a range
            conditions.Add($@"a.""Price"" >= @minPrice");
        }
        if (maxPrice.HasValue)
        {
            conditions.Add($@"a.""Price"" <= @maxPrice");
        }
        if (minPieceAmount.HasValue)
        {
            conditions.Add($@"a.""PieceAmount"" >= @minPieceAmount");
        }
        if (maxPieceAmount.HasValue)
        {
            conditions.Add($@"a.""PieceAmount"" <= @maxPieceAmount");
        }

        // Adds the OrConditions to conditions in the SQL format and then adds it to the wherestring sepperated by " AND "
        conditions.AddRange(OrConditions.Select(orGroup => "(" + string.Join(" OR ", orGroup) + ")"));
        whereString = string.Join(" AND ", conditions);
        return whereString;
    }

    /// <summary>
    /// Adds parameters to a <see cref="NpgsqlCommand"/> based on provided filter values.
    /// </summary>
    private void AddWhereParameters(NpgsqlCommand cmd, 
        bool? isSold = false, 
        string? searchTerm = null, 
        string? username = null, 
        int? advertId = null,
        double? minPrice = null,
        double? maxPrice = null,
        int? minPieceAmount = null,
        int? maxPieceAmount = null
    )
    {
        // Sets the parameters in the cmd if they are not null
        if (isSold.HasValue)
        {
            cmd.Parameters.AddWithValue("isSold", isSold.Value);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            cmd.Parameters.AddWithValue("searchTerm", searchTerm);
        }
        if (!string.IsNullOrEmpty(username))
        {
            cmd.Parameters.AddWithValue("username", username);
        }
        if (advertId.HasValue)
        {
            cmd.Parameters.AddWithValue("advertId", advertId.Value);
        }

        if (minPrice.HasValue)
        {
            cmd.Parameters.AddWithValue("minPrice", minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            cmd.Parameters.AddWithValue("maxPrice", maxPrice.Value);
        }
        if (minPieceAmount.HasValue)
        {
            cmd.Parameters.AddWithValue("minPieceAmount", minPieceAmount.Value);
        }
        if (maxPieceAmount.HasValue)
        {
            cmd.Parameters.AddWithValue("maxPieceAmount", maxPieceAmount.Value);
        }
    }

    /// <summary>
    /// Constructs a list of orderings for the SQL ORDER BY clause.
    /// </summary>
    /// <param name="orderBy">The field to order by.</param>
    /// <param name="orderDirection">The direction to order (ASC/DESC).</param>
    /// <returns>A list of orderings for the ORDER BY clause.</returns>
    private List<List<string>> GetOrderByClause(string? orderBy = null, string? orderDirection = null)
    {
        List<List<string>> orderings = new List<List<string>>();

        // Add custom ordering if provided
        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDirection))
        {
            orderings.Add(new List<string>
            {
                orderBy,
                orderDirection
            });
        }
        // Default ordering: first by IsSold ascending, then by CreatedAt descending
        orderings.Add(new List<string>
        {
            "IsSold",
            "ASC"
        });
        orderings.Add(new List<string>
        {
            "CreatedAt",
            "DESC"
        });

        return orderings;

    }

    /// <summary>
    /// Asynchronously adds a new advert to the database.
    /// </summary>
    /// <param name="advert">The <see cref="Advert"/> object to add.</param>
    public async Task AddAdvertAsync(Advert advert)
    {
        try
        {
            // Open a new database connection
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Prepare the SQL command for inserting a new advert
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

            // Add parameters for the advert fields
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
            // Log the error if advert creation fails
            Console.WriteLine($"Error adding advert: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously retrieves the count, maximum price, and maximum piece amount for adverts matching the given filters.
    /// </summary>
    /// <param name="isSold">Filter by sold status.</param>
    /// <param name="searchTerm">Filter by search term in title or description.</param>
    /// <param name="minPrice">Minimum price filter.</param>
    /// <param name="maxPrice">Maximum price filter.</param>
    /// <param name="minPieceAmount">Minimum piece amount filter.</param>
    /// <param name="maxPieceAmount">Maximum piece amount filter.</param>
    /// <param name="username">Filter by username.</param>
    /// <returns>An array containing: [count, max price, max piece amount].</returns>
    public async Task<dynamic[]> GetAdvertLimits(
        bool? isSold = false, 
        string? searchTerm = null, 
        double? minPrice = null, 
        double? maxPrice = null, 
        int? minPieceAmount = null, 
        int? maxPieceAmount = null,
        string? username = null
    )
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(
                $@"SELECT 
                    COUNT(*),
                    MAX(a.""Price"") as MaxPrice,
                    MAX(a.""PieceAmount"") as MaxPieceAmount
                FROM 
                    ""Advert"" a
                    LEFT JOIN ""UserAccounts"" u ON a.""UserId"" = u.""UserId""
                WHERE 
                    {
                    // returns WHERE criteria
                        GetWhereClause(
                            isSold: isSold, 
                            searchTerm: searchTerm, 
                            minPrice: minPrice, 
                            maxPrice: maxPrice, 
                            minPieceAmount: minPieceAmount, 
                            maxPieceAmount: maxPieceAmount,
                            username: username
                        )
                    }",
                conn
            );
            // replaces the parameters in the WHERE string
            AddWhereParameters(cmd, 
                isSold: isSold, 
                searchTerm: searchTerm, 
                minPrice: minPrice, 
                maxPrice: maxPrice, 
                minPieceAmount: minPieceAmount, 
                maxPieceAmount: maxPieceAmount,
                username: username
            );
            // reads the output of the command
            var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // gets the size of the exstracted table, the highest price and piece amount in the table
                int count = reader.GetInt32(0);
                double maxPriceResult = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                int maxPieceAmountResult = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                // a dynamic array can hold different data types
                return new dynamic[] { count, maxPriceResult, maxPieceAmountResult };
            }
        } catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving advert count: {ex.Message} {ex.StackTrace}");
        }
        return new dynamic[] { 0, 0, 0 };
    }


    /// <summary>
    /// Asynchronously retrieves the maximum advert price for adverts matching the given filters.
    /// </summary>
    /// <param name="isSold">Filter by sold status.</param>
    /// <param name="searchTerm">Filter by search term in title or description.</param>
    /// <returns>The maximum price found, or 0 if none.</returns>
    public async Task<double> GetMaxAdvertPriceAsync(bool? isSold = false, string? searchTerm = null)
    {
        var limits = await GetAdvertLimits(isSold, searchTerm);
        return limits[1];
    }


    /// <summary>
    /// Asynchronously retrieves the maximum piece amount for adverts matching the given filters.
    /// </summary>
    /// <param name="isSold">Filter by sold status.</param>
    /// <param name="searchTerm">Filter by search term in title or description.</param>
    /// <returns>The maximum piece amount found, or 0 if none.</returns>
    public async Task<int> GetMaxAdvertPieceAmountAsync(bool? isSold = false, string? searchTerm = null)
    {
        var limits = await GetAdvertLimits(isSold, searchTerm);
        return limits[2];
    }


    /// <summary>
    /// Asynchronously retrieves the count of adverts matching the given filters.
    /// </summary>
    /// <param name="isSold">Filter by sold status.</param>
    /// <param name="searchTerm">Filter by search term in title or description.</param>
    /// <param name="minPrice">Minimum price filter.</param>
    /// <param name="maxPrice">Maximum price filter.</param>
    /// <param name="minPieceAmount">Minimum piece amount filter.</param>
    /// <param name="maxPieceAmount">Maximum piece amount filter.</param>
    /// <param name="username">Filter by username.</param>
    /// <returns>The count of adverts found.</returns>
    public async Task<int> GetAdvertCountAsync(
        bool? isSold = false, 
        string? searchTerm = null, 
        double? minPrice = null, 
        double? maxPrice = null,
        int? minPieceAmount = null, 
        int? maxPieceAmount = null,
        string? username = null
    )
    {
        var limits = await GetAdvertLimits(isSold, searchTerm, minPrice, maxPrice, minPieceAmount, maxPieceAmount, username);
        return limits[0];
    }


    /// <summary>
    /// Asynchronously retrieves a list of adverts matching the given filters, with pagination and ordering.
    /// </summary>
    /// <param name="offset">The number of records to skip (for pagination).</param>
    /// <param name="limit">The maximum number of records to return.</param>
    /// <param name="isSold">Filter by sold status.</param>
    /// <param name="searchTerm">Filter by search term in title or description.</param>
    /// <param name="username">Filter by username.</param>
    /// <param name="advertId">Filter by advert ID.</param>
    /// <param name="minPrice">Minimum price filter.</param>
    /// <param name="maxPrice">Maximum price filter.</param>
    /// <param name="minPieceAmount">Minimum piece amount filter.</param>
    /// <param name="maxPieceAmount">Maximum piece amount filter.</param>
    /// <param name="orderBy">Field to order by.</param>
    /// <param name="orderDirection">Order direction (ASC/DESC).</param>
    /// <returns>A list of adverts matching the filters.</returns>
    public async Task<List<Advert>> GetAllAdvertsAsync(
        int offset = 0, 
        int limit = 100, 
        bool? isSold = false, 
        string? searchTerm = null, 
        string? username = null, 
        int? advertId = null, 
        double? minPrice = null, 
        double? maxPrice = null,
        int? minPieceAmount = null, 
        int? maxPieceAmount = null,
        string? orderBy = null,
        string? orderDirection = null
    )
    {
        var adverts = new List<Advert>();

        // creates WHERE string
        string whereString = GetWhereClause(
            isSold: isSold, 
            searchTerm: searchTerm, 
            username: username, 
            advertId: advertId, 
            minPrice: minPrice, 
            maxPrice: maxPrice,
            minPieceAmount: minPieceAmount, 
            maxPieceAmount: maxPieceAmount
        );

        // gets a list of lists that determine the order of the adverts
        var orderings = GetOrderByClause(orderBy, orderDirection);

        // turns all the lists into a string of orders
        string orderByString = string.Join(", ", orderings.Select(o => $@"a.""{o[0]}"" {o[1]}"));

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
                    u.""UserId"", -- FROM USER OBJECT
                    u.""Username"", -- FROM USER OBJECT
                    u.""Email"", -- FROM USER OBJECT
                    u.""PhoneNumber"", -- FROM USER OBJECT
                    a.""CreatedAt"", 
                    a.""IsSold"" 
                FROM 
                    ""Advert"" a 
                    LEFT JOIN ""UserAccounts"" u ON a.""UserId"" = u.""UserId""
                WHERE 
                    {whereString}
                ORDER BY 
                    {orderByString}
                OFFSET 
                    @offset 
                LIMIT 
                    @limit",
                conn
            );

            cmd.Parameters.AddWithValue("offset", offset);
            cmd.Parameters.AddWithValue("limit", limit);

            AddWhereParameters(cmd, 
                isSold: isSold, 
                searchTerm: searchTerm, 
                username: username, 
                advertId: advertId, 
                minPrice: minPrice, 
                maxPrice: maxPrice,
                minPieceAmount: minPieceAmount, 
                maxPieceAmount: maxPieceAmount
            );

            using var reader = await cmd.ExecuteReaderAsync();

            // read the output from the database and add it to the list
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


    /// <summary>
    /// Asynchronously retrieves a single advert by its ID.
    /// </summary>
    /// <param name="advertId">The ID of the advert to retrieve.</param>
    /// <returns>The <see cref="Advert"/> if found; otherwise, null.</returns>
    public async Task<Advert> GetAdvertByIdAsync(int advertId)
    {
        var adverts = await GetAllAdvertsAsync(advertId: advertId);
        return adverts.FirstOrDefault();
    }


    /// <summary>
    /// Asynchronously retrieves all adverts created by a specific user, with pagination.
    /// </summary>
    /// <param name="username">The username to filter adverts by.</param>
    /// <param name="offset">The number of records to skip (for pagination).</param>
    /// <param name="limit">The maximum number of records to return.</param>
    /// <returns>A list of adverts created by the specified user.</returns>
    public async Task<List<Advert>> GetAdvertsByUserAsync(string username, int offset = 0, int limit = 100)
    {
        return await GetAllAdvertsAsync(offset: offset, limit: limit, username: username, isSold: null);
    }


    /// <summary>
    /// Asynchronously updates an existing advert in the database.
    /// </summary>
    /// <param name="advert">The <see cref="Advert"/> object with updated values.</param>
    /// <returns>The updated <see cref="Advert"/> if successful; otherwise, null.</returns>
    public async Task<Advert> UpdateAdvertAsync(Advert advert)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Prepare the SQL command to update the advert by ID
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

            // Add parameters for the updated advert fields
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



    /// <summary>
    /// Asynchronously deletes an advert from the database by its ID.
    /// </summary>
    /// <param name="advertId">The ID of the advert to delete.</param>
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