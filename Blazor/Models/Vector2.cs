/// <summary>
/// Represents a two-dimensional vector with multiple property aliases for its components,
/// such as X/Y, Width/Height, and Latitude/Longitude.
/// </summary>
public class Vector2
{
    public double _X;
    public double _Y;
    public double Height
    {
        get { return _X; }
        set { _X = value; }
    }
    public double Width
    {
        get { return _Y; }
        set { _Y = value; }
    }

    public double Y
    {
        get { return _Y; }
        set { _Y = value; }
    }

    public double X
    {
        get { return _X; }
        set { _X = value; }
    }

    public double Latitude
    {
        get { return _X; }
        set { _X = value; }
    }
    public double Longitude
    {
        get { return _Y; }
        set { _Y = value; }
    }
}