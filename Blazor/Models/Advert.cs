using System;

public class Advert
{
    // Unique identifier for the advert
    int id { get; set; }

    // Name of the advert
    string name { get; set; }

    // Description of the advert
    string description { get; set; }

    // Price of the advert its a double because we want to allow decimal values 64-bit precision
    double price { get; set;  }

    Vector3 boxDimensions { get; set; }

    Vector2 puzzleDimensions { get; set; }

    DateTime createdAt { get; set; }
    bool isSold { get; set; }
    bool isActive { get; set; }
}
