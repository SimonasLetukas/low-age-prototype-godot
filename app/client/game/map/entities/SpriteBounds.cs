using Godot;

public readonly struct SpriteBounds
{
    private readonly float _minX;
    private readonly float _minY;
    private readonly float _maxX;
    private readonly float _maxY;
    
    public SpriteBounds(Vector2 min, Vector2 max)
    {
        _minX = min.x;
        _minY = min.y;
        _maxX = max.x;
        _maxY = max.y;
    }

    public bool Intersects(SpriteBounds otherBounds)
    {
        if (_minX > otherBounds._maxX || otherBounds._minX > _maxX)
            return false;

        if (_maxY < otherBounds._minY || otherBounds._maxY < _minY)
            return false;

        return true;
    }

    public override string ToString()
    {
        return "Min: (" + _minX + ", " + _minY + ")  Max: (" + _maxX + ", " + _maxY + ")";
    }
}
