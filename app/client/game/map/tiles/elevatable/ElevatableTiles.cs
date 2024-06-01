using System.Collections.Generic;
using System.Linq;
using Godot;

public class ElevatableTiles : Node2D
{
    private Tiles _tiles;
    private readonly HashSet<int> _preparedElevations = new HashSet<int> { 0 }; 
    private FocusedTile _focusedTile; // TODO add elevation
    private Node2D _alpha;
    private readonly Dictionary<int, AvailableTiles> _availableTilesVisual = new Dictionary<int, AvailableTiles>();
    private readonly Dictionary<int, AvailableHoveringTiles> _availableTilesHovering = new Dictionary<int, AvailableHoveringTiles>();
    private IEnumerable<Point> _availableTilesCache = new List<Point>();
    private TileMap _targetTiles;
    private TileMap _targetMapPositiveTiles;
    private TileMap _targetMapNegativeTiles;
    private readonly ICollection<Tiles.TileInstance> _targetTileInstances = new List<Tiles.TileInstance>();
    private readonly Dictionary<int, PathTiles> _pathTileMaps = new Dictionary<int, PathTiles>();
    
    private const int TileMapAvailableTileIndex = 8;
    private const int TileMapPathTileIndex = 9;
    private const int TileMapNegativeTargetTileIndex = 11;
    private const int TileMapPositiveTargetTileIndex = 12;

    public override void _Ready()
    {
        base._Ready();

        _alpha = GetNode<Node2D>($"Alpha");
        _availableTilesVisual[0] = GetNode<AvailableTiles>($"Alpha/{nameof(AvailableTiles)}");
        _availableTilesHovering[0] = GetNode<AvailableHoveringTiles>($"Alpha/{nameof(AvailableHoveringTiles)}");
        GD.Print($"Count: {_availableTilesVisual.Count}");
        _targetTiles = GetNode<TileMap>("Alpha/Target");
        _targetMapPositiveTiles = GetNode<TileMap>("Alpha/TargetMapPositive");
        _targetMapPositiveTiles.Clear();
        _targetMapNegativeTiles = GetNode<TileMap>("Alpha/TargetMapNegative");
        _targetMapNegativeTiles.Clear();
        
        _pathTileMaps[0] = GetNode<PathTiles>($"{nameof(PathTiles)}");
        
        _focusedTile = GetNode<FocusedTile>("FocusedTile");
        _focusedTile.Disable();
        
        ClearPath();
        ClearAvailableTiles(true);
        ClearAvailableTiles(false);
        ClearTargetTiles();
    }

    public void Initialize(Tiles parent) => _tiles = parent;

    public void SetMapWideTargetTiles(Vector2 at)
    {
        _targetMapPositiveTiles.SetCellv(at, TileMapPositiveTargetTileIndex);
        _targetMapNegativeTiles.SetCellv(at, TileMapNegativeTargetTileIndex);
    }
    
    public void DisableFocusedTile()
    {
        _focusedTile.Disable();
    }

    public void MoveFocusedTileTo(Vector2 position)
    {
        _focusedTile.Enable();
        _focusedTile.MoveTo(_tiles.GetGlobalPositionFromMapPosition(position));
    }

    public void SetAvailableTiles(EntityNode entity, IEnumerable<Point> availablePoints, int size, bool hovering)
    {
        ClearAvailableTiles(hovering);

        var availablePointsSource = availablePoints.ToList();
        
        foreach (var tileSource in availablePointsSource.ToList()
                     .Where(tileSource => CanTileBeMovedOn(entity, tileSource, size) is false)) // TODO test this method out...
            availablePointsSource.Remove(tileSource);

        if (hovering is false)
            _availableTilesCache = availablePointsSource;

        var dilatedPoints = _tiles.GetDilated(availablePointsSource, size).Select(x => x.Point).ToList();
        var pointsByElevation = SplitIntoElevations(dilatedPoints);
        var tileMapsByElevation = GetAndPrepareAllElevationsOfAvailableTileMaps(pointsByElevation.Keys);

        foreach (var tileMapsByElevationEntry in tileMapsByElevation)
        {
            var (availableTiles, availableHoveringTiles) = tileMapsByElevationEntry.Value;

            foreach (var availablePosition in pointsByElevation
                         .Where(x => x.Key == tileMapsByElevationEntry.Key)
                         .SelectMany(x => x.Value, (_, point) => point.Position))
            {
                if (hovering)
                {
                    availableHoveringTiles.SetCellv(availablePosition, TileMapAvailableTileIndex);
                    availableHoveringTiles.UpdateBitmaskRegion(availablePosition);
                    continue;
                }
        
                availableTiles.SetCellv(availablePosition, TileMapAvailableTileIndex);
                availableTiles.UpdateBitmaskRegion(availablePosition);
            }
        }
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

    private bool CanTileBeMovedOn(EntityNode entity, Point point, int size)
    {
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                var tile = _tiles.GetTile(point.Position + new Vector2(x, y), point.IsHighGround);
                
                if (tile is null || entity.CanBeMovedOnAt(tile.Point) is false)
                    continue;

                if (tile.Occupants.Any(occupant => occupant.CanBeMovedOnAt(tile.Point) is false))
                    return false;
            }
        }

