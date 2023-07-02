using System.Collections.Generic;
using System.Linq;
using Godot;

public class Pathfinding : Node // TODO not tested
{
    public Vector2 MapSize { get; private set; }
    public Godot.Collections.Dictionary<Vector2, int> PointIdsByPositions { get; private set; }
    public Godot.Collections.Dictionary<int, Vector2> PositionsByPointIds { get; private set; }

    private static readonly Godot.Collections.Dictionary<Constants.Game.Terrain, float> TerrainWeights = new Godot.Collections.Dictionary<Constants.Game.Terrain, float>
    {
        { Constants.Game.Terrain.Grass, 1.0f },
        { Constants.Game.Terrain.Mountains, float.PositiveInfinity },
        { Constants.Game.Terrain.Marsh, 2.0f }
    };
    
    // Documentation: https://github.com/MatejSloboda/Dijkstra_map_for_Godot/blob/master/DOCUMENTATION.md
    private readonly DijkstraMap _pathfinding = new DijkstraMap();

    private Vector2 _previousPosition = Vector2.Inf;
    private float _previousRange = -1.0f;

    public void Initialize(Vector2 mapSize) // TODO not tested if this works without ready, especially caching
    {
	    MapSize = mapSize;
	    
	    PointIdsByPositions = _pathfinding.AddSquareGrid(
		    0,
		    new Rect2(0, 0, MapSize.x, MapSize.y),
		    (int) Constants.Game.Terrain.Grass,
		    1.0f,
		    Mathf.Sqrt(2));

	    PositionsByPointIds = new Godot.Collections.Dictionary<int, Vector2>();
	    foreach (var position in PointIdsByPositions.Keys)
	    {
		    var id = PointIdsByPositions[position];
		    PositionsByPointIds[id] = position;
	    }
    }

    public void SetTerrainForPoint(Vector2 at, Constants.Game.Terrain terrain)
    {
	    if (at.IsInBoundsOf(MapSize))
	    {
		    _pathfinding.SetTerrainForPoint(
			    PointIdsByPositions[at],
			    (int) terrain);
	    }
    }

    public Vector2[] GetAvailablePositions(Vector2 from, float range, int size = 1)
    {
	    if (IsCached(from, range) is false)
	    {
		    _pathfinding.Recalculate(
			    PointIdsByPositions[from],
			    new Godot.Collections.Dictionary<string, object>
			    {
				    { "maximum_cost", range },
				    { "terrain_weights", TerrainWeights }
			    });
		    Cache(from, range);
	    }

	    var availablePointIds = _pathfinding.GetAllPointsWithCostBetween(0.0f, range);
	    return availablePointIds.Select(pointId => PositionsByPointIds[pointId]).ToArray();
    }

    public Vector2[] FindPath(Vector2 to)
    {
	    if (to.IsInBoundsOf(MapSize) is false)
	    {
		    return new []{ to };
	    }

	    var targetPointId = PointIdsByPositions[to];
	    var pathOfPointIds = _pathfinding.GetShortestPathFromPoint(targetPointId);

	    var path = new List<Vector2> { to };
	    path.AddRange(pathOfPointIds.Select(pointId => PositionsByPointIds[pointId]));
	    path.Reverse();

	    return path.ToArray();
    }

    private bool IsCached(Vector2 position, float range) 
	    => position.Equals(_previousPosition) && range.Equals(_previousRange);

    private void Cache(Vector2 position, float range)
    {
	    _previousPosition = position;
	    _previousRange = range;
    }
}

public class DijkstraMap : Node // TODO not tested whether this works without scene + all calls
{
	private Script _dijkstraMap;

	public DijkstraMap()
	{
		_dijkstraMap = GD.Load("res://addons/dijkstra-map/Dijkstra_map_library/nativescript.gdns") as Script;
	}
	
	public override void _Ready()
	{
		_dijkstraMap = GD.Load("res://addons/dijkstra-map/Dijkstra_map_library/nativescript.gdns") as Script;
	}

	public Godot.Collections.Dictionary<Vector2, int> AddSquareGrid(int pointOffset, Rect2 bounds, int terrain, float orthCost,
		float diagCost)
	{
		return _dijkstraMap.Call("add_square_grid", pointOffset, bounds, terrain, orthCost, diagCost) 
			as Godot.Collections.Dictionary<Vector2, int>;
	}

	public void SetTerrainForPoint(int pointId, int terrain)
	{
		_dijkstraMap.Call("set_terrain_for_point", pointId, terrain);
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
}
