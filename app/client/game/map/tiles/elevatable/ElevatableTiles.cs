using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using MultipurposePathfinding;

public partial class ElevatableTiles : Node2D
{
    public FocusedTile Focused { get; private set; }
    
    private Tiles _tiles;
    private Vector2 _tilemapOffset;
    private readonly HashSet<int> _preparedElevations = new HashSet<int> { 0 }; 
    private Node2D _alpha;

    private readonly Dictionary<(Vector2<int>, int), int> _zIndexesByPositionAndElevation = new Dictionary<(Vector2<int>, int), int>();
    private readonly Dictionary<int, AvailableTiles> _availableTilesVisual = new Dictionary<int, AvailableTiles>();
    private readonly Dictionary<int, AvailableHoveringTiles> _availableTilesHovering = new Dictionary<int, AvailableHoveringTiles>();
    private IEnumerable<Tiles.TileInstance> _availableTilesCache = new List<Tiles.TileInstance>();
    private readonly Dictionary<int, TargetTiles> _targetTileMaps = new Dictionary<int, TargetTiles>();
    private TileMap _targetMapPositiveTiles;
    private TileMap _targetMapNegativeTiles;
    private readonly ICollection<Tiles.TileInstance> _targetTileInstances = new List<Tiles.TileInstance>();
    private readonly Dictionary<int, PathTiles> _pathTileMaps = new Dictionary<int, PathTiles>();
    
    private Guid? _availableTilesCachedEntity = Guid.Empty;
    private Vector2<int> _availableTilesCachedEntityPosition = Vector2Int.Zero;
    private Guid? _availableHoveringTilesCachedEntity = Guid.Empty;
    private Vector2<int> _availableHoveringTilesCachedEntityPosition = Vector2Int.Zero;
    
    private const int TileMapAvailableTileIndex = 8;
    private const int TileMapPathTileIndex = 9;

    public override void _Ready()
    {
        base._Ready();

        _alpha = GetNode<Node2D>($"Alpha");
        _availableTilesVisual[0] = GetNode<AvailableTiles>($"Alpha/{nameof(AvailableTiles)}");
        _availableTilesHovering[0] = GetNode<AvailableHoveringTiles>($"Alpha/{nameof(AvailableHoveringTiles)}");
        GD.Print($"Count: {_availableTilesVisual.Count}");
        _targetTileMaps[0] = GetNode<TargetTiles>($"Alpha/{nameof(TargetTiles)}");
        _targetMapPositiveTiles = GetNode<TileMap>("Alpha/TargetMapPositive");
        _targetMapPositiveTiles.Clear();
        _targetMapNegativeTiles = GetNode<TileMap>("Alpha/TargetMapNegative");
        _targetMapNegativeTiles.Clear();
        
        _pathTileMaps[0] = GetNode<PathTiles>($"{nameof(PathTiles)}");
        
        Focused = GetNode<FocusedTile>($"{nameof(FocusedTile)}");

        EventBus.Instance.EntityZIndexUpdated += OnEntityZIndexUpdated;
        EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
        
        ClearPath();
        ClearAvailableTiles(true);
        ClearAvailableTiles(false);
        ClearTargetTiles();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();

        EventBus.Instance.EntityZIndexUpdated -= OnEntityZIndexUpdated;
        EventBus.Instance.PathfindingUpdating -= OnPathfindingUpdating;
    }

    public void Initialize(Vector2 mapSize, Tiles parent)
    {
        _tiles = parent;
        Focused.Initialize(parent);
        _tilemapOffset = new Vector2(mapSize.X / 2, (mapSize.Y / 2) * -1);
    }

    public void SetMapWideTargetTiles(Vector2I at)
    {
        _targetMapPositiveTiles.SetCell(0, at, TargetTiles.TileMapPositiveTargetTileIndex);
        _targetMapNegativeTiles.SetCell(0, at, TargetTiles.TileMapNegativeTargetTileIndex);
    }

