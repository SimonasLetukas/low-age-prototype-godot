using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using MultipurposePathfinding;

public partial class ElevatableTiles : Node2D
{
    public enum TargetPurpose
    {
        AttackMelee,
        AttackRanged,
        Ability,
        Placement
    }
    
    public FocusedTile Focused { get; private set; } = null!;

    private Tiles _tiles = null!;
    private Vector2 _tilemapOffset;
    private readonly HashSet<int> _preparedElevations = [0]; 
    private Node2D _alpha = null!;

    private readonly Dictionary<(Vector2Int, int), int> _zIndexesByPositionAndElevation = new();
    private readonly Dictionary<int, AvailableTiles> _availableTilesVisual = new();
    private readonly Dictionary<int, AvailableHoveringTiles> _availableTilesHovering = new();
    private IEnumerable<Tiles.TileInstance> _availableTilesCache = new List<Tiles.TileInstance>();
    private readonly Dictionary<int, TargetTiles> _targetNormalTileMaps = new();
    private readonly Dictionary<int, TargetMeleeTiles> _targetMeleeTileMaps = new();
    private readonly Dictionary<int, TargetTiles> _targetNormalTileMapsHovering = new();
    private readonly Dictionary<int, TargetMeleeTiles> _targetMeleeTileMapsHovering = new();
    private TileMapLayer _targetMapTiles = null!;
    private TileMapLayer _targetMapTilesHovering = null!;
    private readonly HashSet<Tiles.TileInstance> _targetNormalTileInstances = [];
    private readonly HashSet<Tiles.TileInstance> _targetMeleeTileInstances = [];
    private readonly Dictionary<int, PathTiles> _pathTileMaps = new();
    
    private Guid? _availableTilesCachedEntity = Guid.Empty;
    private Vector2Int _availableTilesCachedEntityPosition = Vector2Int.Zero;
    private Guid? _availableHoveringTilesCachedEntity = Guid.Empty;
    private Vector2Int _availableHoveringTilesCachedEntityPosition = Vector2Int.Zero;

    private const int SourceId = 0;
    
    private const int TileMapAvailableTileTerrainSetIndex = 0;
    private const int TileMapAvailableTileTerrainIndex = 1;
    
    private const int TileMapPathTerrainSetIndex = 0;
    private const int TileMapPathTerrainIndex = 9;

    public override void _Ready()
    {
        base._Ready();

        _alpha = GetNode<Node2D>($"Alpha");
        _availableTilesVisual[0] = GetNode<AvailableTiles>($"Alpha/{nameof(AvailableTiles)}");
        _availableTilesHovering[0] = GetNode<AvailableHoveringTiles>($"Alpha/{nameof(AvailableHoveringTiles)}");
        GD.Print($"Count: {_availableTilesVisual.Count}");
        _targetNormalTileMaps[0] = GetNode<TargetTiles>($"Alpha/{nameof(TargetTiles)}");
        _targetMeleeTileMaps[0] = GetNode<TargetMeleeTiles>($"Alpha/{nameof(TargetMeleeTiles)}");
        _targetNormalTileMapsHovering[0] = GetNode<TargetTiles>($"Alpha/{nameof(TargetTiles)}Hovering");
        _targetMeleeTileMapsHovering[0] = GetNode<TargetMeleeTiles>($"Alpha/{nameof(TargetMeleeTiles)}Hovering");
        _targetMapTiles = GetNode<TileMapLayer>("Alpha/TargetMap");
        _targetMapTiles.Clear();
        _targetMapTilesHovering = GetNode<TileMapLayer>("Alpha/TargetMapHovering");
        _targetMapTilesHovering.Clear();
        
        _pathTileMaps[0] = GetNode<PathTiles>($"{nameof(PathTiles)}");
        
        Focused = GetNode<FocusedTile>($"{nameof(FocusedTile)}");

        EventBus.Instance.EntityZIndexUpdated += OnEntityZIndexUpdated;
        EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
        
        ClearPath();
        ClearAvailableTiles(true);
        ClearAvailableTiles(false);
        ClearTargetTiles(true);
        ClearTargetTiles(false);
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
        _targetMapTiles.SetCell(at, SourceId, TargetTiles.TileMapNormalTargetAtlasPosition);
        _targetMapTilesHovering.SetCell(at, SourceId, TargetTiles.TileMapHoveringTargetAtlasPosition);
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

            var availablePositions = pointsByElevation
                .Where(x => x.Key == tileMapsByElevationEntry.Key)
                .SelectMany(x => x.Value, (_, point) => 
                    (point.Position.ToGodotVector2I(), GetZIndexAt(point.Position, tileMapsByElevationEntry.Key)))
                .ToList();
            
            if (hovering)
            {
                availableHoveringTiles.SetTiles(
                    availablePositions,
                    TileMapAvailableTileTerrainSetIndex,
                    TileMapAvailableTileTerrainIndex);
                continue;
            }
            
            availableTiles.SetTiles(
                availablePositions, 
                TileMapAvailableTileTerrainSetIndex, 
                TileMapAvailableTileTerrainIndex);
        }

