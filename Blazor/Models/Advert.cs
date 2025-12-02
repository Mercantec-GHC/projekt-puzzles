using System;

public class Advert
{
    // Unique identifier for the advert
    public int Id { get; set; }

    // Name of the advert
    public string Title { get; set; }

    // Description of the advert
    public string Description { get; set; }

    // Price of the advert its a double because we want to allow decimal values 64-bit precision
    public double Price { get; set;  }

    public int PieceAmount { get; set; }

    public Vector3 BoxDimensions { get; set; }

    public Vector2 PuzzleDimensions { get; set; }

    public byte[] Picture { get; set; }

    public User User { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool IsSold { get; set; }
}
