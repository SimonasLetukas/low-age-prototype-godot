using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using LowAgeData;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Tiles;
using Object = Godot.GodotObject;

public partial class Pathfinding : Node
{
	public event Action FinishedInitializing = delegate { };

	private const float DiagonalCost = Mathf.Sqrt2;

	private Vector2 MapSize { get; set; }
    private Blueprint Blueprint { get; set; }
    private Dictionary<Vector2, TileId> Tiles { get; set; } = new Dictionary<Vector2, TileId>();
    private Godot.Collections.Dictionary<Vector2, int> PointIdsByPositions { get; set; }
    private Godot.Collections.Dictionary<int, Vector2> PositionsByPointIds { get; set; }
    private Godot.Collections.Dictionary<int, float> WeightsByPointIds { get; } =
	    new Godot.Collections.Dictionary<int, float>();
    private Godot.Collections.Dictionary<int, float> WeightsByPointIds2X { get; } =
	    new Godot.Collections.Dictionary<int, float>();
    private Godot.Collections.Dictionary<int, float> WeightsByPointIds3X { get; } =
	    new Godot.Collections.Dictionary<int, float>();

    private static Godot.Collections.Dictionary<int, float> _terrainWeights = new Godot.Collections.Dictionary<int, float>
    {
        { Terrain.Grass.ToIndex(),     1.0f },
        { Terrain.Mountains.ToIndex(), float.PositiveInfinity },
        { Terrain.Marsh.ToIndex(),     2.0f },
        { ImpassableIndex, float.PositiveInfinity }
    };
    private const int ImpassableIndex = -1;
    
    // Documentation: https://github.com/MatejSloboda/Dijkstra_map_for_Godot/blob/master/addons/dijkstra-map/doc/DijkstraMap.md
    private DijkstraMap _pathfinding = new DijkstraMap();
    private DijkstraMap _pathfinding2X = new DijkstraMap();
    private DijkstraMap _pathfinding3X = new DijkstraMap();

