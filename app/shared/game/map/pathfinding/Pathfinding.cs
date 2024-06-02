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

public struct Point
{
	public int Id { get; set; }
	public Vector2 Position { get; set; }
	public bool IsHighGround { get; set; }
	public int HighGroundAscensionLevel { get; set; }
	public int YSpriteOffset { get; set; }
}

public class Pathfinding : Node
{
	public class PointContainer
	{
		public Dictionary<Vector2, int> NonHighGroundIdsByPosition { get; set; } = new Dictionary<Vector2, int>();
		private Dictionary<int, Point> PointsByIds { get; set; } = new Dictionary<int, Point>();
		private Dictionary<(Vector2, bool), int> IdsByPositionAndLevel { get; set; } = new Dictionary<(Vector2, bool), int>();

		public void SetBase(Godot.Collections.Dictionary<Vector2, int> graph)
		{
			foreach (var entry in graph)
			{
				var point = new Point
				{
					Id = entry.Value,
					Position = entry.Key,
					IsHighGround = false,
					HighGroundAscensionLevel = 0
				};
				
				PointsByIds[entry.Value] = point;
				IdsByPositionAndLevel[(entry.Key, false)] = entry.Value;
				NonHighGroundIdsByPosition[entry.Key] = entry.Value;
			}
		}

		public IEnumerable<Point> GetAll() => PointsByIds.Values;

		public Point GetPoint(int id) => PointsByIds[id];

		public Point GetPoint(Vector2 position, bool isHighGround = false) => GetPoint(GetId(position, isHighGround));

		public bool ContainsPoint(Vector2 position, bool isHighGround = false)
			=> IdsByPositionAndLevel.ContainsKey((position, isHighGround));

		public bool TryGetId(Vector2 position, out int id, bool isHighGround = false)
		{
			var result = IdsByPositionAndLevel.TryGetValue((position, isHighGround), out var idResult);
			id = idResult;
			return result;
		}
		
		public int GetId(Point point) => GetId(point.Position, point.IsHighGround);
		
		public int GetId(Vector2 position, bool isHighGround = false) 
			=> IdsByPositionAndLevel[(position, isHighGround)];

		public void Add(Point point)
		{
			PointsByIds[point.Id] = point;
			IdsByPositionAndLevel[(point.Position, point.IsHighGround)] = point.Id;
		}

		public void Remove(int id)
		{
			var point = PointsByIds[id];
			
			PointsByIds.Remove(id);
			IdsByPositionAndLevel.Remove((point.Position, point.IsHighGround));
		}
	}
	
	public event Action FinishedInitializing = delegate { };

	private const float DiagonalCost = Mathf.Sqrt2;
	private const int HighGroundTolerance = 13; // works until approx 18 levels of ascension
	
	public PointContainer Points { get; } = new PointContainer();
	
