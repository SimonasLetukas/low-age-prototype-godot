using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Tiles;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using multipurpose_pathfinding;

/// <summary>
/// Responsible for: <para />
/// - Keeping track of all tiles data <para />
/// - Handling all tile-map related visuals <para />
/// - Conversions between global and map positions
/// </summary>
public partial class Tiles : Node2D
{
    /// <summary>
    /// Wrapper of <see cref="Point"/>.
    /// </summary>
    public class TileInstance
    {
        public Vector2<int> Position { get; set; }
        public TileId Blueprint { get; set; }
        public Terrain Terrain { get; set; }
        public bool IsTarget { get; set; }
        public IList<EntityNode> Occupants { get; set; }
        public Point Point { get; set; }
        public int YSpriteOffset { get; set; }
    }
    
    public event Action FinishedInitialInitializing = delegate { };
    public event Action FinishedPointInitialization = delegate { };
    
    public StructureFoundations StructureFoundations { get; private set; }
    public ElevatableTiles Elevatable { get; private set; }
    
    private readonly Dictionary<(Vector2<int>, bool), TileInstance> _tiles = new Dictionary<(Vector2<int>, bool), TileInstance>();
    private IList<Tile> _tilesBlueprint;
    private Vector2<int> _mapSize;
    private Vector2 _tilemapOffset;
    private Vector2 _tileOffset;
    private int _mountainsFillOffset;
    private TileMap _grass;
    private TileMap _scraps;
    private TileMap _marsh;
    private TileMap _mountains;
    
    private const int TileMapGrassIndex = 6;
    private const int TileMapCelestiumIndex = 7;
    private const int TileMapScrapsIndex = 5;
    private const int TileMapMarshIndex = 3;
    private const int TileMapMountainsIndex = 4;
    
    private readonly Dictionary<Terrain, int> _tileMapIndexesByTerrain = new Dictionary<Terrain, int>
    {
        { Terrain.Grass,     TileMapGrassIndex },
        { Terrain.Mountains, TileMapMountainsIndex },
        { Terrain.Marsh,     TileMapMarshIndex },
        { Terrain.Scraps,    TileMapScrapsIndex },
        { Terrain.Celestium, TileMapCelestiumIndex }
    };

    private bool _dataInitialized = false;
    private IList<(Vector2<int>, TileId)> _tilesForDataInitialization;
    private bool _visualsInitialized = false;
    private IList<(Vector2<int>, TileId)> _tilesForVisualsInitialization;
    private bool _initialInitializationDone = false;
    private IList<Point> _pointsForInitialization;
    private bool _pointsStartedInitializing = false;
    private bool _pointsInitialized = false;
    private bool _initialized = true;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        
        _grass = GetNode<TileMap>("Grass");
        _scraps = GetNode<TileMap>("Scraps");
        _marsh = GetNode<TileMap>("Marsh");
        _mountains = GetNode<TileMap>("Stone");

        StructureFoundations = GetNode<StructureFoundations>($"{nameof(StructureFoundations)}");
        Elevatable = GetNode<ElevatableTiles>($"{nameof(ElevatableTiles)}");

        EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
        EventBus.Instance.HighGroundPointCreated += OnHighGroundPointCreated;
        EventBus.Instance.HighGroundPointRemoved += OnHighGroundPointRemoved;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        EventBus.Instance.PathfindingUpdating -= OnPathfindingUpdating;
        EventBus.Instance.HighGroundPointCreated -= OnHighGroundPointCreated;
        EventBus.Instance.HighGroundPointRemoved -= OnHighGroundPointRemoved;
    }

    public void Initialize(Vector2 mapSize, ICollection<(Vector2<int>, TileId)> tiles)
    {
        _tilesBlueprint = Data.Instance.Blueprint.Tiles;
        _mapSize = new Vector2<int>((int)mapSize.x, (int)mapSize.y);
        _tilemapOffset = new Vector2(mapSize.X / 2, (mapSize.Y / 2) * -1);
        _mountainsFillOffset = (int)Mathf.Max(mapSize.X, mapSize.Y);
        _tileOffset = new Vector2(1, (float)Constants.TileHeight / 2);
        Elevatable.Initialize(mapSize, this);
        ClearTilemaps();
        _tilesForDataInitialization = tiles.ToList();
        _tilesForVisualsInitialization = tiles.ToList();
        
        _initialized = false;
    }

    public override void _Process(double delta)
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
            {
                IterateVisualInitialization();
                continue;
            }

            if (_pointsInitialized is false && _pointsStartedInitializing)
            {
                IteratePointInitialization();
            }
        }
        
        _stopwatch.Stop();
    }

    private void CheckInitialization()
    {
        if (_pointsInitialized && _pointsStartedInitializing)
        {
            FinishedPointInitialization();
            _initialized = true;
            return;
        }

        if (_dataInitialized && _visualsInitialized && _initialInitializationDone is false)
        {
            FinishedInitialInitializing();
            _initialInitializationDone = true;
        }
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
        var tile = new TileInstance
        {
            Position = position,
            Blueprint = blueprintId,
            Terrain = GetBlueprint(blueprintId).Terrain,
            Occupants = new List<EntityNode>()
        };
        _tiles[(position, false)] = tile;
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

        var positionI = new Vector2I((int)position.X, (int)position.Y);
        _grass.SetCell(0, positionI, TileMapGrassIndex); // needed to fill up gaps
        SetCell(positionI, GetTerrain(position));

        Elevatable.SetMapWideTargetTiles(godotPosition);
    }

    public void AddPoints(IEnumerable<Point> points)
    {
        _pointsForInitialization = points.ToList();
        _pointsStartedInitializing = true;
    }
    
    private void IteratePointInitialization()
    {
        if (_pointsForInitialization.IsEmpty())
        {
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(Tiles)}.{nameof(IteratePointInitialization)} completed");
            _pointsInitialized = true;
            return;
        }

        var point = _pointsForInitialization[0];
        _pointsForInitialization.RemoveAt(0);
        
        var tile = GetTile(point.Position);
        tile.Point = point;
    }

    public Vector2<int> GetMapPositionFromGlobalPosition(Vector2 globalPosition) 
        => _grass.LocalToMap(globalPosition) - _tilemapOffset;
    
    public Vector2 GetGlobalPositionFromMapPosition(Vector2<int> mapPosition) 
    {
        var adjustedMapPosition = new Vector2I(
            (int)(mapPosition.X + _tilemapOffset.X), 
            (int)(mapPosition.Y + _tilemapOffset.Y));
    
        var localPosition = _grass.MapToLocal(adjustedMapPosition);
    
        return _grass.ToGlobal(localPosition) + _tileOffset;
    }

    public IEnumerable<Vector2> GetGlobalPositionsFromMapPoints(IEnumerable<Point> points) => points
        .Select(x => GetTile(x.Position, x.IsHighGround))
        .Select(x => GetGlobalPositionFromMapPosition(x.Position) + Vector2.Up * 
            (x.YSpriteOffset > 0 && ClientState.Instance.Flattened 
                ? Constants.FlattenedHighGroundHeight 
                : x.YSpriteOffset));

    public Terrain GetTerrain(TileInstance tile) => tile is null 
        ? Terrain.Mountains 
        : GetTerrain(tile.Position, tile.Point.IsHighGround);
    
    public Terrain GetTerrain(Vector2<int> at, bool isHighGround = false) => at.IsInBoundsOf(_mapSize)
        ? GetTile(at, isHighGround).Terrain
        : Terrain.Mountains;

    public IList<TileInstance> GetEntityTiles(EntityNode entity) 
        => entity.EntityOccupyingPositions.Select(position => entity is UnitNode unit && unit.IsOnHighGround 
            ? GetHighestTile(position) 
            : GetTile(position, false)).ToList();

    public IList<TileInstance> GetHighestTiles(IList<Vector2<int>> at) => at.Select(GetHighestTile).ToList();

    public TileInstance GetHighestTile(Vector2<int> at) => GetTile(at, true) ?? GetTile(at);

    public TileInstance GetTile(Vector2<int> at, bool isHighGround = false) => at.IsInBoundsOf(_mapSize) 
        ? _tiles.ContainsKey((at, isHighGround)) 
            ? _tiles[(at, isHighGround)] 
            : null 
        : null;
    
    public TileInstance GetTile(Point point) => GetTile(point.Position, point.IsHighGround);

    public Tile GetBlueprint(TileId of) => _tilesBlueprint.SingleOrDefault(x => x.Id.Equals(of));
    
    public bool IsOccupied(TileInstance tile, EntityNode by = null) => by is null 
        ? tile.Occupants.Any() 
        : tile.Occupants.Contains(by);

    public void AddOccupation(EntityNode entity)
    {
        foreach (var tile in GetEntityTiles(entity))
        {
            if (IsOccupied(tile, entity))
                continue;
            
            tile.Occupants.Add(entity);
        }
        
        if (entity is StructureNode structure)
            StructureFoundations.AddOccupation(structure);
    }

    public void RemoveOccupation(EntityNode entity)
    {
        foreach (var tile in GetEntityTiles(entity))
        {
            tile.Occupants.Remove(entity);
        }
        
        if (entity is StructureNode structure)
            StructureFoundations.RemoveOccupation(structure);
    }

    public void FillMapOutsideWithMountains()
    {
        for (var y = _mountainsFillOffset * -1; y < _mapSize.Y + _mountainsFillOffset; y++) 
        for (var x = _mountainsFillOffset * -1; x < _mapSize.X + _mountainsFillOffset; x++) 
            if (x < 0 || x >= _mapSize.X || y < 0 || y >= _mapSize.Y)
                _mountains.SetCell(x, y, TileMapMountainsIndex);
    }

    public void UpdateALlBitmaps()
    {
        _grass.UpdateBitmaskRegion(Vector2.Zero, new Vector2(_mapSize.X, _mapSize.Y));
        _scraps.UpdateBitmaskRegion(Vector2.Zero, new Vector2(_mapSize.X, _mapSize.Y));
        _marsh.UpdateBitmaskRegion(Vector2.Zero, new Vector2(_mapSize.X, _mapSize.Y));
        _mountains.UpdateBitmaskRegion(
            new Vector2(_mountainsFillOffset * -1, _mountainsFillOffset * -1), 
            new Vector2(_mapSize.X + _mountainsFillOffset, _mapSize.Y + _mountainsFillOffset));
    }
    
    public IEnumerable<TileInstance> GetDilated(IEnumerable<Point> points, int size)
    {
        var tileSearchSet = new HashSet<(Vector2<int>, bool)>();
        foreach (var point in points)
        {
            for (var xOffset = 0; xOffset < size; xOffset++)
            {
                for (var yOffset = 0; yOffset < size; yOffset++)
                {
                    var resultingCoordinates = point.Position + new Vector2<int>(xOffset, yOffset);
                    
                    if (resultingCoordinates.IsInBoundsOf(_mapSize))
                        tileSearchSet.Add((resultingCoordinates, point.IsHighGround));
                }
            }
        }

        return tileSearchSet.Select(x => GetTile(x.Item1, x.Item2) ?? GetHighestTile(x.Item1));
    }

    private void SetCell(Vector2I at, Terrain terrain)
    {
        var tilemapIndex = GetTilemapIndexFrom(terrain);
        
        switch (tilemapIndex)
        {
            case TileMapMountainsIndex:
                _mountains.SetCell(0, at, TileMapMountainsIndex);
                break;
            case TileMapMarshIndex:
                _marsh.SetCell(0, at, TileMapMarshIndex);
                break;
            case TileMapScrapsIndex:
                _scraps.SetCell(0, at, TileMapScrapsIndex);
                break;
            case TileMapCelestiumIndex:
                _grass.SetCell(0, at, TileMapCelestiumIndex);
                break;
            case TileMapGrassIndex:
            default:
                _grass.SetCell(0, at, TileMapGrassIndex);
                break;
        }
    }
    
    private int GetTilemapIndexFrom(Terrain terrain) => _tileMapIndexesByTerrain.ContainsKey(terrain) 
        ? _tileMapIndexesByTerrain[terrain] 
        : TileMapGrassIndex;
    
    private void ClearTilemaps()
    {
        Elevatable.ClearPath();
        Elevatable.ClearAvailableTiles(true);
        Elevatable.ClearAvailableTiles(false);
        Elevatable.ClearTargetTiles();
        _grass.Clear();
        _scraps.Clear();
        _marsh.Clear();
        _mountains.Clear();
    }

    private void OnPathfindingUpdating(IPathfindingUpdatable data, bool isAdded)
    {
        if (isAdded is false)
            return;

        foreach (var (position, ySpriteOffset) in data.FlattenedPositions)
        {
            var tile = GetTile(position, true);
            if (tile is null)
            {
                tile = new TileInstance
                {
                    Position = position,
                    Blueprint = TileId.HighGround,
                    Terrain = Terrain.HighGround,
                    IsTarget = false,
                    Occupants = new List<EntityNode>(),
                    Point = null,
                    YSpriteOffset = ySpriteOffset
                };
                _tiles[(position, true)] = tile;
                
                continue;
            }
            
            tile.YSpriteOffset = ySpriteOffset;
        }
    }

    private void OnHighGroundPointCreated(Point point)
    {
        var tile = GetTile(point.Position, true);
        if (tile is null)
        {
            tile = new TileInstance
            {
                Position = point.Position,
                Blueprint = TileId.HighGround,
                Terrain = Terrain.HighGround,
                IsTarget = false,
                Occupants = new List<EntityNode>(),
                Point = point,
                YSpriteOffset = 0
            };
            _tiles[(point.Position, true)] = tile;

            return;
        }

        tile.Point = point;
    }

    private void OnHighGroundPointRemoved(Point point) => _tiles.Remove((point.Position, true));
}
