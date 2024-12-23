using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using low_age_data;
using low_age_data.Domain.Common;
using low_age_data.Domain.Tiles;
using low_age_prototype_common.Extensions;
using multipurpose_pathfinding;
using Newtonsoft.Json;

public class Pathfinding : Node
{
    [Export] public bool DebugEnabled = true;

    public event Action FinishedInitializing = delegate { };

    public const string ScenePath = @"res://app/shared/game/map/pathfinding/Pathfinding.tscn";

    private const int MaxSizeForPathfinding = Constants.Pathfinding.MaxSizeForPathfinding;
    private const float DiagonalCost = Constants.Pathfinding.DiagonalCost;
    private const int HighGroundTolerance = 13; // works until approx 18 levels of ascension

    public Graph Graph { get; } = new Graph();

    private Vector2 MapSize { get; set; }
    private Blueprint Blueprint { get; set; }
    private Dictionary<Vector2, TileId> Tiles { get; set; } = new Dictionary<Vector2, TileId>();

    private const int ImpassableIndex = Constants.Pathfinding.ImpassableIndex;
    private const int HighGroundIndex = Constants.Pathfinding.HighGroundIndex;

    private Vector2 _previousPosition = Vector2.Inf;
    private float _previousRange = -1.0f;
    private bool _previousIsOnHighGround = false;
    private int _previousTeam = 1;
    private int _previousSize = 1;

    private bool _iterateInitialization = false;
    private bool _terrainGraphFirstPassInitialized = false;
    private bool _terrainGraphFurtherPassesInitialized = false;
    private IList<(Vector2, TileId)> _tilesForFirstPassInitialization;
    private Dictionary<TileId, IList<Vector2>> _tilesForFurtherInitialization;
    private TileId _tileIdWithSmallestMovementCost;
    private bool _diagonalConnectionsInitialized = false;
    private Dictionary<Vector2, int> _pointIdsByPositionsForInitialization;
    private bool _initialized = true;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    public override void _Ready()
    {
        base._Ready();
        PauseMode = PauseModeEnum.Process;

        EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        EventBus.Instance.PathfindingUpdating -= OnPathfindingUpdating;
    }

    #region Initialization

    public void Initialize(Vector2 mapSize, IEnumerable<(Vector2, TileId)> tiles, int? forceSizesUpTo = null)
    {
        MapSize = mapSize;
        Blueprint = Data.Instance.Blueprint;
        Graph.Initialize(mapSize, forceSizesUpTo: forceSizesUpTo);

        _iterateInitialization = true;

        var tileIdsWithAscendingMovementCost = Blueprint.Tiles
            .OrderBy(x => x.MovementCost)
            .Select(x => x.Id)
            .ToList();
        _tileIdWithSmallestMovementCost = tileIdsWithAscendingMovementCost.First();
        _tilesForFurtherInitialization = new Dictionary<TileId, IList<Vector2>>();
        foreach (var tileId in tileIdsWithAscendingMovementCost
                     .Where(tileId => tileId.Equals(_tileIdWithSmallestMovementCost) is false))
        {
            _tilesForFurtherInitialization[tileId] = new List<Vector2>();
        }

        var tilesList = tiles.ToList();
        _tilesForFirstPassInitialization = tilesList;
        foreach (var (position, tile) in tilesList)
            Tiles.Add(position, tile);

        InitializePathfindingGraphs();

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

        while (_stopwatch.ElapsedMilliseconds < deltaMs || (_iterateInitialization && CheckInitialization() is false))
        {
            if (_terrainGraphFirstPassInitialized is false)
            {
                IterateTerrainGraphFirstPassUpdate();
                continue;
            }
            
            if (_terrainGraphFurtherPassesInitialized is false)
            {
                IterateTerrainGraphFurtherPassUpdate();
                continue;
            }

            if (_diagonalConnectionsInitialized is false)
                IterateDiagonalConnectionUpdate();
        }

        _stopwatch.Stop();
    }

    private bool CheckInitialization()
    {
        if (_terrainGraphFirstPassInitialized is false
            || _terrainGraphFurtherPassesInitialized is false
            || _diagonalConnectionsInitialized is false)
            return false;

        _initialized = true;
        FinishedInitializing();
        return _initialized;
    }

    private void InitializePathfindingGraphs()
    {
        _pointIdsByPositionsForInitialization = Graph.InitializeSquareGrid(
            new Rect2(0, 0, MapSize.x, MapSize.y),
            Terrain.Grass.ToIndex());
    }

