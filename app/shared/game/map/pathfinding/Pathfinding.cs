using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using low_age_data;
using low_age_data.Domain.Common;
using low_age_data.Domain.Tiles;
using Newtonsoft.Json;
using Object = Godot.Object;

public class Pathfinding : Node
{
	[Export] public bool DebugEnabled = false;
	
	public event Action FinishedInitializing = delegate { };
	
	public const string ScenePath = @"res://app/shared/game/map/pathfinding/Pathfinding.tscn";

	private const int MaxSizeForPathfinding = Constants.Pathfinding.MaxSizeForPathfinding;
	private const float DiagonalCost = Constants.Pathfinding.DiagonalCost;
	private const int HighGroundTolerance = 13; // works until approx 18 levels of ascension
	
	public Graph Graph { get; } = new Graph();
	
	private Vector2 MapSize { get; set; }
    private Blueprint Blueprint { get; set; }
    private Dictionary<Vector2, TileId> Tiles { get; set; } = new Dictionary<Vector2, TileId>();

    private static Godot.Collections.Dictionary<int, float> _terrainWeights = new Godot.Collections.Dictionary<int, float>
    {
        { Terrain.Grass.ToIndex(),     1.0f },
        { Terrain.Mountains.ToIndex(), float.PositiveInfinity },
        { Terrain.Marsh.ToIndex(),     2.0f },
        { ImpassableIndex, float.PositiveInfinity },
        { HighGroundIndex, 1.0f }
    };
    private const int ImpassableIndex = Constants.Pathfinding.ImpassableIndex;
    private const int HighGroundIndex = Constants.Pathfinding.HighGroundIndex;

    private Vector2 _previousPosition = Vector2.Inf;
    private float _previousRange = -1.0f;
    private bool _previousIsOnHighGround = false;
    private int _previousSize = 1;

    private bool _iterateInitialization = false;
    private bool _terrainGraphInitialized = false;
    private IList<(Vector2, TileId)> _tilesForInitialization;
    private bool _terrainDilationInitialized = false;
    private Dictionary<int, IList<Vector2>> _coordinatesByTerrainForInitialization;
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

    public void Initialize(Vector2 mapSize, IEnumerable<(Vector2, TileId)> tiles, bool iterateInitialization = true)
    {
	    MapSize = mapSize;
	    Blueprint = Data.Instance.Blueprint;
	    Graph.Initialize(mapSize);
	    
	    _iterateInitialization = iterateInitialization;
	    
	    var tilesList = tiles.ToList();
	    _tilesForInitialization = tilesList;
	    foreach (var (position, tile) in tilesList) 
		    Tiles.Add(position, tile);
	    
	    InitializePathfindingGraphs();
	    InitializeTerrainWeights();

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
		    if (_terrainGraphInitialized is false)
		    {
			    IterateTerrainGraphUpdate();
			    continue;
		    }

		    if (_terrainDilationInitialized is false)
		    {
			    IterateTerrainDilationUpdate();
			    continue;
		    }
		    
		    if (_diagonalConnectionsInitialized is false)
			    IterateDiagonalConnectionUpdate();
	    }
        
