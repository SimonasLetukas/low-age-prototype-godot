using Godot;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Shared;

/// <summary>
/// Responsible for: <para />
/// - Keeping track of all tiles data <para />
/// - Handling all tile-map related visuals <para />
/// - Conversions between global and map positions
/// </summary>
public class Tiles : Node2D
{
    private Vector2 _mapSize;
    private ICollection<(Vector2, Terrain)> _tiles;
    private Vector2 _tilemapOffset;
    private Vector2 _tileOffset;
    private int _mountainsFillOffset;
    private TileMap _grass;
    private TileMap _scraps;
    private TileMap _marsh;
    private TileMap _mountains;
    private FocusedTile _focusedTile;
    private TileMap _availableTiles;
    private TileMap _path;
    
    private const int TileMapGrassIndex = 6;
    private const int TileMapCelestiumIndex = 7;
    private const int TileMapScrapsIndex = 5;
    private const int TileMapMarshIndex = 3;
    private const int TileMapMountainsIndex = 4;
    private const int TileMapAvailableTileIndex = 8;
    private const int TileMapPathTileIndex = 9;
    
    private readonly Dictionary<Terrain, int> _tileMapIndexesByTerrain = new Dictionary<Terrain, int>
    {
        { Terrain.Grass,     TileMapGrassIndex },
        { Terrain.Mountains, TileMapMountainsIndex },
        { Terrain.Marsh,     TileMapMarshIndex },
        { Terrain.Scraps,    TileMapScrapsIndex },
        { Terrain.Celestium, TileMapCelestiumIndex }
    };

    public override void _Ready()
    {
        _grass = GetNode<TileMap>("Grass");
        _scraps = GetNode<TileMap>("Scraps");
        _marsh = GetNode<TileMap>("Marsh");
        _mountains = GetNode<TileMap>("Stone");
        _focusedTile = GetNode<FocusedTile>("FocusedTile");
        _availableTiles = GetNode<TileMap>("Alpha/Available");
        _path = GetNode<TileMap>("Path");
        
        _focusedTile.Disable();
    }

    public void Initialize(Vector2 mapSize, ICollection<(Vector2, Terrain)> tiles)
    {
        _mapSize = mapSize;
        _tiles = tiles;
        _tilemapOffset = new Vector2(mapSize.x / 2, (mapSize.y / 2) * -1);
        _mountainsFillOffset = (int)Mathf.Max(mapSize.x, mapSize.y);
        _tileOffset = new Vector2(0, (float)Constants.TileHeight / 2);
        ClearTilemaps();
        
        foreach (var (coordinates, terrain) in tiles)
        {
            _grass.SetCellv(coordinates, TileMapGrassIndex); // needed to fill up gaps
            SetCell(coordinates, terrain);
        }
    }

    public Vector2 GetMapPositionFromGlobalPosition(Vector2 globalPosition) 
        => _grass.WorldToMap(globalPosition) - _tilemapOffset;
    
    public Vector2 GetGlobalPositionFromMapPosition(Vector2 mapPosition) 
        => _grass.MapToWorld(mapPosition + _tilemapOffset, true) + _tileOffset;

    public Vector2[] GetGlobalPositionsFromMapPositions(IEnumerable<Vector2> mapPositions) 
        => mapPositions.Select(GetGlobalPositionFromMapPosition).ToArray();

    public Terrain GetTerrain(Vector2 at) => at.IsInBoundsOf(_mapSize)
        ? _tiles.SingleOrDefault(x => x.Item1.Equals(at)).Item2
        : Terrain.Mountains;

    public void MoveFocusedTileTo(Vector2 position)
    {
        _focusedTile.Enable();
        _focusedTile.MoveTo(GetGlobalPositionFromMapPosition(position));
    }

    public void SetAvailableTiles(IEnumerable<Vector2> availablePositions)
    {
        ClearAvailableTiles();

        foreach (var availablePosition in availablePositions)
        {
            _availableTiles.SetCellv(availablePosition, TileMapAvailableTileIndex);
            _availableTiles.UpdateBitmaskRegion(availablePosition);
        }
    }

    public bool IsCurrentlyAvailable(Vector2 mapPosition) 
        => _availableTiles.GetCellv(mapPosition) == TileMapAvailableTileIndex;

    public void SetPathTiles(IEnumerable<Vector2> pathPositions)
    {
        ClearPath();

        foreach (var pathPosition in pathPositions)
        {
            _path.SetCellv(pathPosition, TileMapPathTileIndex);
        }
    }

    public void FillMapOutsideWithMountains()
    {
        for (var y = _mountainsFillOffset * -1; y < _mapSize.y + _mountainsFillOffset; y++) 
        for (var x = _mountainsFillOffset * -1; x < _mapSize.x + _mountainsFillOffset; x++) 
            if (x < 0 || x >= _mapSize.x || y < 0 || y >= _mapSize.y)
                _mountains.SetCell(x, y, TileMapMountainsIndex);
    }

    public void UpdateALlBitmaps()
    {
        _grass.UpdateBitmaskRegion(Vector2.Zero, new Vector2(_mapSize.x, _mapSize.y));
        _scraps.UpdateBitmaskRegion(Vector2.Zero, new Vector2(_mapSize.x, _mapSize.y));
        _marsh.UpdateBitmaskRegion(Vector2.Zero, new Vector2(_mapSize.x, _mapSize.y));
        _mountains.UpdateBitmaskRegion(
            new Vector2(_mountainsFillOffset * -1, _mountainsFillOffset * -1), 
            new Vector2(_mapSize.x + _mountainsFillOffset, _mapSize.y + _mountainsFillOffset));
    }

    public void ClearPath() => _path.Clear();
    
    public void ClearAvailableTiles() => _availableTiles.Clear();

    private void SetCell(Vector2 at, Terrain terrain)
    {
        var tilemapIndex = GetTilemapIndexFrom(terrain);
        
        switch (tilemapIndex)
        {
            case TileMapMountainsIndex:
                _mountains.SetCellv(at, TileMapMountainsIndex);
                break;
            case TileMapMarshIndex:
                _marsh.SetCellv(at, TileMapMarshIndex);
                break;
            case TileMapScrapsIndex:
                _scraps.SetCellv(at, TileMapScrapsIndex);
                break;
            case TileMapCelestiumIndex:
                _grass.SetCellv(at, TileMapCelestiumIndex);
                break;
            case TileMapGrassIndex:
            default:
                _grass.SetCellv(at, TileMapGrassIndex);
                break;
        }
    }
    
    private int GetTilemapIndexFrom(Terrain terrain) => _tileMapIndexesByTerrain.ContainsKey(terrain) 
        ? _tileMapIndexesByTerrain[terrain] 
        : TileMapGrassIndex;
    
    private void ClearTilemaps()
    {
        ClearPath();
        ClearAvailableTiles();
        _grass.Clear();
        _scraps.Clear();
        _marsh.Clear();
        _mountains.Clear();
    }
}
