using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using low_age_data.Domain.Common;
using low_age_data.Domain.Tiles;

/// <summary>
/// Responsible for: <para />
/// - Keeping track of all tiles data <para />
/// - Handling all tile-map related visuals <para />
/// - Conversions between global and map positions
/// </summary>
public class Tiles : Node2D
{
    public class TileInstance
    {
        public Vector2 Position { get; set; }
        public TileId Blueprint { get; set; }
        public Terrain Terrain { get; set; }
        public bool IsTarget { get; set; }
        public IList<EntityNode> Occupants { get; set; }

        public bool IsInBoundsOf(Vector2 bounds) => Position.IsInBoundsOf(bounds);
    }
    
    public event Action FinishedInitializing = delegate { };
    
    public StructureFoundations StructureFoundations;
    
    private ICollection<TileInstance> _tiles = new List<TileInstance>();
    private IList<Tile> _tilesBlueprint;
    private Vector2 _mapSize;
    private Vector2 _tilemapOffset;
    private Vector2 _tileOffset;
    private int _mountainsFillOffset;
    private TileMap _grass;
    private TileMap _scraps;
    private TileMap _marsh;
    private TileMap _mountains;
    private FocusedTile _focusedTile;
    private TileMap _availableTilesVisual;
    private TileMap _availableTilesHovering;
    private IEnumerable<Vector2> _availableTilesSource = new List<Vector2>();
    private TileMap _targetTiles;
    private TileMap _targetMapPositiveTiles;
    private TileMap _targetMapNegativeTiles;
    private readonly ICollection<TileInstance> _targetTileInstances = new List<TileInstance>();
    private TileMap _path;
    
    private const int TileMapGrassIndex = 6;
    private const int TileMapCelestiumIndex = 7;
    private const int TileMapScrapsIndex = 5;
    private const int TileMapMarshIndex = 3;
    private const int TileMapMountainsIndex = 4;
    private const int TileMapAvailableTileIndex = 8;
    private const int TileMapPathTileIndex = 9;
    private const int TileMapNegativeTargetTileIndex = 11;
    private const int TileMapPositiveTargetTileIndex = 12;
    
    private readonly Dictionary<Terrain, int> _tileMapIndexesByTerrain = new Dictionary<Terrain, int>
    {
        { Terrain.Grass,     TileMapGrassIndex },
        { Terrain.Mountains, TileMapMountainsIndex },
        { Terrain.Marsh,     TileMapMarshIndex },
        { Terrain.Scraps,    TileMapScrapsIndex },
        { Terrain.Celestium, TileMapCelestiumIndex }
    };

    private bool _dataInitialized = false;
    private IList<(Vector2, TileId)> _tilesForDataInitialization;
    private bool _visualsInitialized = false;
    private IList<(Vector2, TileId)> _tilesForVisualsInitialization;
    private bool _initialized = true;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
        
        _grass = GetNode<TileMap>("Grass");
        _scraps = GetNode<TileMap>("Scraps");
        _marsh = GetNode<TileMap>("Marsh");
        _mountains = GetNode<TileMap>("Stone");

        StructureFoundations = GetNode<StructureFoundations>($"{nameof(StructureFoundations)}");
        
        _availableTilesVisual = GetNode<TileMap>("Alpha/Available");
        _availableTilesHovering = GetNode<TileMap>("Alpha/AvailableHovering");
        _targetTiles = GetNode<TileMap>("Alpha/Target");
        _targetMapPositiveTiles = GetNode<TileMap>("Alpha/TargetMapPositive");
        _targetMapPositiveTiles.Visible = false;
        _targetMapNegativeTiles = GetNode<TileMap>("Alpha/TargetMapNegative");
        _targetMapNegativeTiles.Visible = false;
        
        _path = GetNode<TileMap>("Path");
        