    #endregion Initialization

    #region IterationHelpers

    private void IterateTerrainGraphFirstPassUpdate()
    {
        if (_tilesForFirstPassInitialization.IsEmpty())
        {
            if (DebugEnabled)
                GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                         $"{nameof(Pathfinding)}.{nameof(IterateTerrainGraphFirstPassUpdate)} completed");
            _terrainGraphFirstPassInitialized = true;
            TrimEmptyTilesForFurtherInitialization();
            return;
        }
        
        var entry = _tilesForFirstPassInitialization[0];
        _tilesForFirstPassInitialization.RemoveAt(0);

        var (coordinates, tileId) = entry;

        if (tileId.Equals(_tileIdWithSmallestMovementCost))
        {
            var terrainIndex = Blueprint.Tiles.Single(x => x.Id.Equals(tileId)).Terrain.ToIndex();
            Graph.SetTerrainForPoint(coordinates, terrainIndex);
            return;
        }
        
        _tilesForFurtherInitialization[tileId].Add(coordinates);
    }
    
    private void IterateTerrainGraphFurtherPassUpdate()
    {
        if (_tilesForFurtherInitialization.IsEmpty())
        {
            if (DebugEnabled)
                GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                         $"{nameof(Pathfinding)}.{nameof(IterateTerrainGraphFurtherPassUpdate)} completed");
            _terrainGraphFurtherPassesInitialized = true;
            return;
        }

        var tileId = FindTileIdKeyWithLowestMovementCost();
        var coordinates = _tilesForFurtherInitialization[tileId][0];
        _tilesForFurtherInitialization[tileId].RemoveAt(0);
        TrimEmptyTilesForFurtherInitialization();
        
        var terrainIndex = Blueprint.Tiles.Single(x => x.Id.Equals(tileId)).Terrain.ToIndex();
        Graph.SetTerrainForPoint(coordinates, terrainIndex);
    }

    private void IterateDiagonalConnectionUpdate()
    {
        if (_pointIdsByPositionsForInitialization.IsEmpty())
        {
            if (DebugEnabled)
                GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                         $"{nameof(Pathfinding)}.{nameof(IterateDiagonalConnectionUpdate)} completed");
            _diagonalConnectionsInitialized = true;
            return;
        }

        var position = _pointIdsByPositionsForInitialization.First().Key;
        _pointIdsByPositionsForInitialization.Remove(position);

        Graph.UpdateDiagonalConnection(position);
    }
    
    private TileId FindTileIdKeyWithLowestMovementCost()
    {
        var tileIdsWithAscendingMovementCost = Blueprint.Tiles
            .OrderBy(x => x.MovementCost)
            .ToList();

        return tileIdsWithAscendingMovementCost
            .Where(nextTile => _tilesForFurtherInitialization.ContainsKey(nextTile.Id))
            .Select(nextTile => nextTile.Id).FirstOrDefault();
    }
    
    private void TrimEmptyTilesForFurtherInitialization()
    {
        var keysToRemove = _tilesForFurtherInitialization
            .Where(entry => entry.Value.Count == 0)
            .Select(entry => entry.Key).ToList();

        foreach (var key in keysToRemove)
        {
            _tilesForFurtherInitialization.Remove(key);
        }
    }

    #endregion IterationHelpers

    public void ClearCache() => Cache(Vector2.Inf, -1.0f, false, 1, 1);

    public IEnumerable<Point> GetAvailablePoints(Vector2 from, float range, bool isOnHighGround, int size,
        bool temporary = false)
    {
        if (size > MaxSizeForPathfinding)
            throw new ArgumentException($"{nameof(Pathfinding)}." +
                                        $"{nameof(GetAvailablePoints)}: argument " +
                                        $"{nameof(size)} '{size}' exceeded maximum " +
                                        $"value of '{MaxSizeForPathfinding}'.");

        var team = 1; // TODO add team

        if (Graph.IsSupported(team, size) is false)
            return Enumerable.Empty<Point>();

        if (IsCached(from, range, isOnHighGround, team, size) is false)
        {
            Graph.Recalculate(from, range, isOnHighGround, team, size);

            if (temporary is false)
                Cache(from, range, isOnHighGround, team, size);
        }

        var availablePositions = Graph.GetAllPointsWithCostBetween(0.0f, range, team, size);

        if (temporary && team == _previousTeam 
                      && size == _previousSize 
                      && Graph.TryGetPointId(_previousPosition, team, size, out var previousPointId,
                          _previousIsOnHighGround))
        {
            Graph.Recalculate(previousPointId, _previousRange, _previousTeam, _previousSize);
        }

        return availablePositions;
    }

    public IEnumerable<Point> FindPath(Point to, int size)
    {
        var team = 1; // TODO add team

        if (to.Position.IsInBoundsOf(MapSize) is false || Graph.IsSupported(team, size) is false)
            return Enumerable.Empty<Point>();

        var points = Graph.GetShortestPathFromPoint(to.Id, team, size);

        var path = new List<Point> { Graph.GetPoint(to.Id, team, size) };
        path.AddRange(points);
        path.Reverse();

        return path;
    }

    public void AddOccupation(EntityNode entity)
    {
        var team = 1; // TODO add team
        var size = 1; // TODO move AddOccupation to Graph and automatically handle adding
        // occupation for all graph sizes

        var foundPoints = GetPointsProjectedDownFromEntity(entity)
            .Where(point => entity.CanBeMovedThroughAt(point, team) is false)
            .ToList();

        Graph.SetPointsImpassable(true, foundPoints);

        foreach (var foundPosition in foundPoints
                     .Where(x => x.IsHighGround is false)
                     .Select(x => x.Position))
        {
            for (var x = (int)foundPosition.x - MaxSizeForPathfinding;
                 x <= (int)foundPosition.x + MaxSizeForPathfinding;
                 x++)
            {
                for (var y = (int)foundPosition.y - MaxSizeForPathfinding;
                     y <= (int)foundPosition.y + MaxSizeForPathfinding;
                     y++)
                {
                    var position = new Vector2(x, y);
                    if (Graph.ContainsPoint(position, team, size) is false)
                        continue;

                    Graph.UpdateDiagonalConnection(position);
                }
            }
        }
    }

    public void RemoveOccupation(EntityNode entity) // TODO not tested properly
    {
        var team = 1; // TODO add team
        var size = 1; // TODO move AddOccupation to Graph and automatically handle adding
        // occupation for all graph sizes

        var start = entity.EntityPrimaryPosition - Vector2.One * MaxSizeForPathfinding;
        var end = entity.EntityPrimaryPosition + entity.EntitySize + Vector2.One * MaxSizeForPathfinding;

        /*var coordinatesByTerrain = _terrainWeights
            .OrderBy(x => x.Value)
            .ToDictionary<KeyValuePair<int, float>, int, IList<Vector2>>(terrainWeight =>
                terrainWeight.Key, terrainWeight => new List<Vector2>());*/

        foreach (var point in GetPointsProjectedDownFromEntity(entity))
        {
            if (point.IsHighGround)
                continue;

            var position = point.Position;
            if (position.IsInBoundsOf(entity.EntityPrimaryPosition,
                    entity.EntityPrimaryPosition + entity.EntitySize))
            {
                //IterateTerrainGraphUpdate(new KeyValuePair<Vector2, TileId>(position, Tiles[position]));
            }

            var terrainIndex = Blueprint.Tiles.Single(t => t.Id.Equals(Tiles[position])).Terrain.ToIndex();
            //coordinatesByTerrain[terrainIndex].Add(position);
        }

        // TODO
        //foreach (var pair in coordinatesByTerrain) 
        //IterateTerrainDilationUpdate(pair);

        for (var x = (int)start.x; x < (int)end.x; x++)
        {
            for (var y = (int)start.y; y < (int)end.y; y++)
            {
                var position = new Vector2(x, y);
                if (Graph.ContainsPoint(position, team, size) is false)
                    continue;

                Graph.UpdateDiagonalConnection(position);
            }
        }
    }

    public bool HasConnection(Point pointA, Point pointB, int team, int size)
    {
        if (Graph.IsSupported(team, size) is false)
            return false;

        return Graph.HasConnection(pointA, pointB, team, size);
    }

    private void OnPathfindingUpdating(IPathfindingUpdatable source, bool isAdded)
    {
        ClearCache();
        var positions = source.LeveledPositions;
        switch (source)
        {
            case AscendableNode _:
                if (isAdded)
                    AddAscendableHighGround(positions);
                else
                    RemoveAscendableHighGround(positions);
                return;
            case HighGroundNode _:
                if (isAdded)
                    AddHighGround(positions);
                else
                    RemoveHighGround(positions);
                return;
            default:
                return;
        }
    }

    private void AddAscendableHighGround(IList<(IEnumerable<Vector2>, int)> path)
    {
        if (path.IsEmpty())
            return;

        var currentAscensionLevel = 0;
        var ascensionGain = (int)(100f / path.Count);
        for (var i = 0; i < path.Count; i++)
        {
            currentAscensionLevel += ascensionGain;
            foreach (var pos in path[i].Item1)
            {
                if (pos.IsInBoundsOf(MapSize) is false)
                    continue;

                Point point;
                if (Graph.ContainsMainPoint(pos, true))
                {
                    point = Graph.GetMainPoint(pos, true);
                    if (DebugEnabled)
                        GD.Print($"1. Found existing point {JsonConvert.SerializeObject(point)}");
                }
                else
                {
                    point = Graph.AddPoint(pos, true, currentAscensionLevel);
                    EventBus.Instance.RaiseHighGroundPointCreated(point, path[i].Item2);
                    if (DebugEnabled)
                        GD.Print($"2. Creating point {JsonConvert.SerializeObject(point)}");
                }

                for (var xOffset = -1; xOffset < 2; xOffset++)
                {
                    for (var yOffset = -1; yOffset < 2; yOffset++)
                    {
                        var offset = new Vector2(xOffset, yOffset);
                        var isDiagonallyAdjacent = point.Position.IsDiagonalTo(point.Position + offset);

                        var lowGroundPoint = GetAdjacentPoint(point, offset, false);
                        if (i == 0 && lowGroundPoint != null)
                        {
                            Graph.ConnectPoints(point, (Point)lowGroundPoint, isDiagonallyAdjacent, 1);
                            if (DebugEnabled)
                                GD.Print($"3. Connecting {JsonConvert.SerializeObject(point)} to low ground " +
                                         $"point {JsonConvert.SerializeObject(lowGroundPoint)}, " +
                                         $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}. ");
                        }

                        if (path.Count > 1 && i != 0)
                        {
                            var offsetPosition = point.Position + offset;
                            var previousStepPositionExists = path[i - 1].Item1.Any(x =>
                                x.Equals(offsetPosition));
                            if (previousStepPositionExists)
                            {
                                var previousStepPosition = path[i - 1].Item1.FirstOrDefault(x =>
                                    x.Equals(offsetPosition));
                                var previousStepPoint = Graph.GetMainPoint(previousStepPosition, true);
                                Graph.ConnectPoints(point, previousStepPoint, isDiagonallyAdjacent, 1);
                                if (DebugEnabled)
                                    GD.Print($"4. Connecting {JsonConvert.SerializeObject(point)} to previous " +
                                             $"step point {JsonConvert.SerializeObject(previousStepPoint)}, " +
                                             $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}. ");
                            }
                        }

                        var highGroundPoint = GetAdjacentPoint(point, offset, true);
                        if (highGroundPoint is null)
                            continue;

                        Graph.ConnectPoints(point, (Point)highGroundPoint, isDiagonallyAdjacent, 1);
                        if (DebugEnabled)
                            GD.Print($"5. Connecting {JsonConvert.SerializeObject(point)} to adjacent high " +
                                     $"ground point {JsonConvert.SerializeObject(highGroundPoint)}, " +
                                     $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}. ");
                    }
                }

                UpdateHighGroundForNon1XPathfinding(point);
            }
        }
    }

    private void RemoveAscendableHighGround(IList<(IEnumerable<Vector2>, int)> path) // TODO not tested properly
    {
        RemoveHighGround(path);
    }

    private void AddHighGround(IList<(IEnumerable<Vector2>, int)> path)
    {
        var flattenedPositions = GetFlattenedPositions(path);

        foreach (var pos in flattenedPositions)
        {
            if (pos.IsInBoundsOf(MapSize) is false)
                continue;

            if (Graph.ContainsMainPoint(pos, true))
                continue;

            var point = Graph.AddPoint(pos, true, 100);
            EventBus.Instance.RaiseHighGroundPointCreated(point, path.First().Item2);
            if (DebugEnabled)
                GD.Print($"1. Creating point {JsonConvert.SerializeObject(point)}");

            for (var xOffset = -1; xOffset < 2; xOffset++)
            {
                for (var yOffset = -1; yOffset < 2; yOffset++)
                {
                    var offset = new Vector2(xOffset, yOffset);
                    var isDiagonallyAdjacent = point.Position.IsDiagonalTo(point.Position + offset);

                    var otherPoint = GetAdjacentPoint(point, offset, true);
                    if (otherPoint is null)
                        continue;

                    Graph.ConnectPoints(point, (Point)otherPoint, isDiagonallyAdjacent, 1);

                    if (DebugEnabled)
                        GD.Print($"5. Connecting {JsonConvert.SerializeObject(point)} to adjacent high " +
                                 $"ground point {JsonConvert.SerializeObject(otherPoint)}, " +
                                 $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}. ");
                }
            }

            UpdateHighGroundForNon1XPathfinding(point);
        }
    }

    private void RemoveHighGround(IList<(IEnumerable<Vector2>, int)> path) // TODO not tested properly
    {
        var flattenedPositions = GetFlattenedPositions(path);

        foreach (var pos in flattenedPositions)
        {
            if (pos.IsInBoundsOf(MapSize) is false)
                continue;

            if (Graph.ContainsMainPoint(pos, true) is false)
                continue;

            var point = Graph.GetMainPoint(pos, true);

            /*for (var xOffset = -1; xOffset < 2; xOffset++)
            {
                for (var yOffset = -1; yOffset < 2; yOffset++)
                {
                    var otherPoint = GetAdjacentHighGroundPoint(point, new Vector2(xOffset, yOffset));
                    if (otherPoint is null)
                        continue;

                    _pathfinding.RemoveConnection(point.Id, ((Point)otherPoint).Id);
                }
            }*/ // TODO test it out before removing

            RemoveHighGroundForNon1XPathfinding(point);
            EventBus.Instance.RaiseHighGroundPointRemoved(point);
            Graph.RemoveAllPoints(point.Id);
        }
    }

    private void RemoveHighGroundForNon1XPathfinding(Point removed1XPoint)
    {
        for (var size = 2; size <= MaxSizeForPathfinding; size++)
        {
            foreach (var position in Iterate.Positions(
                         removed1XPoint.Position,
                         removed1XPoint.Position + Vector2.One * size))
            {
                //Graph.RemovePoint();
            }

            for (var x = (int)removed1XPoint.Position.x - size; x <= (int)removed1XPoint.Position.x + size; x++)
            {
                for (var y = (int)removed1XPoint.Position.y - size; y <= (int)removed1XPoint.Position.y + size; y++)
                {
                    var position = new Vector2(x, y);
                    /*if (Graph.ContainsPoint(position, true) is false)
                        continue;

                    pathfinding.RemovePoint(Graph.GetPointId(position, true));*/
                }
            }

            UpdateHighGroundForNon1XPathfinding(removed1XPoint);
        }
    }

    private void UpdateHighGroundForNon1XPathfinding(Point main1XPoint)
    {
        if (DebugEnabled)
            GD.Print($"6. Updating non 1x high ground for main point {JsonConvert.SerializeObject(main1XPoint)}.");

        foreach (var currentPosition in Iterate.Positions(
                     main1XPoint.Position - Vector2.One * MaxSizeForPathfinding,
                     main1XPoint.Position + Vector2.One * MaxSizeForPathfinding + Vector2.One))
        {
            if (DebugEnabled)
                GD.Print($"6.1. Looking at {JsonConvert.SerializeObject(currentPosition)}.");

            if (Graph.ContainsMainPoint(currentPosition, true) is false)
            {
                if (DebugEnabled)
                    GD.Print($"6.2. High ground position {JsonConvert.SerializeObject(currentPosition)} " +
                             $"does not exist.");
                continue;
            }

            var point = Graph.GetMainPoint(currentPosition, true);

            foreach (var size in Graph.SupportedSizes.Skip(1))
            {
                if (DebugEnabled)
                    GD.Print($"6.3. Looking at size {size} with {nameof(AllMainHighGroundPointsAreFilledForSize)}: " +
                             $"{AllMainHighGroundPointsAreFilledForSize(size, currentPosition)}, " +
                             $"{nameof(MainPointHasAdjacentLowGroundConnections)}: {MainPointHasAdjacentLowGroundConnections(point)}.");

                if (AllMainHighGroundPointsAreFilledForSize(size, currentPosition) is false
                    && MainPointHasAdjacentLowGroundConnections(point) is false)
                {
                    if (DebugEnabled)
                        GD.Print($"6.4. Returning {JsonConvert.SerializeObject(currentPosition)}.");
                    continue;
                }

                foreach (var adjacentPosition in Iterate.AdjacentPositions(currentPosition, false))
                {
                    var isDiagonallyAdjacent = currentPosition.IsDiagonalTo(adjacentPosition);

                    if (DebugEnabled)
                        GD.Print($"7. Looking at adjacent position " +
                                 $"{JsonConvert.SerializeObject(adjacentPosition)}.");

                    if (Graph.ContainsMainPoint(adjacentPosition, false))
                    {
                        if (DebugEnabled)
                            GD.Print($"8.1. Adjacent point exists on low ground.");

                        var adjacentLowGroundPoint = Graph.GetMainPoint(adjacentPosition, false);
                        if (Graph.HasMainConnection(point, adjacentLowGroundPoint))
                        {
                            Graph.ConnectPoints(point, adjacentLowGroundPoint, isDiagonallyAdjacent, size);
                            if (DebugEnabled)
                                GD.Print($"8.2. Connecting {size}X pathfinding point " +
                                         $"{JsonConvert.SerializeObject(point)} with low ground point " +
                                         $"{JsonConvert.SerializeObject(adjacentLowGroundPoint)}, " +
                                         $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}.");
                        }
                    }

                    if (Graph.ContainsMainPoint(adjacentPosition, true))
                    {
                        if (DebugEnabled)
                            GD.Print($"9.1. Adjacent point exists on high ground.");

                        var adjacentHighGroundPoint = Graph.GetMainPoint(adjacentPosition, true);
                        if (Graph.HasMainConnection(point, adjacentHighGroundPoint))
                        {
                            Graph.ConnectPoints(point, adjacentHighGroundPoint, isDiagonallyAdjacent, size);
                            if (DebugEnabled)
                                GD.Print($"9.2. Connecting {size}X pathfinding point " +
                                         $"{JsonConvert.SerializeObject(point)} with high ground point " +
                                         $"{JsonConvert.SerializeObject(adjacentHighGroundPoint)}, " +
                                         $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}.");
                        }
                    }
                }
            }
        }
    }

    private bool AllMainHighGroundPointsAreFilledForSize(int size, Vector2 at)
        => Iterate.Positions(at, at + Vector2.One * size)
            .All(position => Graph.ContainsMainPoint(position, true));

    private bool MainPointHasAdjacentLowGroundConnections(Point point)
        => Iterate.AdjacentPositions(point.Position)
            .Where(adjacentPosition => Graph.ContainsMainPoint(adjacentPosition, false))
            .Select(adjacentPosition => Graph.GetMainPoint(adjacentPosition, false))
            .Any(adjacentLowGroundPoint => Graph.HasMainConnection(point, adjacentLowGroundPoint));

    private static IEnumerable<Vector2> GetFlattenedPositions(IList<(IEnumerable<Vector2>, int)> from)
    {
        var flattenedPositions = new HashSet<Vector2>();
        foreach (var step in from)
        {
            foreach (var position in step.Item1)
            {
                flattenedPositions.Add(position);
            }
        }

        return flattenedPositions;
    }

    private Point? GetAdjacentPoint(Point point, Vector2 offset, bool isHighGround)
    {
        var otherPosition = point.Position + offset;
        if (otherPosition.IsInBoundsOf(MapSize) is false)
            return null;

        if (otherPosition.IsEqualApprox(point.Position))
            return null;

        if (Graph.ContainsMainPoint(otherPosition, isHighGround) is false)
            return null;

        var otherPoint = Graph.GetMainPoint(otherPosition, isHighGround);

        if (isHighGround
            && Mathf.Abs(otherPoint.HighGroundAscensionLevel - point.HighGroundAscensionLevel) > HighGroundTolerance)
            return null;

        return otherPoint;
    }
    
    public IList<Point> GetPointsProjectedDownFromEntity(EntityNode entity)
    {
        var isOnHighGround = entity is UnitNode unit && unit.IsOnHighGround;
        const int pathfindingSize = 1;
        var team = 1; // TODO add team

        var points = Iterate
            .Positions(entity.EntityPrimaryPosition, 
                entity.EntityPrimaryPosition + entity.EntitySize)
            .Select(position => Graph.GetHighestPoint(position, team, pathfindingSize, isOnHighGround))
            .Where(point => point != null)
            .Select(point => point.Value)
            .ToList();

        return points;
    }

    private bool IsCached(Vector2 position, float range, bool isOnHighGround, int team, int size)
        => position.Equals(_previousPosition)
           && range.Equals(_previousRange)
           && isOnHighGround.Equals(_previousIsOnHighGround)
           && team.Equals(_previousTeam)
           && size.Equals(_previousSize);

    private void Cache(Vector2 position, float range, bool isOnHighGround, int team, int size)
    {
        _previousPosition = position;
        _previousRange = range;
        _previousIsOnHighGround = isOnHighGround;
        _previousTeam = team;
        _previousSize = size;
    }
}