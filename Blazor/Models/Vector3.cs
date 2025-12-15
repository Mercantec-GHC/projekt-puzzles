/// <summary>
/// Represents a three-dimensional vector, extending <see cref="Vector2"/> with a Z component.
/// Provides multiple aliases for the Z component (Z, Depth, Altitude, B) and for X and Y (R, G).
/// </summary>
/// <remarks>
/// Useful for mathematical operations in 3D space or as a color vector (RGB).
/// </remarks>
public class Vector3 : Vector2
{
	public double _Z;
	public double Z
	{
		get { return _Z; }
		set { _Z = value; }
	}
    public double Depth
    {
        get { return _Z; }
        set { _Z = value; }
    }

    public double Altitude
    {
        get { return _Z; }
        set { _Z = value; }
    }

    public double R {
        get { return _X; }
        set { _X = value; }
    }
    public double G
    {
        get { return _Y; }
        set { _Y = value; }
    }
    public double B
    {
        get { return _Z; }
        set { _Z = value; }
    }
}