    public void SetAvailableTiles(EntityNode entity, IEnumerable<Point> availablePoints, int size, bool hovering)
    {
        if (AvailableTilesCachedFor(entity, hovering))
        {
            ShowCachedAvailableTiles(hovering);
            return;
        }

        ClearAvailableTilesCache(hovering);
        ClearAvailableTiles(hovering);

        var availablePointsSource = availablePoints.ToList();
        
        foreach (var tileSource in availablePointsSource.ToList()
                     .Where(tileSource => CanTileBeMovedOn(entity, tileSource, size) is false))
            availablePointsSource.Remove(tileSource);
        
        var dilatedPoints = _tiles.GetDilated(availablePointsSource, size).ToList();
        var pointsByElevation = SplitIntoElevations(dilatedPoints);
        var tileMapsByElevation = GetAndPrepareAllElevationsOfAvailableTileMaps(pointsByElevation.Keys);
        
        foreach (var tileMapsByElevationEntry in tileMapsByElevation)
        {
            var (availableTiles, availableHoveringTiles) = tileMapsByElevationEntry.Value;

            foreach (var availablePosition in pointsByElevation
                         .Where(x => x.Key == tileMapsByElevationEntry.Key)
                         .SelectMany(x => x.Value, (_, point) => point.Position))
            {
                var godotAvailablePosition = availablePosition.ToGodotVector2I<int>();
                if (hovering)
                {
                    availableHoveringTiles.SetTile(godotAvailablePosition, TileMapAvailableTileIndex, 
                        GetZIndexAt(availablePosition, tileMapsByElevationEntry.Key));
                    //availableHoveringTiles.UpdateBitmaskRegion(godotAvailablePosition);
                    continue;
                }
        
                availableTiles.SetTile(godotAvailablePosition, TileMapAvailableTileIndex, 
                    GetZIndexAt(availablePosition, tileMapsByElevationEntry.Key));
                //availableTiles.UpdateBitmaskRegion(godotAvailablePosition);
            }
        }

        var availableTileInstances = availablePointsSource
            .Select(x => _tiles.GetTile(x.Position, x.IsHighGround));
        CacheAvailableTiles(entity, hovering, availableTileInstances);
    }

    public Tiles.TileInstance GetAvailableTileAtMousePosition()
    {
        if (_availableTilesVisual.Values.Any(x => x.Visible is false))
            return null;
        
        var mousePosition = GetGlobalMousePosition();
        var highestElevation = -1;
        Tiles.TileInstance result = null;
        foreach (var availableTiles in _availableTilesVisual)
        {
            var position = availableTiles.Value.LocalToMap(mousePosition 
                                                           - availableTiles.Value.Position) - _tilemapOffset;
            var tileExists = _availableTilesCache.Any(x => x.Position.ToGodotVector2().Equals(position) 
                                                           && x.YSpriteOffset.Equals(availableTiles.Key));
            if (tileExists is false || availableTiles.Key <= highestElevation) 
                continue;
            
            result = _availableTilesCache.First(x => x.Position.ToGodotVector2().Equals(position)
                                                     && x.YSpriteOffset.Equals(availableTiles.Key));
            highestElevation = availableTiles.Key;
        }

        return result;
    }
    
    public bool IsCurrentlyAvailable(Tiles.TileInstance tile) => IsCurrentlyAvailable(tile.Point);
    
    public bool IsCurrentlyAvailable(Point point) => _availableTilesCache.Any(x => x.Point.Id.Equals(point.Id));