	    _stopwatch.Stop();
    }
    
    private bool CheckInitialization()
    {
	    if (_terrainGraphInitialized is false 
	        || _terrainDilationInitialized is false 
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

    private void InitializeTerrainWeights()
    {
	    _terrainWeights = new Godot.Collections.Dictionary<int, float>();
	    foreach (var tile in Blueprint.Tiles)
	    {
		    _terrainWeights.Add(tile.Terrain.ToIndex(), tile.MovementCost);
	    }
	    _terrainWeights.Add(ImpassableIndex, float.PositiveInfinity);
	    _terrainWeights.Add(HighGroundIndex, 1.0f);
	    
	    _coordinatesByTerrainForInitialization = _terrainWeights
		    .OrderBy(x => x.Value)
		    .ToDictionary<KeyValuePair<int, float>, int, IList<Vector2>>(terrainWeight => 
			    terrainWeight.Key, terrainWeight => new List<Vector2>());
    }
    
    #endregion Initialization

    #region IterationHelpers

    private void IterateTerrainGraphUpdate(KeyValuePair<Vector2, TileId>? point = null, bool useTerrainWeight = true)
    {
	    if (_tilesForInitialization.IsEmpty() && point is null)
	    {
		    if (DebugEnabled)
				GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " + 
				         $"{nameof(Pathfinding)}.{nameof(IterateTerrainGraphUpdate)} completed");
		    _terrainGraphInitialized = true;
		    return;
	    }
	    
	    (Vector2, TileId) entry; 
	    if (point is null) // we are initializing
	    {
		    entry = _tilesForInitialization[0];
		    _tilesForInitialization.RemoveAt(0);
	    }
	    else // we are updating pathfinding during the game
		    entry = ((Vector2)point?.Key, point?.Value);

	    var (coordinates, tileId) = entry;

	    var terrainIndex = useTerrainWeight
		    ? Blueprint.Tiles.Single(x => x.Id.Equals(tileId)).Terrain.ToIndex()
		    : ImpassableIndex;
	    
	    if (point is null)
			_coordinatesByTerrainForInitialization[terrainIndex].Add(coordinates);
	    
	    Graph.SetTerrainForPoint(coordinates, terrainIndex, _terrainWeights[terrainIndex]);
	    Graph.SaveChanges();
    }

    private void IterateTerrainDilationUpdate(KeyValuePair<int, IList<Vector2>>? coordinatesByTerrainId = null)
    {
	    if (_coordinatesByTerrainForInitialization.IsEmpty() && coordinatesByTerrainId is null)
	    {
		    if (DebugEnabled)
				GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " + 
				         $"{nameof(Pathfinding)}.{nameof(IterateTerrainDilationUpdate)} completed");
		    _terrainDilationInitialized = true;
		    return;
	    }
	    
	    KeyValuePair<int, IList<Vector2>> entry;
	    if (coordinatesByTerrainId is null) // we are initializing
	    {
		    entry = _coordinatesByTerrainForInitialization.First();
		    _coordinatesByTerrainForInitialization.Remove(entry.Key);
	    }
	    else // we are updating pathfinding during the game
		    entry = new KeyValuePair<int, IList<Vector2>>((int)coordinatesByTerrainId?.Key, coordinatesByTerrainId?.Value);
	    
	    if (_terrainWeights.ContainsKey(entry.Key) && _terrainWeights[entry.Key].Equals(1f))
		    return;
		    
	    foreach (var coordinate in entry.Value)
	    {
		    if (_pathfinding2X != null)
		    {
			    for (var xOffset = 0; xOffset < 2; xOffset++)
			    {
				    for (var yOffset = 0; yOffset < 2; yOffset++)
				    { 
					    var resultingCoordinates = coordinate - new Vector2(xOffset, yOffset);
					    if (resultingCoordinates.IsInBoundsOf(MapSize) is false) 
						    continue;
					    
					    SetTerrainForPoint(resultingCoordinates, entry.Key, 2);
					    WeightsByPointIds2X[Graph.GetId(resultingCoordinates)] = _terrainWeights[entry.Key];
				    }
			    }
		    }

		    if (_pathfinding3X == null) 
			    continue;
		    
		    for (var xOffset = 0; xOffset < 3; xOffset++)
		    {
			    for (var yOffset = 0; yOffset < 3; yOffset++)
			    {
				    var resultingCoordinates = coordinate - new Vector2(xOffset, yOffset);
				    if (resultingCoordinates.IsInBoundsOf(MapSize) is false)
					    continue;
					    
				    SetTerrainForPoint(resultingCoordinates, entry.Key, 3);
				    WeightsByPointIds3X[Graph.GetId(resultingCoordinates)] = _terrainWeights[entry.Key];
			    }
		    }
	    }
    }
    
    private void IterateDiagonalConnectionUpdate(KeyValuePair<Vector2, int>? point = null)
    {
	    if (_pointIdsByPositionsForInitialization.IsEmpty() && point is null)
	    {
		    if (DebugEnabled)
				GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " + 
				         $"{nameof(Pathfinding)}.{nameof(IterateDiagonalConnectionUpdate)} completed");
		    _diagonalConnectionsInitialized = true;
		    return;
	    }

	    Point entry;
	    if (point is null) // we are initializing
	    {
		    entry = Graph.GetPoint(_pointIdsByPositionsForInitialization.First().Value);
		    _pointIdsByPositionsForInitialization.Remove(entry.Position);
	    }
	    else // we are updating pathfinding during the game
		    entry = Graph.GetPoint(point.Value.Value);
	    
	    IterateDiagonalConnectionUpdate(entry, 1);
	    
	    if (_pathfinding2X != null)
		    IterateDiagonalConnectionUpdate(entry, 2);
	    
	    if (_pathfinding3X != null)
		    IterateDiagonalConnectionUpdate(entry, 3);
    }
    
    private void IterateDiagonalConnectionUpdate(Point point, int size)
    {
	    if (point.IsHighGround)
		    return;
	    
	    var pathfinding = GetPathfindingForSize(size);
	    
	    var diagonalPosition = point.Position + new Vector2(1, 1);
	    if (diagonalPosition.IsInBoundsOf(MapSize) is false)
		    return;
		    
	    var rightNeighbour = Graph.GetId(point.Position + new Vector2(1, 0), false);
	    var bottomNeighbour = Graph.GetId(point.Position + new Vector2(0, 1), false);
	    var diagonalNeighbour = Graph.GetId(diagonalPosition, false);

	    if (IsInfiniteWeight(point.Id, size)
	        && IsInfiniteWeight(diagonalNeighbour, size)
	        && IsInfiniteWeight(rightNeighbour, size) is false
	        && IsInfiniteWeight(bottomNeighbour, size) is false)
	    {
		    pathfinding.RemoveConnection(rightNeighbour, bottomNeighbour);
	    }
	    else
	    {
		    pathfinding.ConnectPoints(rightNeighbour, bottomNeighbour, DiagonalCost);
	    }

	    if (IsInfiniteWeight(point.Id, size) is false
	        && IsInfiniteWeight(diagonalNeighbour, size) is false
	        && IsInfiniteWeight(rightNeighbour, size)
	        && IsInfiniteWeight(bottomNeighbour, size))
	    {
		    pathfinding.RemoveConnection(point.Id, diagonalNeighbour);
	    }
	    else
	    {
		    pathfinding.ConnectPoints(point.Id, diagonalNeighbour, DiagonalCost);
	    }
    }

    #endregion IterationHelpers

    public void ClearCache() => Cache(Vector2.Inf, -1.0f, false, 1);
    
    public IEnumerable<Point> GetAvailablePoints(Vector2 from, float range, bool isOnHighGround, int size, 
	    bool temporary = false)
    {
	    if (size > MaxSizeForPathfinding) throw new ArgumentException($"{nameof(Pathfinding)}." +
	                                                                  $"{nameof(GetAvailablePoints)}: argument " +
	                                                                  $"{nameof(size)} '{size}' exceeded maximum " +
	                                                                  $"value of '{MaxSizeForPathfinding}'.");
	    
	    var pathfinding = GetPathfindingForSize(size);
	    if (pathfinding is null)
		    return Enumerable.Empty<Point>();
	    
	    if (IsCached(from, range, isOnHighGround, size) is false)
	    {
		    pathfinding.Recalculate(
			    Graph.GetId(from, isOnHighGround),
			    new Godot.Collections.Dictionary<string, object>
			    {
				    { "maximum_cost", range },
				    { "terrain_weights", _terrainWeights }
			    });
		    
		    if (temporary is false)
				Cache(from, range, isOnHighGround, size);
	    }

	    var availablePointIds = pathfinding.GetAllPointsWithCostBetween(0.0f, range);
	    var availablePositions = availablePointIds.Select(pointId => Graph.GetPoint(pointId));
	    
	    if (temporary && size == _previousSize 
	                  && Graph.TryGetId(_previousPosition, out var previousPointId, _previousIsOnHighGround))
	    {
		    pathfinding.Recalculate(
			    previousPointId,
			    new Godot.Collections.Dictionary<string, object>
			    {
				    { "maximum_cost", _previousRange },
				    { "terrain_weights", _terrainWeights }
			    });
	    }
	    
	    return availablePositions;
    }

    public IEnumerable<Point> FindPath(Point to, int size)
    {
	    if (to.Position.IsInBoundsOf(MapSize) is false)
	    {
		    return Enumerable.Empty<Point>();
	    }

	    var pathfinding = GetPathfindingForSize(size);
	    if (pathfinding is null)
		    return Enumerable.Empty<Point>();

	    var pathOfPointIds = pathfinding.GetShortestPathFromPoint(to.Id);

	    var path = new List<Point> { Graph.GetPoint(to.Id) };
	    path.AddRange(pathOfPointIds.Select(pointId => Graph.GetPoint(pointId)));
	    path.Reverse();

	    return path;
    }

    public void AddOccupation(EntityNode entity)
    {
	    var foundPoints = new List<Point>();
	    foreach (var point in Graph.GetPointsForEntity(entity))
	    {
		    if (entity.CanBeMovedThroughAt(point)) 
			    continue;
			    
		    foundPoints.Add(point);
		    IterateTerrainGraphUpdate(new KeyValuePair<Vector2, TileId>(point.Position, TileId.Grass), 
			    false);
	    }
	    
	    IterateTerrainDilationUpdate(new KeyValuePair<int, IList<Vector2>>(ImpassableIndex, 
		    foundPoints.Select(x => x.Position).ToList()));

	    foreach (var foundPosition in foundPoints
		             .Where(x => x.IsHighGround is false)
		             .Select(x => x.Position))
	    {
		    for (var x = (int)foundPosition.x - MaxSizeForPathfinding; 
		         x <= (int)foundPosition.x + MaxSizeForPathfinding; x++)
		    {
			    for (var y = (int)foundPosition.y - MaxSizeForPathfinding; 
			         y <= (int)foundPosition.y + MaxSizeForPathfinding; y++)
			    {
				    var position = new Vector2(x, y);
				    if (Graph.ContainsPoint(position) is false)
					    continue;

				    IterateDiagonalConnectionUpdate(new KeyValuePair<Vector2, int>(position, Graph.GetId(position)));
			    }
		    }
	    }
    }

    public void RemoveOccupation(EntityNode entity) // TODO not tested properly
    {
	    var start = entity.EntityPrimaryPosition - Vector2.One * MaxSizeForPathfinding;
	    var end = entity.EntityPrimaryPosition + entity.EntitySize + Vector2.One * MaxSizeForPathfinding;
	    
	    var coordinatesByTerrain = _terrainWeights
		    .OrderBy(x => x.Value)
		    .ToDictionary<KeyValuePair<int, float>, int, IList<Vector2>>(terrainWeight => 
			    terrainWeight.Key, terrainWeight => new List<Vector2>());

	    foreach (var point in Graph.GetPointsForEntity(entity))
	    {
		    if (point.IsHighGround)
			    continue;
		    
		    var position = point.Position;
		    if (position.IsInBoundsOf(entity.EntityPrimaryPosition, 
			        entity.EntityPrimaryPosition + entity.EntitySize))
		    {
			    IterateTerrainGraphUpdate(new KeyValuePair<Vector2, TileId>(position, Tiles[position]));
		    }
			    
		    var terrainIndex = Blueprint.Tiles.Single(t => t.Id.Equals(Tiles[position])).Terrain.ToIndex();
		    coordinatesByTerrain[terrainIndex].Add(position);
	    }

	    foreach (var pair in coordinatesByTerrain) 
		    IterateTerrainDilationUpdate(pair);
	    
	    for (var x = (int)start.x; x < (int)end.x; x++)
	    {
		    for (var y = (int)start.y; y < (int)end.y; y++)
		    {
			    var position = new Vector2(x, y);
			    if (Graph.ContainsPoint(position) is false)
				    continue;
			    
			    IterateDiagonalConnectionUpdate(new KeyValuePair<Vector2, int>(position, Graph.GetId(position)));
		    }
	    }
    }

    public bool HasConnection(Point pointA, Point pointB, int size)
    {
	    var pathfinding = GetPathfindingForSize(size);
	    return pathfinding != null && pathfinding.HasConnection(pointA.Id, pointB.Id);
    }

    public float GetWeight(Point point, int size)
    {
	    var pathfinding = GetPathfindingForSize(size);
	    return pathfinding is null ? float.PositiveInfinity : _terrainWeights[pathfinding.GetTerrainForPoint(point.Id)];
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

			    int pointId;
			    Point point;
			    if (Graph.ContainsPoint(pos, true))
			    {
				    pointId = Graph.GetId(pos, true);
				    point = Graph.GetPoint(pointId);
				    if (DebugEnabled)
						GD.Print($"1. Found existing point {JsonConvert.SerializeObject(point)}");
			    }
			    else
			    {
				    pointId = ++PointIdIterator;
				    _pathfinding.AddPoint(pointId, HighGroundIndex);
				    point = new Point
				    {
					    Id = pointId,
					    Position = pos,
					    IsHighGround = true,
					    HighGroundAscensionLevel = currentAscensionLevel,
					    YSpriteOffset = path[i].Item2
				    };
				    Graph.Add(point);
				    EventBus.Instance.RaiseHighGroundPointCreated(point);
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
						    _pathfinding.ConnectPoints(pointId, ((Point)lowGroundPoint).Id, 
							    isDiagonallyAdjacent ? DiagonalCost : 1F);
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
							    var previousStepPoint = Graph.GetPoint(previousStepPosition, true);
							    _pathfinding.ConnectPoints(pointId, previousStepPoint.Id,
								    isDiagonallyAdjacent ? DiagonalCost : 1F);
							    if (DebugEnabled)
								    GD.Print($"4. Connecting {JsonConvert.SerializeObject(point)} to previous " +
								             $"step point {JsonConvert.SerializeObject(previousStepPoint)}, " +
								             $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}. ");
						    }
					    }
					    
					    var highGroundPoint = GetAdjacentPoint(point, offset, true);
					    if (highGroundPoint is null)
						    continue;

					    _pathfinding.ConnectPoints(pointId, ((Point)highGroundPoint).Id,
						    isDiagonallyAdjacent ? DiagonalCost : 1F);
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
		    
		    if (Graph.ContainsPoint(pos, true))
			    continue;

		    var pointId = ++PointIdIterator;
		    _pathfinding.AddPoint(pointId, HighGroundIndex);
		    var point = new Point
		    {
			    Id = pointId,
			    Position = pos,
			    IsHighGround = true,
			    HighGroundAscensionLevel = 100,
			    YSpriteOffset = path.First().Item2
		    };
		    Graph.Add(point);
		    EventBus.Instance.RaiseHighGroundPointCreated(point);
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

				    _pathfinding.ConnectPoints(pointId, ((Point)otherPoint).Id, 
					    isDiagonallyAdjacent ? DiagonalCost : 1F);
				    if (DebugEnabled)
					    GD.Print($"5. Connecting {JsonConvert.SerializeObject(point)} to adjacent high " +
					             $"ground point {JsonConvert.SerializeObject(otherPoint)}, " +
					             $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}. ");
			    }
		    }

		    UpdateHighGroundForNon1XPathfinding(point);
	    }
    }

    private void RemoveHighGround(IList<(IEnumerable<Vector2>, int)> path)  // TODO not tested properly
    {
	    var flattenedPositions = GetFlattenedPositions(path);
	    
	    foreach (var pos in flattenedPositions)
	    {
		    if (pos.IsInBoundsOf(MapSize) is false)
			    continue;
		    
		    if (Graph.ContainsPoint(pos, true) is false)
			    continue;

		    var point = Graph.GetPoint(pos, true);
		    
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
		    _pathfinding.RemovePoint(point.Id);
		    EventBus.Instance.RaiseHighGroundPointRemoved(point);
		    Graph.Remove(point.Id);
	    }
    }

    private void RemoveHighGroundForNon1XPathfinding(Point removed1XPoint)
    {
	    for (var size = 2; size <= MaxSizeForPathfinding; size++)
	    {
		    var pathfinding = GetPathfindingForSize(size);
		    if (pathfinding is null)
			    continue;
		    
		    var result = pathfinding.RemovePoint(removed1XPoint.Id);
		    if (result != Error.Ok) 
			    continue;

		    for (var x = (int)removed1XPoint.Position.x - size; x <= (int)removed1XPoint.Position.x + size; x++)
		    {
			    for (var y = (int)removed1XPoint.Position.y - size; y <= (int)removed1XPoint.Position.y + size; y++)
			    {
				    var position = new Vector2(x, y);
				    if (Graph.ContainsPoint(position, true) is false)
					    continue;

				    pathfinding.RemovePoint(Graph.GetId(position, true));
			    }
		    }
		    
		    UpdateHighGroundForNon1XPathfinding(removed1XPoint);
	    }
    }
    
    private void UpdateHighGroundForNon1XPathfinding(Point main1XPoint)
    {
	    if (DebugEnabled)
		    GD.Print($"6. Updating non 1x high ground for main point {JsonConvert.SerializeObject(main1XPoint)}.");
	    for (var x = (int)main1XPoint.Position.x - MaxSizeForPathfinding; 
	         x <= (int)main1XPoint.Position.x + MaxSizeForPathfinding; 
	         x++)
	    {
		    for (var y = (int)main1XPoint.Position.y - MaxSizeForPathfinding; 
		         y <= (int)main1XPoint.Position.y + MaxSizeForPathfinding; 
		         y++)
		    {
			    var currentPosition = new Vector2(x, y);
			    if (DebugEnabled)
				    GD.Print($"6.1. Looking at {JsonConvert.SerializeObject(currentPosition)}.");

			    if (Graph.ContainsPoint(currentPosition, true) is false)
			    {
				    if (DebugEnabled)
					    GD.Print($"6.2. High ground position {JsonConvert.SerializeObject(currentPosition)} " +
					             $"does not exist.");
				    continue;
			    }
				    
			    var point = Graph.GetPoint(currentPosition, true);

			    for (var size = 2; size <= MaxSizeForPathfinding; size++)
			    {
				    if (DebugEnabled)
					    GD.Print($"6.3. Looking at size {size} with {nameof(AllHighGroundPointsAreFilledForSize)}: " +
				                              $"{AllHighGroundPointsAreFilledForSize(size, currentPosition)}, " +
				                              $"{nameof(PointHasAdjacentLowGroundConnections)}: {PointHasAdjacentLowGroundConnections(point)}.");

				    if (AllHighGroundPointsAreFilledForSize(size, currentPosition) is false
				        && PointHasAdjacentLowGroundConnections(point) is false)
				    {
					    if (DebugEnabled)
						    GD.Print($"6.4. Returning {JsonConvert.SerializeObject(currentPosition)}.");
					    continue;
				    }

				    var pathfinding = GetPathfindingForSize(size);
				    if (pathfinding is null)
				    {
					    if (DebugEnabled)
						    GD.Print($"6.5. {size}x pathfinding is null.");
					    continue;
				    }

				    if (pathfinding.HasPoint(point.Id) is false)
				    {
					    var result = pathfinding.AddPoint(point.Id, HighGroundIndex);
					    if (DebugEnabled)
						    GD.Print($"6.6. Adding point to {size}X pathfinding " +
						             $"{JsonConvert.SerializeObject(point)} with result: {result}.");
				    }
				    
				    for (var xAdjacent = x - 1; xAdjacent <= x + 1; xAdjacent++)
				    {
					    for (var yAdjacent = y - 1; yAdjacent <= y + 1; yAdjacent++)
					    {
						    var adjacentPosition = new Vector2(xAdjacent, yAdjacent);
						    var isDiagonallyAdjacent = currentPosition.IsDiagonalTo(adjacentPosition);
						    
						    if (DebugEnabled)
							    GD.Print($"7. Looking at adjacent position " +
							             $"{JsonConvert.SerializeObject(adjacentPosition)}.");

						    if (Graph.ContainsPoint(adjacentPosition, false))
						    {
							    if (DebugEnabled)
								    GD.Print($"8.1. Adjacent point exists on low ground.");

							    var adjacentLowGroundPoint = Graph.GetPoint(adjacentPosition, false);
							    if (_pathfinding.HasConnection(point.Id, adjacentLowGroundPoint.Id))
							    {
								    var result = pathfinding.ConnectPoints(point.Id, adjacentLowGroundPoint.Id,
									    isDiagonallyAdjacent ? DiagonalCost : 1F);
								    if (DebugEnabled)
									    GD.Print($"8.2. Connecting {size}X pathfinding point " +
									             $"{JsonConvert.SerializeObject(point)} with low ground point " +
									             $"{JsonConvert.SerializeObject(adjacentLowGroundPoint)}, " +
									             $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}, " +
									             $"result: {result}.");
							    }
						    }

						    if (Graph.ContainsPoint(adjacentPosition, true))
						    {
							    if (DebugEnabled)
								    GD.Print($"9.1. Adjacent point exists on high ground.");
							    
							    var adjacentHighGroundPoint = Graph.GetPoint(adjacentPosition, true);
							    if (_pathfinding.HasConnection(point.Id, adjacentHighGroundPoint.Id))
							    {
								    var result = pathfinding.ConnectPoints(point.Id, adjacentHighGroundPoint.Id,
									    isDiagonallyAdjacent ? DiagonalCost : 1F);
								    if (DebugEnabled)
									    GD.Print($"9.2. Connecting {size}X pathfinding point " +
									             $"{JsonConvert.SerializeObject(point)} with high ground point " +
									             $"{JsonConvert.SerializeObject(adjacentHighGroundPoint)}, " +
									             $"{nameof(isDiagonallyAdjacent)}: {isDiagonallyAdjacent}, " +
									             $"result: {result}.");
							    }
						    }
					    }
				    } 
			    }
		    }
	    }
    }

    private bool AllHighGroundPointsAreFilledForSize(int size, Vector2 at)
    {
	    for (var x = (int)at.x; x < (int)at.x + size; x++) 
	    for (var y = (int)at.y; y < (int)at.y + size; y++)
		    if (Graph.ContainsPoint(new Vector2(x, y), true) is false)
		    {
			    
			    return false;
		    }

	    return true;
    }

    private bool PointHasAdjacentLowGroundConnections(Point point)
    {
	    for (var x = (int)point.Position.x - 1; x <= (int)point.Position.x + 1; x++)
	    {
		    for (var y = (int)point.Position.y - 1; y <= (int)point.Position.y + 1; y++)
		    {
			    var position = new Vector2(x, y);
			    if (Graph.ContainsPoint(position, false) is false)
				    continue;

			    var lowGroundPoint = Graph.GetPoint(position, false);
			    if (_pathfinding.HasConnection(point.Id, lowGroundPoint.Id))
				    return true;
		    }
	    }

	    return false;
    }

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
	    
	    if (Graph.ContainsPoint(otherPosition, isHighGround) is false)
		    return null;
	    
	    var otherPoint = Graph.GetPoint(otherPosition, isHighGround);

	    if (isHighGround 
	        && Mathf.Abs(otherPoint.HighGroundAscensionLevel - point.HighGroundAscensionLevel) > HighGroundTolerance)
		    return null;

	    return otherPoint;
    }
    
    private void SetTerrainForPoint(Vector2 at, int terrain, int size)
    {
	    if (at.IsInBoundsOf(MapSize))
	    {
		    GetPathfindingForSize(size).SetTerrainForPoint(
			    Graph.GetId(at),
			    terrain);
	    }
    }

    private bool IsInfiniteWeight(int pointId, int size) => GetWeightsByPointIds(size)[pointId]
	    .Equals(float.PositiveInfinity);

    private Godot.Collections.Dictionary<int, float> GetWeightsByPointIds(int size)
    {
	    switch (size)
	    {
		    case 3:
			    return WeightsByPointIds3X;
		    case 2:
			    return WeightsByPointIds2X;
		    case 1:
		    default:
			    return WeightsByPointIds;
	    }
    }

    private DijkstraMap GetPathfindingForSize(int size)
    {
	    switch (size)
	    {
		    case 3:
			    return _pathfinding3X;
		    case 2:
			    return _pathfinding2X;
		    case 1:
		    default:
			    return _pathfinding;
	    }
    }

    private bool IsCached(Vector2 position, float range, bool isOnHighGround, int size) 
	    => position.Equals(_previousPosition) 
	       && range.Equals(_previousRange)
	       && isOnHighGround.Equals(_previousIsOnHighGround) 
	       && size.Equals(_previousSize);

    private void Cache(Vector2 position, float range, bool isOnHighGround, int size)
    {
	    _previousPosition = position;
	    _previousRange = range;
	    _previousIsOnHighGround = isOnHighGround;
	    _previousSize = size;
    }
}