        return true;
    }

    public bool IsCurrentlyAvailable(Tiles.TileInstance tile) => IsCurrentlyAvailable(tile.Point);
    
    public bool IsCurrentlyAvailable(Point point) => _availableTilesCache.Any(x => x.Equals(point));

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
            
            var tileInstance = _tiles.GetTile(target);
            tileInstance.IsTarget = true;
            _targetTileInstances.Add(tileInstance);
        }
    }

    public bool IsCurrentlyTarget(Vector2 mapPosition) 
        => _targetTiles.GetCellv(mapPosition) == TileMapNegativeTargetTileIndex 
           || _targetTiles.GetCellv(mapPosition) == TileMapPositiveTargetTileIndex;

    public void SetPathTiles(IEnumerable<Point> pathPoints, int size)
    {
        ClearPath();
        
        var dilatedPoints = _tiles.GetDilated(pathPoints, size).Select(x => x.Point).ToList();
        var pointsByElevation = SplitIntoElevations(dilatedPoints);
        var tileMapsByElevation = GetAndPrepareAllElevationsOfPathTileMap(pointsByElevation.Keys);
        
        foreach (var entry in tileMapsByElevation)
        {
            foreach (var pathPosition in pointsByElevation
                         .Where(x => x.Key == entry.Key)
                         .SelectMany(x => x.Value, (_, point) => point.Position))
            {
                entry.Value.SetCellv(pathPosition, TileMapPathTileIndex);
            }
        }
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
    
    public void ClearPath()
    {
        foreach (var tileMap in _pathTileMaps.Values)
        {
            tileMap.Clear();
        }
    }

    public void ClearAvailableTiles(bool hovering)
    {
        if (hovering)
        {
            foreach (var availableTilesHovering in _availableTilesHovering.Values)
            {
                availableTilesHovering.Clear();
            }
            return;
        }
        
        foreach (var availableTilesVisual in _availableTilesVisual.Values)
        {
            availableTilesVisual.Clear();
        }
        _availableTilesCache = Enumerable.Empty<Point>();
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
    
    private void PrepareElevation(int elevation)
    {
        if (_preparedElevations.Add(elevation) is false) 
            return;
        
        // Available tiles
        var availableTiles = AvailableTiles.InstantiateAsChild(_alpha);
        var availableHoveringTiles = AvailableHoveringTiles.InstantiateAsChild(_alpha);
            
        availableTiles.Position = Vector2.Up * elevation;
        availableHoveringTiles.Position = Vector2.Up * elevation;

        // TODO use: availableTiles.TileSet.TileSetZIndex();
        availableTiles.ZIndex = 1000; // TODO be one higher than structure below & change upon each render update
        availableHoveringTiles.ZIndex = 1000; // TODO be one higher than structure below & change upon each render update
            
        _availableTilesVisual[elevation] = availableTiles;
        _availableTilesHovering[elevation] = availableHoveringTiles;
        
        // Path tiles
        var pathTiles = PathTiles.InstantiateAsChild(this);
        pathTiles.Position = Vector2.Up * elevation;
        // TODO use: pathTiles.TileSet.TileSetZIndex();
        pathTiles.ZIndex = 1000; // TODO be one higher than structure below & change upon each render update
            
        _pathTileMaps[elevation] = pathTiles;
    }

    private static Dictionary<int, ICollection<Point>> SplitIntoElevations(IEnumerable<Point> points)
    {
        var result = new Dictionary<int, ICollection<Point>>();
        foreach (var point in points)
        {
            var elevation = point.YSpriteOffset;
            if (result.ContainsKey(elevation) is false)
                result[elevation] = new List<Point>();

            result[elevation].Add(point);
        }

        return result;
    }
}
