class Vector3 : Vector2
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