    public void SetTargetTiles(IEnumerable<Vector2<int>> targets, bool isPlacementAreaTheWholeMap, bool isTargetPositive = true)
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
            var tileInstance = _tiles.GetHighestTile(target);
            tileInstance.IsTarget = true;
            _targetTileInstances.Add(tileInstance);
        }

        var pointsByElevation = SplitIntoElevations(_targetTileInstances);
        var tileMapsByElevation = GetAndPrepareAllElevationsOfTargetTileMap(pointsByElevation.Keys);
        
        foreach (var entry in tileMapsByElevation)
        {
            foreach (var targetPosition in pointsByElevation
                         .Where(x => x.Key == entry.Key)
                         .SelectMany(x => x.Value, (_, point) => point.Position))
            {
                entry.Value.SetTile(targetPosition.ToGodotVector2I<int>(), isTargetPositive, 
                    GetZIndexAt(targetPosition, entry.Key));
            }
        }
    }

    public void SetPathTiles(IEnumerable<Point> pathPoints, int size)
    {
        ClearPath();
        
        var dilatedPoints = _tiles.GetDilated(pathPoints, size).ToList();
        var pointsByElevation = SplitIntoElevations(dilatedPoints);
        var tileMapsByElevation = GetAndPrepareAllElevationsOfPathTileMap(pointsByElevation.Keys);
        
        foreach (var entry in tileMapsByElevation)
        {
            foreach (var pathPosition in pointsByElevation
                         .Where(x => x.Key == entry.Key)
                         .SelectMany(x => x.Value, (_, point) => point.Position))
            {
                entry.Value.SetTile(pathPosition.ToGodotVector2I<int>(), TileMapPathTileIndex, 
                    GetZIndexAt(pathPosition, entry.Key));
            }
        }
    }
    
    public void ClearAvailableTiles(bool hovering)
    {
        if (hovering)
        {
            foreach (var availableTilesHovering in _availableTilesHovering.Values)
            {
                if (_availableHoveringTilesCachedEntity != null)
                {
                    availableTilesHovering.Visible = false;
                    continue;
                }
                
                availableTilesHovering.Clear();
                availableTilesHovering.Visible = true;
            }
            return;
        }
        
        foreach (var availableTilesVisual in _availableTilesVisual.Values)
        {
            if (_availableTilesCachedEntity != null)
            {
                availableTilesVisual.Visible = false;
                continue;
            }
            
            availableTilesVisual.Clear();
            availableTilesVisual.Visible = true;
        }
    }
    
    public void ClearTargetTiles()
    {
        _targetMapPositiveTiles.Visible = false;
        _targetMapNegativeTiles.Visible = false;
        foreach (var targetTiles in _targetTileMaps.Values)
        {
            targetTiles.Clear();
        }
        foreach (var tileInstance in _targetTileInstances) 
            tileInstance.IsTarget = false;
        _targetTileInstances.Clear();
    }
    
    public void ClearPath()
    {
        foreach (var tileMap in _pathTileMaps.Values)
        {
            tileMap.Clear();
        }
    }
    
    public void ClearCache()
    {
        ClearAvailableTilesCache(true);
        ClearAvailableTilesCache(false);
    }
    
    private static Dictionary<int, ICollection<Tiles.TileInstance>> SplitIntoElevations(
        IEnumerable<Tiles.TileInstance> tiles)
    {
        var result = new Dictionary<int, ICollection<Tiles.TileInstance>>();
        foreach (var tile in tiles)
        {
            var elevation = tile.YSpriteOffset;
            if (result.ContainsKey(elevation) is false)
                result[elevation] = new List<Tiles.TileInstance>();

            result[elevation].Add(tile);
        }

        return result;
    }
    
    private Dictionary<int, (AvailableTiles, AvailableHoveringTiles)> GetAndPrepareAllElevationsOfAvailableTileMaps(
        IEnumerable<int> elevations)
    {
        var result = new Dictionary<int, (AvailableTiles, AvailableHoveringTiles)>();
        foreach (var elevation in elevations)
        {
            PrepareElevation(elevation);
            result[elevation] = (_availableTilesVisual[elevation], _availableTilesHovering[elevation]);
        }

        return result;
    }
    
    private Dictionary<int, TargetTiles> GetAndPrepareAllElevationsOfTargetTileMap(IEnumerable<int> elevations)
    {
        var result = new Dictionary<int, TargetTiles>();
        foreach (var elevation in elevations)
        {
            PrepareElevation(elevation);
            result[elevation] = _targetTileMaps[elevation];
        }

        return result;
    }
    
    private Dictionary<int, PathTiles> GetAndPrepareAllElevationsOfPathTileMap(IEnumerable<int> elevations)
    {
        var result = new Dictionary<int, PathTiles>();
        foreach (var elevation in elevations)
        {
            PrepareElevation(elevation);
            result[elevation] = _pathTileMaps[elevation];
        }

        return result;
    }
    
    private void PrepareElevation(int elevation)
    {
        if (_preparedElevations.Add(elevation) is false) 
            return;
        
        // Available tiles
        var availableTiles = AvailableTiles.InstantiateAsChild(_alpha);
        var availableHoveringTiles = AvailableHoveringTiles.InstantiateAsChild(_alpha);
            
        availableTiles.SetElevation(elevation);
        availableHoveringTiles.SetElevation(elevation);
        
        _availableTilesVisual[elevation] = availableTiles;
        _availableTilesHovering[elevation] = availableHoveringTiles;
        
        // Target tiles
        var targetTiles = TargetTiles.InstantiateAsChild(_alpha);
        targetTiles.SetElevation(elevation);
        _targetTileMaps[elevation] = targetTiles;
        
        // Path tiles
        var pathTiles = PathTiles.InstantiateAsChild(this);
        pathTiles.SetElevation(elevation);
        _pathTileMaps[elevation] = pathTiles;
    }
    
    private bool CanTileBeMovedOn(EntityNode entity, Point point, int size)
    {
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                var tile = _tiles.GetTile(point.Position + new Vector2<int>(x, y), point.IsHighGround);
                
                if (tile is null || entity.CanBeMovedOnAt(tile.Point, entity.Team) is false)
                    continue;

                if (tile.Occupants.Any(occupant => occupant.CanBeMovedOnAt(tile.Point, entity.Team) is false))
                    return false;
            }
        }

        return true;
    }
    
    private bool AvailableTilesCachedFor(EntityNode entity, bool hovering) => hovering
        ? _availableHoveringTilesCachedEntity != null 
          && _availableHoveringTilesCachedEntity.Equals(entity.InstanceId) 
          && _availableHoveringTilesCachedEntityPosition.Equals(entity.EntityPrimaryPosition)
        : _availableTilesCachedEntity != null 
          && _availableTilesCachedEntity.Equals(entity.InstanceId) 
          && _availableTilesCachedEntityPosition.Equals(entity.EntityPrimaryPosition);

    private void ShowCachedAvailableTiles(bool hovering)
    {
        foreach (var tileMap in hovering ? _availableTilesHovering.Values.Cast<TileMap>() : _availableTilesVisual.Values)
            tileMap.Visible = true;
    }

    private void ClearAvailableTilesCache(bool hovering)
    {
        if (hovering)
        {
            _availableHoveringTilesCachedEntity = null;
            return;
        }

        _availableTilesCache = Enumerable.Empty<Tiles.TileInstance>();
        _availableTilesCachedEntity = null;
    }
    
    private void CacheAvailableTiles(EntityNode entity, bool hovering, IEnumerable<Tiles.TileInstance> availableTiles)
    {
        if (hovering)
        {
            _availableHoveringTilesCachedEntity = entity.InstanceId;
            _availableHoveringTilesCachedEntityPosition = entity.EntityPrimaryPosition;
            return;
        }

        _availableTilesCachedEntity = entity.InstanceId;
        _availableTilesCachedEntityPosition = entity.EntityPrimaryPosition;
        _availableTilesCache = availableTiles;
    }

    private int GetZIndexAt(Vector2<int> position, int elevation) 
        => _zIndexesByPositionAndElevation.ContainsKey((position, elevation))
            ? _zIndexesByPositionAndElevation[(position, elevation)] 
            : 0;

    private void OnEntityZIndexUpdated(EntityNode entity, int to)
    {
        foreach (var pair in entity.ProvidedHighGroundHeightByOccupyingPosition)
        {
            var position = pair.Key;
            var elevation = pair.Value;

            if (elevation is 0)
                return;
            
            _zIndexesByPositionAndElevation[(position, elevation)] = to;
            
            if (_preparedElevations.Contains(elevation) is false)
                return;

            var godotPosition = position.ToGodotVector2();
            _availableTilesVisual[elevation].SetTileZIndex(godotPosition, to);
            _availableTilesHovering[elevation].SetTileZIndex(godotPosition, to);
            _targetTileMaps[elevation].SetTileZIndex(godotPosition, to);
            _pathTileMaps[elevation].SetTileZIndex(godotPosition, to);
        }
    }
    
    private void OnPathfindingUpdating(IPathfindingUpdatable data, bool isAdded) => ClearCache();
}