        _focusedTile = GetNode<FocusedTile>("FocusedTile");
        _focusedTile.Disable();
    }

    public void Initialize(Vector2 mapSize, ICollection<(Vector2, TileId)> tiles)
    {
        _tilesBlueprint = Data.Instance.Blueprint.Tiles;
        _mapSize = mapSize;
        _tilemapOffset = new Vector2(mapSize.x / 2, (mapSize.y / 2) * -1);
        _mountainsFillOffset = (int)Mathf.Max(mapSize.x, mapSize.y);
        _tileOffset = new Vector2(1, (float)Constants.TileHeight / 2);
        ClearTilemaps();
        _tilesForDataInitialization = tiles.ToList();
        _tilesForVisualsInitialization = tiles.ToList();
        
        _initialized = false;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (_initialized)
            return;
        
        CheckInitialization();
        
        var deltaMs = (int)(delta * 1000);
        _stopwatch.Reset();
        _stopwatch.Start();

        while (_stopwatch.ElapsedMilliseconds < deltaMs)
        {
            if (_dataInitialized is false)
            {
                IterateDataInitialization();
                continue;
            }

            if (_visualsInitialized is false) 
                IterateVisualInitialization();
        }
        
        _stopwatch.Stop();
    }

    private void CheckInitialization()
    {
        if (_dataInitialized is false || _visualsInitialized is false)
            return;

        _initialized = true;
        FinishedInitializing();
    }

    private void IterateDataInitialization()
    {
        if (_tilesForDataInitialization.IsEmpty())
        {
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(Tiles)}.{nameof(IterateDataInitialization)} completed");
            _dataInitialized = true;
            return;
        }

        var (position, blueprintId) = _tilesForDataInitialization[0];
        _tilesForDataInitialization.RemoveAt(0);
        _tiles.Add(new TileInstance
        {
            Position = position,
            Blueprint = blueprintId,
            Terrain = GetBlueprint(blueprintId).Terrain,
            Occupants = new List<EntityNode>()
        });
    }

    private void IterateVisualInitialization()
    {
        if (_tilesForVisualsInitialization.IsEmpty())
        {
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(Tiles)}.{nameof(IterateVisualInitialization)} completed");
            _visualsInitialized = true;
            return;
        }

        var (position, blueprintId) = _tilesForVisualsInitialization[0];
        _tilesForVisualsInitialization.RemoveAt(0);
        
        _grass.SetCellv(position, TileMapGrassIndex); // needed to fill up gaps
        SetCell(position, GetTerrain(position));

        _targetMapPositiveTiles.SetCellv(position, TileMapPositiveTargetTileIndex);
        _targetMapNegativeTiles.SetCellv(position, TileMapNegativeTargetTileIndex);
    }

    public Vector2 GetMapPositionFromGlobalPosition(Vector2 globalPosition) 
        => _grass.WorldToMap(globalPosition) - _tilemapOffset;
    
    public Vector2 GetGlobalPositionFromMapPosition(Vector2 mapPosition) 
        => _grass.MapToWorld(mapPosition + _tilemapOffset, true) + _tileOffset;

    public Vector2[] GetGlobalPositionsFromMapPositions(IEnumerable<Vector2> mapPositions) 
        => mapPositions.Select(GetGlobalPositionFromMapPosition).ToArray();

    public Terrain GetTerrain(TileInstance tile) => tile is null 
        ? Terrain.Mountains 
        : GetTerrain(tile.Position);
    
    public Terrain GetTerrain(Vector2 at) => at.IsInBoundsOf(_mapSize)
        ? GetTile(at).Terrain
        : Terrain.Mountains;

    public IList<TileInstance> GetTiles(IList<Vector2> at) => at.Select(GetTile).ToList();

    public TileInstance GetTile(Vector2 at) => _tiles.SingleOrDefault(x => x.Position.Equals(at));

    public Tile GetBlueprint(TileId of) => _tilesBlueprint.SingleOrDefault(x => x.Id.Equals(of));

    public bool IsOccupied(Vector2 at, EntityNode by = null) => IsOccupied(GetTile(at), by);

    public bool IsOccupied(TileInstance tile, EntityNode by = null) => by is null 
        ? tile.Occupants.Any() 
        : tile.Occupants.Contains(by);

    public void AddOccupation(EntityNode entity)
    {
        foreach (var position in entity.EntityOccupyingPositions)
        {
            var tile = GetTile(position);
            if (IsOccupied(tile, entity))
                continue;
            
            tile.Occupants.Add(entity);
        }
        
        if (entity is StructureNode structure)
            StructureFoundations.AddOccupation(structure);
    }

    public void RemoveOccupation(EntityNode entity)
    {
        foreach (var position in entity.EntityOccupyingPositions)
        {
            var tile = GetTile(position);
            tile.Occupants.Remove(entity);
        }
        
        if (entity is StructureNode structure)
            StructureFoundations.RemoveOccupation(structure);
    }

    public void DisableFocusedTile()
    {
        _focusedTile.Disable();
    }

    public void MoveFocusedTileTo(Vector2 position)
    {
        _focusedTile.Enable();
        _focusedTile.MoveTo(GetGlobalPositionFromMapPosition(position));
    }

    public void SetAvailableTiles(EntityNode entity, IEnumerable<Vector2> availablePositions, int size, bool hovering)
    {
        ClearAvailableTiles(hovering);

        var availableTilesSource = availablePositions.ToList();

        foreach (var tileSource in availableTilesSource.ToList()
                     .Where(tileSource => CanTileBeMovedOn(entity, tileSource, size) is false)) 
            availableTilesSource.Remove(tileSource);

        if (hovering is false)
            _availableTilesSource = availableTilesSource;

        var dilatedPositions = GetDilated(availableTilesSource, size);
        foreach (var availablePosition in dilatedPositions)
        {
            if (hovering)
            {
                _availableTilesHovering.SetCellv(availablePosition, TileMapAvailableTileIndex);
                _availableTilesHovering.UpdateBitmaskRegion(availablePosition);
                continue;
            }
            
            _availableTilesVisual.SetCellv(availablePosition, TileMapAvailableTileIndex);
            _availableTilesVisual.UpdateBitmaskRegion(availablePosition);
        }
    }

    private bool CanTileBeMovedOn(EntityNode entity, Vector2 tileSource, int size)
    {
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                var tile = GetTile(tileSource + new Vector2(x, y));
                
                if (tile is null || IsOccupied(tile) is false || entity.CanBeMovedOnAt(tile.Position) is false)
                    continue;

                if (tile.Occupants.Any(occupant => occupant.CanBeMovedOnAt(tile.Position) is false))
                    return false;
            }
        }

        return true;
    }

    public bool IsCurrentlyAvailable(TileInstance tile) => IsCurrentlyAvailable(tile.Position);
    
    public bool IsCurrentlyAvailable(Vector2 mapPosition) => _availableTilesSource.Any(x => x.Equals(mapPosition));

    public void SetTargetTiles(IEnumerable<Vector2> targets, bool isPlacementAreaTheWholeMap, bool isTargetPositive = true)
    {
        if (isPlacementAreaTheWholeMap)
        {
            _targetMapPositiveTiles.Visible = isTargetPositive;
            _targetMapNegativeTiles.Visible = isTargetPositive is false;
            
            return;
        }
        
        ClearTargetTiles();

        foreach (var target in targets)
        {
            _targetTiles.SetCellv(target, isTargetPositive 
                ? TileMapPositiveTargetTileIndex 
                : TileMapNegativeTargetTileIndex);
            
            var tileInstance = GetTile(target);
            tileInstance.IsTarget = true;
            _targetTileInstances.Add(tileInstance);
        }
    }

    public bool IsCurrentlyTarget(Vector2 mapPosition) 
        => _targetTiles.GetCellv(mapPosition) == TileMapNegativeTargetTileIndex 
           || _targetTiles.GetCellv(mapPosition) == TileMapPositiveTargetTileIndex;

    public void SetPathTiles(IEnumerable<Vector2> pathPositions, int size)
    {
        ClearPath();

        var positions = GetDilated(pathPositions, size);
        foreach (var pathPosition in positions)
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
    
    public void ClearAvailableTiles(bool hovering)
    {
        if (hovering)
        {
            _availableTilesHovering.Clear();
            return;
        }
        
        _availableTilesVisual.Clear();
        _availableTilesSource = Enumerable.Empty<Vector2>();
    }

    public void ClearTargetTiles()
    {
        _targetMapPositiveTiles.Visible = false;
        _targetMapNegativeTiles.Visible = false;
        _targetTiles.Clear();
        foreach (var tileInstance in _targetTileInstances) 
            tileInstance.IsTarget = false;
        _targetTileInstances.Clear();
    }

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

    private IEnumerable<Vector2> GetDilated(IEnumerable<Vector2> positions, int size)
    {
        var newPositions = new HashSet<Vector2>();
        foreach (var position in positions)
        {
            for (var xOffset = 0; xOffset < size; xOffset++)
            {
                for (var yOffset = 0; yOffset < size; yOffset++)
                {
                    var resultingCoordinates = position + new Vector2(xOffset, yOffset);
                    if (resultingCoordinates.IsInBoundsOf(_mapSize))
                        newPositions.Add(resultingCoordinates);
                }
            }
        }

        return newPositions;
    }
    
    private void ClearTilemaps()
    {
        ClearPath();
        ClearAvailableTiles(true);
        ClearAvailableTiles(false);
        ClearTargetTiles();
        _grass.Clear();
        _scraps.Clear();
        _marsh.Clear();
        _mountains.Clear();
    }
}
