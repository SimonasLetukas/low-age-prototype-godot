using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Responsible for handling all tile-map related visuals
/// and conversions between global and map positions.
/// </summary>
public class Tiles : Node2D
{
    private Vector2 _mapSize;
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

    public void Initialize(Vector2 mapSize)
    {
        _mapSize = mapSize;
        _tilemapOffset = new Vector2(mapSize.x / 2, (mapSize.y / 2) * -1);
        _mountainsFillOffset = (int)Mathf.Max(mapSize.x, mapSize.y);
        _tileOffset = new Vector2(0, (float)Constants.TileHeight / 2);
        ClearTilemaps();
        
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                _grass.SetCellv(new Vector2(x, y), TileMapGrassIndex);
            }
        }
    }

    public Vector2 GetMapPositionFromGlobalPosition(Vector2 globalPosition) 
        => _grass.WorldToMap(globalPosition) - _tilemapOffset;
    
    public Vector2 GetGlobalPositionFromMapPosition(Vector2 mapPosition) 
        => _grass.MapToWorld(mapPosition + _tilemapOffset, true) + _tileOffset;

    public Vector2[] GetGlobalPositionsFromMapPositions(IEnumerable<Vector2> mapPositions) 
        => mapPositions.Select(GetGlobalPositionFromMapPosition).ToArray();

    public int GetTilemapIndexFromTerrainIndex(Constants.Game.Terrain terrain)
    {
        switch (terrain)
        {
            case Constants.Game.Terrain.Grass:
                return TileMapGrassIndex;
            case Constants.Game.Terrain.Mountains:
                return TileMapMountainsIndex;
            case Constants.Game.Terrain.Marsh:
                return TileMapMarshIndex;
            case Constants.Game.Terrain.Scraps:
                return TileMapScrapsIndex;
            case Constants.Game.Terrain.Celestium:
                return TileMapCelestiumIndex;
            default:
                return TileMapGrassIndex;
        }
    }

    public void SetCell(Vector2 at, Constants.Game.Terrain terrain)
    {
        switch (terrain)
        {
            case Constants.Game.Terrain.Mountains:
                _mountains.SetCellv(at, TileMapMountainsIndex);
                break;
            case Constants.Game.Terrain.Marsh:
                _marsh.SetCellv(at, TileMapMarshIndex);
                break;
            case Constants.Game.Terrain.Scraps:
                _scraps.SetCellv(at, TileMapScrapsIndex);
                break;
            case Constants.Game.Terrain.Celestium:
                _grass.SetCellv(at, TileMapCelestiumIndex);
                break;
            case Constants.Game.Terrain.Grass:
            default:
                _grass.SetCellv(at, TileMapGrassIndex);
                break;
        }
    }

    public void SetCelestium(Vector2 at)
    {
        _grass.SetCellv(at, TileMapCelestiumIndex);
    }

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

    private void ClearPath() => _path.Clear();
    
    private void ClearAvailableTiles() => _availableTiles.Clear();

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
