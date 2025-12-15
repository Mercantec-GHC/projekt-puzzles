using System;

/// <summary>
/// Represents an advert for a puzzle, including details such as title, description, price, dimensions, and user information.
/// </summary>
public class Advert
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Advert"/> class with default dimensions.
    /// </summary>
    public Advert()
    {
        // Initialize dimensions to default values
        BoxDimensions = new Vector3();
        PuzzleDimensions = new Vector2();
    }

    /// <summary>
    /// Gets or sets the unique identifier for the advert.
    /// </summary>
    public int AdvertId { get; set; }

    /// <summary>
    /// Gets or sets the title of the advert.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the advert.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the advert. Uses double for decimal values with 64-bit precision.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets the number of pieces in the puzzle.
    /// </summary>
    public int PieceAmount { get; set; }

    /// <summary>
    /// Gets or sets the dimensions of the puzzle box.
    /// </summary>
    public Vector3 BoxDimensions { get; set; }

    /// <summary>
    /// Gets or sets the dimensions of the puzzle itself.
    /// </summary>
    public Vector2 PuzzleDimensions { get; set; }

    /// <summary>
    /// Gets or sets the picture associated with the advert, stored as a byte array.
    /// </summary>
    public byte[] Picture { get; set; }

    /// <summary>
    /// Gets or sets the user who created the advert.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the advert was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the advert has been marked as sold.
    /// </summary>
    public bool IsSold { get; set; }
}