        var availableTileInstances = availablePointsSource
            .Select(x => _tiles.GetTile(x.Position, x.IsHighGround))
            .WhereNotNull();
        CacheAvailableTiles(entity, hovering, availableTileInstances);
    }

    public (int, Tiles.TileInstance?) GetElevationAndAvailableTileAtMousePosition()
    {
        var highestElevation = -1;
        Tiles.TileInstance? foundTile = null;
        
        if (_availableTilesVisual.Values.Any(x => x.Visible is false))
            return (highestElevation, foundTile);
        
        var mousePosition = GetGlobalMousePosition();
        foreach (var (elevation, availableTiles) in _availableTilesVisual)
        {
            if (elevation <= highestElevation)
                continue;

            var position = GetMapPositionFromGlobalPosition(mousePosition, availableTiles);
            var tileExists = _availableTilesCache.Any(x => x.Position.Equals(position) 
                                                           && x.YSpriteOffset.Equals(elevation));
            if (tileExists is false) 
                continue;
            
            foundTile = _availableTilesCache.First(x => x.Position.Equals(position) 
                                                        && x.YSpriteOffset.Equals(elevation));
            highestElevation = elevation;
        }

        return (highestElevation, foundTile);
    }

    public bool IsCurrentlyAvailable(Tiles.TileInstance? tile) => tile != null && IsCurrentlyAvailable(tile.Point);
    
    private bool IsCurrentlyAvailable(Point point) => _availableTilesCache.Any(x => x.Point.Id.Equals(point.Id));
    
    public void SetTargetTiles(IEnumerable<Tiles.TileInstance> targets, bool wholeMapIsTargeted, 
        bool hovering, TargetPurpose purpose)
    {
        if (wholeMapIsTargeted)
        {
            if (hovering)
            {
                _targetMapTilesHovering.Visible = true;
                return;
            }
            
            _targetMapTiles.Visible = true;
            return;
        }
        
        ClearTargetTiles(hovering, purpose);

        var targetTileInstances = GetUpdatedTargetTileInstances(targets, hovering, purpose);
        var pointsByElevation = SplitIntoElevations(targetTileInstances);
        var tileMapsByElevation = GetAndPrepareAllElevationsOfTargetTileMap(
            pointsByElevation.Keys, hovering, purpose is TargetPurpose.AttackMelee);
        
        foreach (var entry in tileMapsByElevation)
        {
            var targetPositions = pointsByElevation
                .Where(x => x.Key == entry.Key)
                .SelectMany(x => x.Value, (_, point) => 
                    (point.Position.ToGodotVector2I(), GetZIndexAt(point.Position, entry.Key)))
                .ToList();
            
            entry.Value.SetTiles(targetPositions, hovering, purpose is TargetPurpose.AttackMelee);
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
            var pathPositions = pointsByElevation
                .Where(x => x.Key == entry.Key)
                .SelectMany(x => x.Value, (_, point) => 
                    (point.Position.ToGodotVector2I(), GetZIndexAt(point.Position, entry.Key)));
            
            entry.Value.SetTiles(pathPositions, TileMapPathTerrainSetIndex, TileMapPathTerrainIndex);
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
    
    public void ClearTargetTiles(bool hovering)
    {
        ClearTargetTiles(hovering, TargetPurpose.AttackMelee);
        ClearTargetTiles(hovering, TargetPurpose.AttackRanged); // implicitly covers TargetPurpose.Ability
    }

    public void ClearTargetTiles(bool hovering, TargetPurpose purpose)
    {
        if (hovering)
        {
            _targetMapTilesHovering.Visible = false;

            if (purpose is TargetPurpose.AttackMelee)
            {
                foreach (var targetTiles in _targetMeleeTileMapsHovering.Values)
                    targetTiles.Clear();
            }
            else
            {
                foreach (var targetTiles in _targetNormalTileMapsHovering.Values) 
                    targetTiles.Clear();
            }
            
            return;
        }
        
        _targetMapTiles.Visible = false;

        if (purpose is TargetPurpose.AttackMelee)
        {
            foreach (var targetTiles in _targetMeleeTileMaps.Values)
                targetTiles.Clear();
            
            foreach (var tileInstance in _targetMeleeTileInstances) 
                tileInstance.TargetType = TargetType.None;
            
            _targetMeleeTileInstances.Clear();

        }
        else
        {
            foreach (var targetTiles in _targetNormalTileMaps.Values) 
                targetTiles.Clear();
            
            foreach (var tileInstance in _targetNormalTileInstances) 
                tileInstance.TargetType = TargetType.None;
            
            _targetNormalTileInstances.Clear();
        }
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
    
    private Vector2Int GetMapPositionFromGlobalPosition(Vector2 globalPosition, ElevatableTileMap tileMap) 
        => (tileMap.LocalToMap(globalPosition - tileMap.Position + tileMap.Offset) - _tilemapOffset)
            .ToVector2();
    
    private HashSet<Tiles.TileInstance> GetUpdatedTargetTileInstances(IEnumerable<Tiles.TileInstance> targets, 
        bool hovering, TargetPurpose purpose)
    {
        var targetTileInstances = purpose is TargetPurpose.AttackMelee 
            ? _targetMeleeTileInstances 
            : _targetNormalTileInstances;

        if (hovering) 
            return targetTileInstances;
        
        foreach (var target in targets)
        {
            var targetType = purpose is TargetPurpose.AttackMelee ? TargetType.Melee : TargetType.Normal;

            target.TargetType = targetType;
            targetTileInstances.Add(target);

            if (purpose is TargetPurpose.Placement)
                continue;
            
            foreach (var occupant in target.GetOccupants())
            {
                foreach (var occupiedTile in occupant.EntityOccupyingTiles)
                {
                    occupiedTile.TargetType = targetType;
                    targetTileInstances.Add(occupiedTile);
                }
            }
        }
        
        return targetTileInstances;
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
    
    private Dictionary<int, TargetTiles> GetAndPrepareAllElevationsOfTargetTileMap(IEnumerable<int> elevations, 
        bool hovering, bool isMelee)
    {
        var result = new Dictionary<int, TargetTiles>();
        foreach (var elevation in elevations)
        {
            PrepareElevation(elevation);
            result[elevation] = hovering switch
            {
                false when isMelee => _targetMeleeTileMaps[elevation],
                false when isMelee is false => _targetNormalTileMaps[elevation],
                true when isMelee => _targetMeleeTileMapsHovering[elevation],
                true when isMelee is false => _targetNormalTileMapsHovering[elevation],
                _ => _targetNormalTileMaps[elevation],
            };
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
        var targetTilesHovering = TargetTiles.InstantiateAsChild(_alpha);
        var targetMeleeTiles = TargetMeleeTiles.InstantiateAsChild(_alpha);
        var targetMeleeTilesHovering = TargetMeleeTiles.InstantiateAsChild(_alpha);
        
        targetTiles.SetElevation(elevation);
        targetTilesHovering.SetElevation(elevation);
        targetMeleeTiles.SetElevation(elevation);
        targetMeleeTilesHovering.SetElevation(elevation);
            
        _targetNormalTileMaps[elevation] = targetTiles;
        _targetNormalTileMapsHovering[elevation] = targetTilesHovering;
        _targetMeleeTileMaps[elevation] = targetMeleeTiles;
        _targetMeleeTileMapsHovering[elevation] = targetMeleeTilesHovering;
        
        // Path tiles
        var pathTiles = PathTiles.InstantiateAsChild(this);
        pathTiles.SetElevation(elevation);
        _pathTileMaps[elevation] = pathTiles;
    }
    
    private bool CanTileBeMovedOn(EntityNode entity, Point point, int size)
    {
        foreach (var offset in IterateVector2Int.Positions(size, size))
        {
            var tile = _tiles.GetTile(point.Position + offset, point.IsHighGround);
                
            if (tile is null || entity.CanBeMovedOnAt(tile.Point, entity.Player.Team) is false)
                continue;

            if (tile.GetOccupants().Any(occupant => 
                    occupant.CanBeMovedOnAt(tile.Point, entity.Player.Team) is false))
                return false;
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
        foreach (var tileMap in hovering 
                     ? _availableTilesHovering.Values.Cast<TileMapLayer>() 
                     : _availableTilesVisual.Values)
            tileMap.Visible = true;
    }

    private void ClearAvailableTilesCache(bool hovering)
    {
        if (hovering)
        {
            _availableHoveringTilesCachedEntity = null;
            return;
        }

        _availableTilesCache = [];
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

    private int GetZIndexAt(Vector2Int position, int elevation) 
        => _zIndexesByPositionAndElevation.ContainsKey((position, elevation))
            ? _zIndexesByPositionAndElevation[(position, elevation)] 
            : 0;

    private void OnEntityZIndexUpdated(EntityNode entity, int to)
    {
        var positionsWithElevation = entity.ProvidedHighGroundHeightByOccupyingPosition;
        foreach (var (position, elevation) in positionsWithElevation)
        {
            if (elevation is 0)
                continue;
            
            _zIndexesByPositionAndElevation[(position, elevation)] = to;
            
            if (_preparedElevations.Contains(elevation) is false)
                continue;

            var godotPosition = position.ToGodotVector2();
            _availableTilesVisual[elevation].SetTileZIndex(godotPosition, to);
            _availableTilesHovering[elevation].SetTileZIndex(godotPosition, to);
            _targetNormalTileMaps[elevation].SetTileZIndex(godotPosition, to);
            _pathTileMaps[elevation].SetTileZIndex(godotPosition, to);
        }
    }
    
    private void OnPathfindingUpdating(IPathfindingUpdatable data, bool isAdded) => ClearCache();
}
