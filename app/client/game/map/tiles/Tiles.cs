using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot.Collections;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Tiles;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using MultipurposePathfinding;

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
        public Vector2Int Position { get; init; }
        public required TileId Blueprint { get; init; }
        public required Terrain Terrain { get; init; }
        public TargetType TargetType { get; set; }
        public required IList<EntityNode> Occupants { get; init; }
        public Point Point { get; set; } = null!;
        public int YSpriteOffset { get; set; }
        
        public bool IsOccupied(EntityNode? by = null) => by is null 
            ? Occupants.Any() 
            : Occupants.Contains(by);
    }
    
    public event Action FinishedInitialInitializing = delegate { };
    public event Action FinishedPointInitialization = delegate { };
    
    public StructureFoundations StructureFoundations { get; private set; } = null!;
    public ElevatableTiles Elevatable { get; private set; } = null!;

    private readonly System.Collections.Generic.Dictionary<(Vector2Int, bool), TileInstance> _tiles = new();
    private IList<Tile> _tilesBlueprint = null!;
    private Vector2Int _mapSize;
    private int _mountainsFillOffset;
    private TileMapLayer _grass = null!;
    private TileMapLayer _scraps = null!;
    private TileMapLayer _marsh = null!;
    private TileMapLayer _mountains = null!;
    private bool _connectTerrain = Config.Instance.ConnectTerrain;

    private const int SourceId = 0;
    private const int TerrainSetIndex = 0;
    private const int TileMapGrassIndex = 10;
    private readonly Vector2I _tileMapGrassAtlasPosition = new(2, 0);
    private const int TileMapCelestiumIndex = 2;
    private readonly Vector2I _tileMapCelestiumAtlasPosition = new(12, 0);
    private const int TileMapScrapsIndex = 3;
    private readonly Vector2I _tileMapScrapsAtlasPosition = new(0, 18);
    private const int TileMapMarshIndex = 4;
    private readonly Vector2I _tileMapMarshAtlasPosition = new(0, 8);
    private const int TileMapMountainsIndex = 5;
    private readonly Vector2I _tileMapMountainsAtlasPosition = new(0, 13);
    
    private readonly System.Collections.Generic.Dictionary<Terrain, int> _tileMapIndexesByTerrain = new()
    {
        { Terrain.Grass,     TileMapGrassIndex },
        { Terrain.Mountains, TileMapMountainsIndex },
        { Terrain.Marsh,     TileMapMarshIndex },
        { Terrain.Scraps,    TileMapScrapsIndex },
        { Terrain.Celestium, TileMapCelestiumIndex }
    };

    private const int InitializationChunk = 2000;
    private bool _dataInitialized = false;
    private List<(Vector2Int, TileId)> _tilesForDataInitialization = null!;
    private bool _mainVisualsInitialized = false;
    private List<(Vector2Int, TileId)> _tilesForMainVisualsInitialization = null!;
    private bool _outsideVisualsInitialized = false;
    private List<Vector2Int> _positionsForOutsideVisualsInitialization = null!;
    private bool _initialInitializationDone = false;
    private List<Point> _pointsForInitialization = null!;
    private bool _pointsStartedInitializing = false;
    private bool _pointsInitialized = false;
    private bool _initialized = true;
    private readonly Stopwatch _stopwatch = new();

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        
        _grass = GetNode<TileMapLayer>("Grass");
        _scraps = GetNode<TileMapLayer>("Scraps");
        _marsh = GetNode<TileMapLayer>("Marsh");
        _mountains = GetNode<TileMapLayer>("Stone");

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

    public void Initialize(Vector2 mapSize, ICollection<(Vector2Int, TileId)> tiles)
    {
        _tilesBlueprint = Data.Instance.Blueprint.Tiles;
        _mapSize = new Vector2Int((int)mapSize.X, (int)mapSize.Y);
        _mountainsFillOffset = (int)Mathf.Max(mapSize.X, mapSize.Y);
        Elevatable.Initialize(mapSize, this);
        ClearTilemaps();
        _tilesForDataInitialization = tiles.ToList();
        _tilesForMainVisualsInitialization = tiles.ToList();
        _positionsForOutsideVisualsInitialization = GetOutsideVisualPositions();
        
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

            if (_mainVisualsInitialized is false)
            {
                IterateVisualInitialization();
                continue;
            }

            if (_outsideVisualsInitialized is false)
            {
                IterateOutsideVisualInitialization();
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

        if (_dataInitialized 
            && _mainVisualsInitialized 
            && _outsideVisualsInitialized
            && _initialInitializationDone is false)
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
        if (_tilesForMainVisualsInitialization.IsEmpty())
        {
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(Tiles)}.{nameof(IterateVisualInitialization)} completed");
            _mainVisualsInitialized = true;
            return;
        }

        var (pos, _) = _tilesForMainVisualsInitialization[0];
        _tilesForMainVisualsInitialization.RemoveAt(0);

        var position = new Array<Vector2I>{ pos.ToGodotVector2I() };
        if (_connectTerrain)
            _grass.SetCellsTerrainConnect(position, TerrainSetIndex, TileMapGrassIndex, false);
        else
            _grass.SetCell(pos.ToGodotVector2I(), SourceId, _tileMapGrassAtlasPosition);
        
        SetCells(position, GetTerrain(pos));
        Elevatable.SetMapWideTargetTiles(pos.ToGodotVector2I());
    }
    
    private List<Vector2Int> GetOutsideVisualPositions()
    {
        var positions = new List<Vector2Int>();
        for (var y = _mountainsFillOffset * -1; y < _mapSize.Y + _mountainsFillOffset; y++) 
        for (var x = _mountainsFillOffset * -1; x < _mapSize.X + _mountainsFillOffset; x++) 
            if (x < 0 || x >= _mapSize.X || y < 0 || y >= _mapSize.Y)
                positions.Add(new Vector2Int(x, y));

        return positions;
    }
    
    private void IterateOutsideVisualInitialization()
    {
        if (_positionsForOutsideVisualsInitialization.IsEmpty())
        {
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(Tiles)}.{nameof(IterateOutsideVisualInitialization)} completed");
            _outsideVisualsInitialized = true;
            return;
        }
        
        var positions = _positionsForOutsideVisualsInitialization.Take(InitializationChunk);
        _positionsForOutsideVisualsInitialization.RemoveRange(0, InitializationChunk);

        if (_connectTerrain)
        {
            _mountains.SetCellsTerrainConnect(
                positions.Select(x => x.ToGodotVector2I()).ToGodotArray(), 
                TerrainSetIndex, TileMapMountainsIndex);
        }
        else
        {
            foreach (var position in positions)
            {
                _mountains.SetCell(position.ToGodotVector2I(), SourceId, _tileMapMountainsAtlasPosition);
            }
        }
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
        
        var tile = GetTile(point.Position, false);
        tile!.Point = point;
    }

    public Vector2Int GetMapPositionFromGlobalPosition(Vector2 globalPosition)
    {
        var localPosition = ToLocal(globalPosition);
        return _grass.LocalToMap(localPosition).ToVector2();
    }

    public Vector2 GetGlobalPositionFromMapPosition(Vector2Int mapPosition) 
    {
        var adjustedMapPosition = mapPosition.ToGodotVector2I();
    
        var localPosition = _grass.MapToLocal(adjustedMapPosition);
    
        return _grass.ToGlobal(localPosition);
    }

    public IEnumerable<Vector2> GetGlobalPositionsFromMapPoints(IEnumerable<Point> points) => points
        .Select(x => GetTile(x.Position, x.IsHighGround))
        .Select(x => GetGlobalPositionFromMapPosition(x!.Position) + Vector2.Up * 
            (x.YSpriteOffset > 0 && ClientState.Instance.Flattened 
                ? Constants.FlattenedHighGroundHeight 
                : x.YSpriteOffset));

    public Terrain GetTerrain(TileInstance? tile) => tile is null 
        ? Terrain.Mountains 
        : GetTerrain(tile.Position, tile.Point.IsHighGround);
    
    public Terrain GetTerrain(Vector2Int at, bool isHighGround = false) => (at.IsInBoundsOf(_mapSize)
        ? GetTile(at, isHighGround)?.Terrain
        : Terrain.Mountains) ?? Terrain.Mountains;

    public IList<TileInstance> GetEntityTiles(EntityNode entity) 
        => entity.EntityOccupyingPositions.Select(position => entity is UnitNode { IsOnHighGround: true }
            ? GetHighestTile(position) 
            : GetTile(position, false)).WhereNotNull().ToList();

    public IList<TileInstance?> GetHighestTiles(IList<Vector2Int> at) => at.Select(GetHighestTile).ToList();

    public TileInstance? GetHighestTile(Vector2Int at) => GetTile(at, true) ?? GetTile(at, false);

    public TileInstance? GetTile(Vector2Int at, bool isHighGround) => at.IsInBoundsOf(_mapSize) 
        ? _tiles.ContainsKey((at, isHighGround)) 
            ? _tiles[(at, isHighGround)] 
            : null 
        : null;
    
    public TileInstance? GetTile(Point point) => GetTile(point.Position, point.IsHighGround);

    public Tile GetBlueprint(TileId of) => _tilesBlueprint.Single(x => x.Id.Equals(of));

    public void AddOccupation(EntityNode entity)
    {
        foreach (var tile in GetEntityTiles(entity))
        {
            if (tile.IsOccupied(entity))
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
    
    public IEnumerable<TileInstance> GetDilated(IEnumerable<Point> points, int size)
    {
        var tileSearchSet = new HashSet<(Vector2Int, bool)>();
        foreach (var point in points)
        {
            foreach (var offset in IterateVector2Int.Positions(Vector2Int.One * size))
            {
                var resultingCoordinates = point.Position + offset;
                    
                if (resultingCoordinates.IsInBoundsOf(_mapSize))
                    tileSearchSet.Add((resultingCoordinates, point.IsHighGround));
            }
        }

        return tileSearchSet
            .Select(x => GetTile(x.Item1, x.Item2) ?? GetHighestTile(x.Item1))
            .WhereNotNull();
    }

    private void SetCells(IList<Vector2I> at, Terrain terrain)
    {
        var tilemapIndex = GetTilemapIndexFrom(terrain);
        var positions = at.ToGodotArray();
        
        switch (tilemapIndex)
        {
            case TileMapMountainsIndex:
                if (_connectTerrain)
                    _mountains.SetCellsTerrainConnect(positions, TerrainSetIndex, TileMapMountainsIndex);
                else
                {
                    foreach (var position in positions)
                    {
                        _mountains.SetCell(position, SourceId, _tileMapMountainsAtlasPosition);
                    }
                }
                break;
            case TileMapMarshIndex:
                if (_connectTerrain)
                    _marsh.SetCellsTerrainConnect(positions, TerrainSetIndex, TileMapMarshIndex);
                else
                {
                    foreach (var position in positions)
                    {
                        _marsh.SetCell(position, SourceId, _tileMapMarshAtlasPosition);
                    }
                }
                break;
            case TileMapScrapsIndex:
                if (_connectTerrain)
                    _scraps.SetCellsTerrainConnect(positions, TerrainSetIndex, TileMapScrapsIndex);
                else
                {
                    foreach (var position in positions)
                    {
                        _scraps.SetCell(position, SourceId, _tileMapScrapsAtlasPosition);
                    }
                }
                break;
            case TileMapCelestiumIndex:
                if (_connectTerrain)
                    _grass.SetCellsTerrainConnect(positions, TerrainSetIndex, TileMapCelestiumIndex);
                else
                {
                    foreach (var position in positions)
                    {
                        _grass.SetCell(position, SourceId, _tileMapCelestiumAtlasPosition);
                    }
                }
                break;
            case TileMapGrassIndex:
            default:
                if (_connectTerrain)
                    _grass.SetCellsTerrainConnect(positions, TerrainSetIndex, TileMapGrassIndex);
                else
                {
                    foreach (var position in positions)
                    {
                        _grass.SetCell(position, SourceId, _tileMapGrassAtlasPosition);
                    }
                }
                break;
        }
    }

    private int GetTilemapIndexFrom(Terrain terrain) 
        => _tileMapIndexesByTerrain.GetValueOrDefault(terrain, TileMapGrassIndex);
    
    private void ClearTilemaps()
    {
        Elevatable.ClearPath();
        Elevatable.ClearAvailableTiles(true);
        Elevatable.ClearAvailableTiles(false);
        Elevatable.ClearTargetTiles(true);
        Elevatable.ClearTargetTiles(false);
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
                    TargetType = TargetType.None,
                    Occupants = new List<EntityNode>(),
                    Point = null!,
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
                TargetType = TargetType.None,
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
