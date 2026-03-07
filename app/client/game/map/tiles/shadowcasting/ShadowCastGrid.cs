using System;
using System.Collections.Generic;
using LowAgeCommon;

public interface IShadowCastGrid
{
    public Vector2Int Size { get; }
    public bool IsWall(int x, int y);
    public void SetLight(int x, int y);
    public IEnumerable<Vector2Int> GetLitPositions();
}

public class ShadowCastGrid(
    Vector2Int startingPosition, 
    Vector2Int size, 
    Func<int, int, bool> isWall) : IShadowCastGrid
{
    public Vector2Int Size { get; } = size * 2;

    private readonly Vector2Int _offset = size;
    private readonly HashSet<Vector2Int> _revealedTiles = [];

    public bool IsWall(int x, int y) => isWall(startingPosition.X + x - _offset.X, startingPosition.Y + y - _offset.Y);

    public void SetLight(int x, int y)
    {
        var position = new Vector2Int(x, y) + startingPosition - _offset;
        _revealedTiles.Add(position);
    }

    public IEnumerable<Vector2Int> GetLitPositions() => _revealedTiles;
}