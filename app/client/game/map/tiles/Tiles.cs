using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using low_age_data.Domain.Common;
using low_age_data.Domain.Tiles;
using low_age_prototype_common.Extensions;

/// <summary>
/// Responsible for: <para />
/// - Keeping track of all tiles data <para />
/// - Handling all tile-map related visuals <para />
/// - Conversions between global and map positions
/// </summary>
public class Tiles : Node2D
{
    /// <summary>
    /// Wrapper of <see cref="Point"/>.
    /// </summary>
    public class TileInstance
    {
        public Vector2 Position { get; set; }
        public TileId Blueprint { get; set; }
        public Terrain Terrain { get; set; }
        public bool IsTarget { get; set; }
        public IList<EntityNode> Occupants { get; set; }
        public Point Point { get; set; }
        public int YSpriteOffset { get; set; }

        public bool IsInBoundsOf(Vector2 bounds) => Position.IsInBoundsOf(bounds);
    }
    
    public event Action FinishedInitialInitializing = delegate { };
    public event Action FinishedPointInitialization = delegate { };
    
    public StructureFoundations StructureFoundations { get; private set; }
    public ElevatableTiles Elevatable { get; private set; }
    
    private readonly Dictionary<(Vector2, bool), TileInstance> _tiles = new Dictionary<(Vector2, bool), TileInstance>();
    private IList<Tile> _tilesBlueprint;
    private Vector2 _mapSize;
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
    private IList<(Vector2, TileId)> _tilesForDataInitialization;
    private bool _visualsInitialized = false;
    private IList<(Vector2, TileId)> _tilesForVisualsInitialization;
    private bool _initialInitializationDone = false;
    private IList<Point> _pointsForInitialization;
    private bool _pointsStartedInitializing = false;
    private bool _pointsInitialized = false;
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
        Elevatable = GetNode<ElevatableTiles>($"{nameof(ElevatableTiles)}");

        EventBus.Instance.HighGroundPointCreated += OnHighGroundPointCreated;
        EventBus.Instance.HighGroundPointRemoved += OnHighGroundPointRemoved;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        EventBus.Instance.HighGroundPointCreated -= OnHighGroundPointCreated;
        EventBus.Instance.HighGroundPointRemoved -= OnHighGroundPointRemoved;
    }

    public void Initialize(Vector2 mapSize, ICollection<(Vector2, TileId)> tiles)
    {
        _tilesBlueprint = Data.Instance.Blueprint.Tiles;
        _mapSize = mapSize;
        _tilemapOffset = new Vector2(mapSize.x / 2, (mapSize.y / 2) * -1);
        _mountainsFillOffset = (int)Mathf.Max(mapSize.x, mapSize.y);
        _tileOffset = new Vector2(1, (float)Constants.TileHeight / 2);
        Elevatable.Initialize(mapSize, this);
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
        
        _grass.SetCellv(position, TileMapGrassIndex); // needed to fill up gaps
        SetCell(position, GetTerrain(position));

        Elevatable.SetMapWideTargetTiles(position);
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

    public Vector2 GetMapPositionFromGlobalPosition(Vector2 globalPosition) 
        => _grass.WorldToMap(globalPosition) - _tilemapOffset;
    
    public Vector2 GetGlobalPositionFromMapPosition(Vector2 mapPosition) 
        => _grass.MapToWorld(mapPosition + _tilemapOffset, true) + _tileOffset;

    public IEnumerable<Vector2> GetGlobalPositionsFromMapPoints(IEnumerable<Point> points) => points
        .Select(x => GetTile(x.Position, x.IsHighGround))
        .Select(x => GetGlobalPositionFromMapPosition(x.Position) + Vector2.Up * 
            (x.YSpriteOffset > 0 && ClientState.Instance.Flattened 
                ? Constants.FlattenedHighGroundHeight 
                : x.YSpriteOffset));

    public Terrain GetTerrain(TileInstance tile) => tile is null 
        ? Terrain.Mountains 
        : GetTerrain(tile.Position, tile.Point.IsHighGround);
    
    public Terrain GetTerrain(Vector2 at, bool isHighGround = false) => at.IsInBoundsOf(_mapSize)
        ? GetTile(at, isHighGround).Terrain
        : Terrain.Mountains;

    public IList<TileInstance> GetEntityTiles(EntityNode entity) 
        => entity.EntityOccupyingPositions.Select(position => entity is UnitNode unit && unit.IsOnHighGround 
            ? GetHighestTile(position) 
            : GetTile(position, false)).ToList();

    public IList<TileInstance> GetHighestTiles(IList<Vector2> at) => at.Select(GetHighestTile).ToList();

    public TileInstance GetHighestTile(Vector2 at) => GetTile(at, true) ?? GetTile(at);

    public TileInstance GetTile(Vector2 at, bool isHighGround = false) => at.IsInBoundsOf(_mapSize) 
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
    
    public IEnumerable<TileInstance> GetDilated(IEnumerable<Point> points, int size)
    {
        var tileSearchSet = new HashSet<(Vector2, bool)>();
        foreach (var point in points)
        {
            for (var xOffset = 0; xOffset < size; xOffset++)
            {
                for (var yOffset = 0; yOffset < size; yOffset++)
                {
                    var resultingCoordinates = point.Position + new Vector2(xOffset, yOffset);
                    
                    if (resultingCoordinates.IsInBoundsOf(_mapSize))
                        tileSearchSet.Add((resultingCoordinates, point.IsHighGround));
                }
            }
        }

        return tileSearchSet.Select(x => GetTile(x.Item1, x.Item2) ?? GetHighestTile(x.Item1));
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

    private void OnHighGroundPointCreated(Point point, int ySpriteOffset)
    {
        var tile = new TileInstance
        {
            Position = point.Position,
            Blueprint = TileId.HighGround,
            Terrain = Terrain.HighGround,
            IsTarget = false,
            Occupants = new List<EntityNode>(),
            Point = point,
            YSpriteOffset = ySpriteOffset
        };
        _tiles[(point.Position, true)] = tile;
    }

    private void OnHighGroundPointRemoved(Point point) => _tiles.Remove((point.Position, true));
}