    private Vector2 _previousPosition = Vector2.Inf;
    private float _previousRange = -1.0f;
    private int _previousSize = 1;

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
	    ProcessMode = ProcessModeEnum.Always;
    }

    #region Initialization

    public void Initialize(Vector2 mapSize, IEnumerable<(Vector2, TileId)> tiles)
    {
	    MapSize = mapSize;
	    Blueprint = Data.Instance.Blueprint;
	    
	    var tilesList = tiles.ToList();
	    _tilesForInitialization = tilesList;
	    foreach (var (position, tile) in tilesList) 
		    Tiles.Add(position, tile);
	    
	    InitializePathfindingGraphs();
	    InitializePositionToPointIdReferences();
	    InitializeTerrainWeights();

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
    
    private void CheckInitialization()
    {
	    if (_terrainGraphInitialized is false 
	        || _terrainDilationInitialized is false 
	        || _diagonalConnectionsInitialized is false)
		    return;

	    _initialized = true;
	    FinishedInitializing();
    }
    
    private void InitializePathfindingGraphs()
    {
	    _pathfinding = new DijkstraMap();
	    _pathfinding2X = Blueprint.Entities.Units.Any(u => u.Size == 2) ? new DijkstraMap() : null;
	    _pathfinding3X = Blueprint.Entities.Units.Any(u => u.Size == 3) ? new DijkstraMap() : null;
	    
	    PointIdsByPositions = _pathfinding.AddSquareGrid(
		    new Rect2(0, 0, MapSize.X, MapSize.Y),
		    Terrain.Grass.ToIndex(),
		    1.0f,
		    DiagonalCost);
	    _pathfinding2X?.DuplicateGraphFrom(_pathfinding);
	    _pathfinding3X?.DuplicateGraphFrom(_pathfinding);

	    _pointIdsByPositionsForInitialization = new Dictionary<Vector2, int>();
	    foreach (var entry in PointIdsByPositions)
	    {
		    _pointIdsByPositionsForInitialization.Add(entry.Key, entry.Value);
	    }
    }
    
    private void InitializePositionToPointIdReferences()
    {
	    PositionsByPointIds = new Godot.Collections.Dictionary<int, Vector2>();
	    foreach (var position in PointIdsByPositions.Keys)
	    {
		    var id = PointIdsByPositions[position];
		    PositionsByPointIds[id] = position;
	    }
    }

    private void InitializeTerrainWeights()
    {
	    _terrainWeights = new Godot.Collections.Dictionary<int, float>();
	    foreach (var tile in Blueprint.Tiles)
	    {
		    _terrainWeights.Add(tile.Terrain.ToIndex(), tile.MovementCost);
	    }
	    _terrainWeights.Add(ImpassableIndex, float.PositiveInfinity);
	    
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
	    
	    SetTerrainForPoint(coordinates, terrainIndex, 1);
	    WeightsByPointIds[PointIdsByPositions[coordinates]] = _terrainWeights[terrainIndex];
	    
	    if (_pathfinding2X != null)
		    WeightsByPointIds2X[PointIdsByPositions[coordinates]] = _terrainWeights[terrainIndex];
	    
	    if (_pathfinding3X != null)
		    WeightsByPointIds3X[PointIdsByPositions[coordinates]] = _terrainWeights[terrainIndex];
    }

    private void IterateTerrainDilationUpdate(KeyValuePair<int, IList<Vector2>>? coordinatesByTerrainId = null)
    {
	    if (_coordinatesByTerrainForInitialization.IsEmpty() && coordinatesByTerrainId is null)
	    {
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
					    WeightsByPointIds2X[PointIdsByPositions[resultingCoordinates]] = _terrainWeights[entry.Key];
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
				    WeightsByPointIds3X[PointIdsByPositions[resultingCoordinates]] = _terrainWeights[entry.Key];
			    }
		    }
	    }
    }
    
    private void IterateDiagonalConnectionUpdate(KeyValuePair<Vector2, int>? point = null)
    {
	    if (_pointIdsByPositionsForInitialization.IsEmpty() && point is null)
	    {
		    GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
		             $"{nameof(Pathfinding)}.{nameof(IterateDiagonalConnectionUpdate)} completed");
		    _diagonalConnectionsInitialized = true;
		    return;
	    }

	    KeyValuePair<Vector2, int> entry;
	    if (point is null) // we are initializing
	    {
		    entry = _pointIdsByPositionsForInitialization.First();
		    _pointIdsByPositionsForInitialization.Remove(entry.Key);
	    }
	    else // we are updating pathfinding during the game
		    entry = new KeyValuePair<Vector2, int>((Vector2)point?.Key, (int)point?.Value);
	    
	    IterateDiagonalConnectionUpdate(entry, 1);
	    
	    if (_pathfinding2X != null)
		    IterateDiagonalConnectionUpdate(entry, 2);
	    
	    if (_pathfinding3X != null)
		    IterateDiagonalConnectionUpdate(entry, 3);
    }
    
    private void IterateDiagonalConnectionUpdate(KeyValuePair<Vector2, int> point, int size)
    {
	    var pathfinding = GetPathfindingForSize(size);
	    
	    var diagonalPosition = point.Key + new Vector2(1, 1);
	    if (diagonalPosition.IsInBoundsOf(MapSize) is false)
		    return;
		    
	    var rightNeighbour = PointIdsByPositions[point.Key + new Vector2(1, 0)];
	    var bottomNeighbour = PointIdsByPositions[point.Key + new Vector2(0, 1)];
	    var diagonalNeighbour = PointIdsByPositions[diagonalPosition];

	    if (IsInfiniteWeight(point.Value, size)
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

	    if (IsInfiniteWeight(point.Value, size) is false
	        && IsInfiniteWeight(diagonalNeighbour, size) is false
	        && IsInfiniteWeight(rightNeighbour, size)
	        && IsInfiniteWeight(bottomNeighbour, size))
	    {
		    pathfinding.RemoveConnection(point.Value, diagonalNeighbour);
	    }
	    else
	    {
		    pathfinding.ConnectPoints(point.Value, diagonalNeighbour, DiagonalCost);
	    }
    }

    #endregion IterationHelpers

    public void ClearCache() => Cache(Vector2.Inf, -1.0f, 1);
    
    public IEnumerable<Vector2> GetAvailablePositions(Vector2 from, float range, int size, bool temporary = false)
    {
	    if (size > 3) throw new ArgumentException($"{nameof(Pathfinding)}.{nameof(GetAvailablePositions)}: " +
	                                              $"argument {nameof(size)} '{size}' exceeded maximum value of '3'.");
	    
	    var pathfinding = GetPathfindingForSize(size);
	    if (pathfinding is null)
		    return Enumerable.Empty<Vector2>();
	    
	    if (IsCached(from, range, size) is false)
	    {
		    pathfinding.Recalculate(
			    PointIdsByPositions[from],
			    new Godot.Collections.Dictionary<string, Variant>
			    {
				    { "maximum_cost", range },
				    { "terrain_weights", _terrainWeights }
			    });
		    
		    if (temporary is false)
				Cache(from, range, size);
	    }

	    var availablePointIds = pathfinding.GetAllPointsWithCostBetween(0.0f, range);
	    var availablePositions = availablePointIds.Select(pointId => PositionsByPointIds[pointId]);
	    
	    if (temporary && size == _previousSize 
	                  && PointIdsByPositions.TryGetValue(_previousPosition, out var previousPointId))
	    {
		    pathfinding.Recalculate(
			    previousPointId,
			    new Godot.Collections.Dictionary<string, Variant>
			    {
				    { "maximum_cost", _previousRange },
				    { "terrain_weights", _terrainWeights }
			    });
	    }
	    
	    return availablePositions;
    }

    public IEnumerable<Vector2> FindPath(Vector2 to, int size)
    {
	    if (to.IsInBoundsOf(MapSize) is false)
	    {
		    return Enumerable.Empty<Vector2>();
	    }

	    var pathfinding = GetPathfindingForSize(size);
	    if (pathfinding is null)
		    return Enumerable.Empty<Vector2>();

	    var targetPointId = PointIdsByPositions[to];
	    var pathOfPointIds = pathfinding.GetShortestPathFromPoint(targetPointId);

	    var path = new List<Vector2> { to };
	    path.AddRange(pathOfPointIds.Select(pointId => PositionsByPointIds[pointId]));
	    path.Reverse();

	    return path;
    }

    public void AddOccupation(EntityNode entity)
    {
	    const int offset = 3;
	    var foundPoints = new List<Vector2>();
	    
	    for (var x = (int)entity.EntityPrimaryPosition.X - offset; 
	         x < entity.EntityPrimaryPosition.X + entity.EntitySize.X + offset; 
	         x++)
	    {
		    for (var y = (int)entity.EntityPrimaryPosition.Y - offset;
		         y < entity.EntityPrimaryPosition.Y + entity.EntitySize.Y + offset;
		         y++)
		    {
			    var position = new Vector2(x, y);
			    if (entity.CanBeMovedThroughAt(position)) 
				    continue;
			    
			    foundPoints.Add(position);
			    IterateTerrainGraphUpdate(new KeyValuePair<Vector2, TileId>(position, TileId.Grass), false);
		    }
	    }
	    
	    IterateTerrainDilationUpdate(new KeyValuePair<int, IList<Vector2>>(ImpassableIndex, foundPoints));

	    foreach (var point in foundPoints)
	    {
		    for (var x = (int)point.X - offset; x <= (int)point.X + offset; x++)
		    {
			    for (var y = (int)point.Y - offset; y <= (int)point.Y + offset; y++)
			    {
				    var position = new Vector2(x, y);
				    if (PointIdsByPositions.ContainsKey(position) is false)
					    continue;
				    
				    IterateDiagonalConnectionUpdate(new KeyValuePair<Vector2, int>(position, 
					    PointIdsByPositions[position]));
			    }
		    }
	    }
    }

    public void RemoveOccupation(EntityNode entity) // TODO not tested properly
    {
	    const int offset = 3;
	    var start = entity.EntityPrimaryPosition - Vector2.One * offset;
	    var end = entity.EntityPrimaryPosition + entity.EntitySize + Vector2.One * offset;
	    
	    var coordinatesByTerrain = _terrainWeights
		    .OrderBy(x => x.Value)
		    .ToDictionary<KeyValuePair<int, float>, int, IList<Vector2>>(terrainWeight => 
			    terrainWeight.Key, terrainWeight => new List<Vector2>());

	    for (var x = (int)start.X; x < (int)end.X; x++)
	    {
		    for (var y = (int)start.Y; y < (int)end.Y; y++)
		    {
			    var position = new Vector2(x, y);
			    if (Tiles.ContainsKey(position) is false)
				    continue;
			    
			    if (position.IsInBoundsOf(entity.EntityPrimaryPosition, 
				        entity.EntityPrimaryPosition + entity.EntitySize))
					IterateTerrainGraphUpdate(new KeyValuePair<Vector2, TileId>(position, Tiles[position]));
			    
			    var terrainIndex = Blueprint.Tiles.Single(t => t.Id.Equals(Tiles[position])).Terrain.ToIndex();
			    coordinatesByTerrain[terrainIndex].Add(position);
		    }
	    }

	    foreach (var pair in coordinatesByTerrain) 
		    IterateTerrainDilationUpdate(pair);
	    
	    for (var x = (int)start.X; x < (int)end.X; x++)
	    {
		    for (var y = (int)start.Y; y < (int)end.Y; y++)
		    {
			    var position = new Vector2(x, y);
			    if (PointIdsByPositions.ContainsKey(position) is false)
				    continue;
			    
			    IterateDiagonalConnectionUpdate(new KeyValuePair<Vector2, int>(position, 
				    PointIdsByPositions[position]));
		    }
	    }
    }
    
    private void SetTerrainForPoint(Vector2 at, int terrain, int size)
    {
	    if (at.IsInBoundsOf(MapSize))
	    {
		    GetPathfindingForSize(size).SetTerrainForPoint(
			    PointIdsByPositions[at],
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

    private bool IsCached(Vector2 position, float range, int size) 
	    => position.Equals(_previousPosition) && range.Equals(_previousRange) && size.Equals(_previousSize);

    private void Cache(Vector2 position, float range, int size)
    {
	    _previousPosition = position;
	    _previousRange = range;
	    _previousSize = size;
    }
}

public partial class DijkstraMap : Node
{
	private Object _dijkstraMap; 

	public DijkstraMap()
	{
		_dijkstraMap = GD.Load("res://addons/dijkstra-map/Dijkstra_map_library/nativescript.gdns") as Script;
		//_dijkstraMap = dijkstraMapScript?.New() as Object;
		if (_dijkstraMap is null) throw new ArgumentNullException($"{nameof(_dijkstraMap)} cannot be null.");
	}
	
	public Godot.Collections.Dictionary<Vector2, int> AddSquareGrid(Rect2 bounds, int terrain, float orthCost,
		float diagCost)
	{
		var dictionary = _dijkstraMap.Call("add_square_grid", bounds, terrain, orthCost, diagCost);
		return new Godot.Collections.Dictionary<Vector2, int>();
	}

	public void SetTerrainForPoint(int pointId, int terrain)
	{
		_dijkstraMap.Call("set_terrain_for_point", pointId, terrain);
	}
	
	public int GetTerrainForPoint(int pointId)
	{
		return (int)_dijkstraMap.Call("get_terrain_for_point", pointId);
	}

	public void Recalculate(int pointId, Godot.Collections.Dictionary<string, Variant> options)
	{
		_dijkstraMap.Call("recalculate", pointId, options);
	}

	public int[] GetAllPointsWithCostBetween(float min, float max)
	{
		return new[] { 0 }; _dijkstraMap.Call("get_all_points_with_cost_between", min, max);
	}

	public int[] GetShortestPathFromPoint(int pointId)
	{
		return new[] { 0 }; _dijkstraMap.Call("get_shortest_path_from_point", pointId);
	}

	public void Clear()
    {
        _dijkstraMap.Call("clear");
    }

    public Error DuplicateGraphFrom(DijkstraMap sourceInstance)
    {
	    return Error.Ok; _dijkstraMap.Call("duplicate_graph_from", sourceInstance._dijkstraMap);
    }

    public int GetAvailablePointId()
    {
        return (int)_dijkstraMap.Call("get_available_point_id");
    }

    public Error AddPoint(int pointId, int terrainType = -1)
    {
        return Error.Ok; _dijkstraMap.Call("add_point", pointId, terrainType);
    }

    public Error RemovePoint(int pointId)
    {
        return Error.Ok; _dijkstraMap.Call("remove_point", pointId);
    }

    public bool HasPoint(int pointId)
    {
        return (bool)_dijkstraMap.Call("has_point", pointId);
    }
    
    public Error DisablePoint(int pointId)
    {
        return Error.Ok; _dijkstraMap.Call("disable_point", pointId);
    }
    
    public Error EnablePoint(int pointId)
    {
        return Error.Ok; _dijkstraMap.Call("enable_point", pointId);
    }
    
    public bool IsPointDisabled(int pointId)
    {
        return (bool)_dijkstraMap.Call("is_point_disabled", pointId);
    }
    
    public Error ConnectPoints(int source, int target, float weight = 1f, bool bidirectional = true)
    {
        return Error.Ok; _dijkstraMap.Call("connect_points", source, target, weight, bidirectional);
    }
    
    public Error RemoveConnection(int source, int target, bool bidirectional = true)
    {
        return Error.Ok; _dijkstraMap.Call("remove_connection", source, target, bidirectional);
    }
    
    public bool HasConnection(int source, int target)
    {
        return (bool)_dijkstraMap.Call("has_connection", source, target);
    }

    public int GetDirectionAtPoint(int pointId)
    {
        return (int)_dijkstraMap.Call("get_direction_at_point", pointId);
    }
    
    public float GetCostAtPoint(int pointId)
    {
        return (float)_dijkstraMap.Call("get_cost_at_point", pointId);
    }
}