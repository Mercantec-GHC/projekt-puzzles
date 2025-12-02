using System;

public class Advert
{
    // Unique identifier for the advert
    int Id { get; set; }

    // Name of the advert
    string Name { get; set; }

    // Description of the advert
    string Description { get; set; }

    // Price of the advert its a double because we want to allow decimal values 64-bit precision
    double Price { get; set;  }

    int PieceAmount { get; set; }

    Vector3 BoxDimensions { get; set; }

    Vector2 PuzzleDimensions { get; set; }

    byte[] Picture { get; set; }

    User UserId { get; set; }

    DateTime createdAt { get; set; }
    bool isSold { get; set; }
    bool isActive { get; set; }
}