	private Vector2 MapSize { get; set; }
    private Blueprint Blueprint { get; set; }
    private Dictionary<Vector2, TileId> Tiles { get; set; } = new Dictionary<Vector2, TileId>();
    private int PointIdIterator { get; set; } = 0;
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
        { ImpassableIndex, float.PositiveInfinity },
        { HighGroundIndex, 1.0f }
    };
    private const int ImpassableIndex = -1;
    private const int HighGroundIndex = -2;
    
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

    public void Initialize(Vector2 mapSize, IEnumerable<(Vector2, TileId)> tiles)
    {
	    MapSize = mapSize;
	    Blueprint = Data.Instance.Blueprint;
	    
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

	    PointIdIterator = (int)MapSize.x * (int)MapSize.y;
	    Points.SetBase(_pathfinding.AddSquareGrid(
		    new Rect2(0, 0, MapSize.x, MapSize.y),
		    Terrain.Grass.ToIndex(),
		    1.0f,
		    DiagonalCost));
	    _pathfinding2X?.DuplicateGraphFrom(_pathfinding);
	    _pathfinding3X?.DuplicateGraphFrom(_pathfinding);

	    _pointIdsByPositionsForInitialization = Points.NonHighGroundIdsByPosition;
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
	    WeightsByPointIds[Points.GetId(coordinates)] = _terrainWeights[terrainIndex];
	    
	    if (_pathfinding2X != null)
		    WeightsByPointIds2X[Points.GetId(coordinates)] = _terrainWeights[terrainIndex];
	    
	    if (_pathfinding3X != null)
		    WeightsByPointIds3X[Points.GetId(coordinates)] = _terrainWeights[terrainIndex];
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
					    WeightsByPointIds2X[Points.GetId(resultingCoordinates)] = _terrainWeights[entry.Key];
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
				    WeightsByPointIds3X[Points.GetId(resultingCoordinates)] = _terrainWeights[entry.Key];
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
		    
	    var rightNeighbour = Points.GetId(point.Key + new Vector2(1, 0));
	    var bottomNeighbour = Points.GetId(point.Key + new Vector2(0, 1));
	    var diagonalNeighbour = Points.GetId(diagonalPosition);

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
    
    public IEnumerable<Point> GetAvailablePoints(Vector2 from, float range, bool isOnHighGround, int size, 
	    bool temporary = false)
    {
	    if (size > 3) throw new ArgumentException($"{nameof(Pathfinding)}.{nameof(GetAvailablePoints)}: " +
	                                              $"argument {nameof(size)} '{size}' exceeded maximum value of '3'.");
	    
	    var pathfinding = GetPathfindingForSize(size);
	    if (pathfinding is null)
		    return Enumerable.Empty<Point>();
	    
	    if (IsCached(from, range, size) is false)
	    {
		    pathfinding.Recalculate(
			    Points.GetId(from, isOnHighGround),
			    new Godot.Collections.Dictionary<string, object>
			    {
				    { "maximum_cost", range },
				    { "terrain_weights", _terrainWeights }
			    });
		    
		    if (temporary is false)
				Cache(from, range, size);
	    }

	    var availablePointIds = pathfinding.GetAllPointsWithCostBetween(0.0f, range);
	    var availablePositions = availablePointIds.Select(pointId => Points.GetPoint(pointId));
	    
	    if (temporary && size == _previousSize 
	                  && Points.TryGetId(_previousPosition, out var previousPointId, isOnHighGround))
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

    public IEnumerable<Point> FindPath(Vector2 to, int size, bool isHighGround)
    {
	    if (to.IsInBoundsOf(MapSize) is false)
	    {
		    return Enumerable.Empty<Point>();
	    }

	    var pathfinding = GetPathfindingForSize(size);
	    if (pathfinding is null)
		    return Enumerable.Empty<Point>();

	    var targetPointId = Points.GetId(to, isHighGround);
	    var pathOfPointIds = pathfinding.GetShortestPathFromPoint(targetPointId);

	    var path = new List<Point> { Points.GetPoint(targetPointId) };
	    path.AddRange(pathOfPointIds.Select(pointId => Points.GetPoint(pointId)));
	    path.Reverse();

	    return path;
    }

    public void AddOccupation(EntityNode entity)
    {
	    const int offset = 3; // Depends on maximum possible size
	    var foundPoints = new List<Point>();
	    
	    for (var x = (int)entity.EntityPrimaryPosition.x - offset; 
	         x < entity.EntityPrimaryPosition.x + entity.EntitySize.x + offset; 
	         x++)
	    {
		    for (var y = (int)entity.EntityPrimaryPosition.y - offset;
		         y < entity.EntityPrimaryPosition.y + entity.EntitySize.y + offset;
		         y++)
		    {
			    var pointFound = Points.TryGetId(new Vector2(x, y), out var pointId);
			    if (pointFound is false)
				    continue;

			    var point = Points.GetPoint(pointId);
			    if (entity.CanBeMovedThroughAt(point)) 
				    continue;
			    
			    foundPoints.Add(point);
			    IterateTerrainGraphUpdate(new KeyValuePair<Vector2, TileId>(point.Position, TileId.Grass), 
				    false);
		    }
	    }
	    
	    IterateTerrainDilationUpdate(new KeyValuePair<int, IList<Vector2>>(ImpassableIndex, 
		    foundPoints.Select(x => x.Position).ToList()));

	    foreach (var foundPosition in foundPoints.Select(x => x.Position))
	    {
		    for (var x = (int)foundPosition.x - offset; x <= (int)foundPosition.x + offset; x++)
		    {
			    for (var y = (int)foundPosition.y - offset; y <= (int)foundPosition.y + offset; y++)
			    {
				    var position = new Vector2(x, y);
				    if (Points.ContainsPoint(position) is false)
					    continue;

				    IterateDiagonalConnectionUpdate(new KeyValuePair<Vector2, int>(position, Points.GetId(position)));
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

	    for (var x = (int)start.x; x < (int)end.x; x++)
	    {
		    for (var y = (int)start.y; y < (int)end.y; y++)
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
	    
	    for (var x = (int)start.x; x < (int)end.x; x++)
	    {
		    for (var y = (int)start.y; y < (int)end.y; y++)
		    {
			    var position = new Vector2(x, y);
			    if (Points.ContainsPoint(position) is false)
				    continue;
			    
			    IterateDiagonalConnectionUpdate(new KeyValuePair<Vector2, int>(position, Points.GetId(position)));
		    }
	    }
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

    private void AddAscendableHighGround(IReadOnlyList<(IList<Vector2>, int)> path) // TODO 2x 3x pathfindings
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
			    if (Points.ContainsPoint(pos, true))
			    {
				    pointId = Points.GetId(pos, true);
				    point = Points.GetPoint(pointId);
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
				    Points.Add(point);
				    EventBus.Instance.RaiseHighGroundPointCreated(point);
				    GD.Print($"2. Creating point {JsonConvert.SerializeObject(point)}");
			    }
		    
			    for (var xOffset = -1; xOffset < 2; xOffset++)
			    {
				    for (var yOffset = -1; yOffset < 2; yOffset++)
				    {
					    var lowGroundPoint = GetAdjacentPoint(point, new Vector2(xOffset, yOffset), 
						    false);
					    if (i == 0 && lowGroundPoint != null)
					    {
						    _pathfinding.ConnectPoints(pointId, ((Point)lowGroundPoint).Id);
						    GD.Print($"3. Connecting to low ground point {JsonConvert.SerializeObject(lowGroundPoint)}");
					    }

					    if (path.Count > 1 && i != 0)
					    {
						    var offsetPosition = point.Position + new Vector2(xOffset, yOffset);
						    var previousStepPositionExists = path[i - 1].Item1.Any(x => 
							    x.Equals(offsetPosition));
						    if (previousStepPositionExists)
						    {
							    var previousStepPosition = path[i - 1].Item1.FirstOrDefault(x => 
								    x.Equals(offsetPosition));
							    var previousStepPoint = Points.GetPoint(previousStepPosition, true);
							    _pathfinding.ConnectPoints(pointId, previousStepPoint.Id);
							    GD.Print($"4. Connecting to previous step point {JsonConvert.SerializeObject(previousStepPoint)}");
						    }
					    }
					    
					    var highGroundPoint = GetAdjacentPoint(point, new Vector2(xOffset, yOffset), 
						    true);
					    if (highGroundPoint is null)
						    continue;

					    _pathfinding.ConnectPoints(pointId, ((Point)highGroundPoint).Id);
					    GD.Print($"5. Connecting to adjacent high ground point {JsonConvert.SerializeObject(highGroundPoint)}");
				    }
			    }
		    }
	    }
    }

    private void RemoveAscendableHighGround(List<(IList<Vector2>, int)> path) // TODO 2x 3x pathfindings; not tested properly
    {
	    RemoveHighGround(path);
    }

    private void AddHighGround(List<(IList<Vector2>, int)> path) // TODO 2x 3x pathfindings
    {
	    var flattenedPositions = GetFlattenedPositions(path);

	    foreach (var pos in flattenedPositions)
	    {
		    if (pos.IsInBoundsOf(MapSize) is false)
			    continue;
		    
		    if (Points.ContainsPoint(pos, true))
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
		    Points.Add(point);
		    EventBus.Instance.RaiseHighGroundPointCreated(point);
		    GD.Print($"1. Creating point {JsonConvert.SerializeObject(point)}");
		    
		    for (var xOffset = -1; xOffset < 2; xOffset++)
		    {
			    for (var yOffset = -1; yOffset < 2; yOffset++)
			    {
				    var otherPoint = GetAdjacentPoint(point, new Vector2(xOffset, yOffset), true);
				    if (otherPoint is null)
					    continue;

				    _pathfinding.ConnectPoints(pointId, ((Point)otherPoint).Id);
				    GD.Print($"5. Connecting to adjacent high ground point {JsonConvert.SerializeObject(otherPoint)}");
			    }
		    }
	    }
    }

    private void RemoveHighGround(List<(IList<Vector2>, int)> path)  // TODO 2x 3x pathfindings; not tested properly
    {
	    var flattenedPositions = GetFlattenedPositions(path);
	    
	    foreach (var pos in flattenedPositions)
	    {
		    if (pos.IsInBoundsOf(MapSize) is false)
			    continue;
		    
		    if (Points.ContainsPoint(pos, true) is false)
			    continue;

		    var point = Points.GetPoint(pos, true);
		    
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
		    
		    _pathfinding.RemovePoint(point.Id);
		    EventBus.Instance.RaiseHighGroundPointRemoved(point);
		    Points.Remove(point.Id);
	    }
    }

    private static IEnumerable<Vector2> GetFlattenedPositions(List<(IList<Vector2>, int)> from)
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
	    
	    if (Points.ContainsPoint(otherPosition, isHighGround) is false)
		    return null;
	    
	    var otherPoint = Points.GetPoint(otherPosition, isHighGround);

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
			    Points.GetId(at),
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

public class DijkstraMap : Node
{
	private Object _dijkstraMap;
	
	public DijkstraMap()
	{
		var dijkstraMapScript = GD.Load("res://addons/dijkstra-map/Dijkstra_map_library/nativescript.gdns") as NativeScript;
		_dijkstraMap = dijkstraMapScript?.New() as Object;
		if (_dijkstraMap is null) throw new ArgumentNullException($"{nameof(_dijkstraMap)} cannot be null.");
	}
	
	public Godot.Collections.Dictionary<Vector2, int> AddSquareGrid(Rect2 bounds, int terrain, float orthCost,
		float diagCost)
	{
		var dictionary = _dijkstraMap.Call("add_square_grid", bounds, terrain, orthCost, diagCost) 
			as Godot.Collections.Dictionary;
		return new Godot.Collections.Dictionary<Vector2, int>(dictionary);
	}

	public void SetTerrainForPoint(int pointId, int terrain)
	{
		_dijkstraMap.Call("set_terrain_for_point", pointId, terrain);
	}
	
	public int GetTerrainForPoint(int pointId)
	{
		return (int)_dijkstraMap.Call("get_terrain_for_point", pointId);
	}

	public void Recalculate(int pointId, Godot.Collections.Dictionary<string, object> options)
	{
		_dijkstraMap.Call("recalculate", pointId, options);
	}

	public int[] GetAllPointsWithCostBetween(float min, float max)
	{
		return _dijkstraMap.Call("get_all_points_with_cost_between", min, max)
			as int[];
	}

	public int[] GetShortestPathFromPoint(int pointId)
	{
		return _dijkstraMap.Call("get_shortest_path_from_point", pointId)
			as int[];
	}

	public void Clear()
    {
        _dijkstraMap.Call("clear");
    }

    public Error DuplicateGraphFrom(DijkstraMap sourceInstance)
    {
        return (Error)_dijkstraMap.Call("duplicate_graph_from", sourceInstance._dijkstraMap);
    }

    public int GetAvailablePointId()
    {
        return (int)_dijkstraMap.Call("get_available_point_id");
    }

    public Error AddPoint(int pointId, int terrainType = -1)
    {
        return (Error)_dijkstraMap.Call("add_point", pointId, terrainType);
    }

    public Error RemovePoint(int pointId)
    {
        return (Error)_dijkstraMap.Call("remove_point", pointId);
    }

    public bool HasPoint(int pointId)
    {
        return (bool)_dijkstraMap.Call("has_point", pointId);
    }
    
    public Error DisablePoint(int pointId)
    {
        return (Error)_dijkstraMap.Call("disable_point", pointId);
    }
    
    public Error EnablePoint(int pointId)
    {
        return (Error)_dijkstraMap.Call("enable_point", pointId);
    }
    
    public bool IsPointDisabled(int pointId)
    {
        return (bool)_dijkstraMap.Call("is_point_disabled", pointId);
    }
    
    public Error ConnectPoints(int source, int target, float weight = 1f, bool bidirectional = true)
    {
        return (Error)_dijkstraMap.Call("connect_points", source, target, weight, bidirectional);
    }
    
    public Error RemoveConnection(int source, int target, bool bidirectional = true)
    {
        return (Error)_dijkstraMap.Call("remove_connection", source, target, bidirectional);
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