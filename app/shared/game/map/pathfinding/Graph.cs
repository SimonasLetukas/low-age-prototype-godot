using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data;
using low_age_data.Domain.Common;
using Object = Godot.Object;

public struct Point : IEquatable<Point>
{
	public int Id { get; set; }
	public Vector2 Position { get; set; }
	public bool IsHighGround { get; set; }
	public int HighGroundAscensionLevel { get; set; }
	public int YSpriteOffset { get; set; }
	public int TerrainIndex { get; set; }
	public bool IsImpassable { get; set; }

	public bool Equals(Point other)
	{
		return Id == other.Id 
		       && Position.Equals(other.Position) 
		       && IsHighGround == other.IsHighGround 
		       && HighGroundAscensionLevel == other.HighGroundAscensionLevel 
		       && YSpriteOffset == other.YSpriteOffset 
		       && TerrainIndex == other.TerrainIndex 
		       && IsImpassable == other.IsImpassable;
	}

	public override bool Equals(object obj)
	{
		return obj is Point other && Equals(other);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hashCode = Id;
			hashCode = (hashCode * 397) ^ Position.GetHashCode();
			hashCode = (hashCode * 397) ^ IsHighGround.GetHashCode();
			hashCode = (hashCode * 397) ^ HighGroundAscensionLevel;
			hashCode = (hashCode * 397) ^ YSpriteOffset;
			hashCode = (hashCode * 397) ^ TerrainIndex;
			hashCode = (hashCode * 397) ^ IsImpassable.GetHashCode();
			return hashCode;
		}
	}
}

/// <summary>
/// <para>
/// Extends <see cref="DijkstraMap"/> with additional dimensions: size, team. Manages the resulting multiple
/// <see cref="DijkstraMap"/>s and <see cref="Point"/>s within.
/// </para>
/// <para>
/// "Main" graph implies a size of 1 and *any* team.
/// </para>
/// </summary>
public class Graph
{
	#region PointChanges

	private interface IPointChange { }

	private class PointAdded : IPointChange { }

	private class PointTerrainChanged : IPointChange { }

	private class PointRemoved : IPointChange { }

	private class PointConnectionsAdded : IPointChange
	{
		public IEnumerable<Connection> Connections { get; }
		
		public PointConnectionsAdded(IEnumerable<Connection> connections)
		{
			Connections = connections;
		}
	}
	
	private class PointConnectionsRemoved : IPointChange
	{
		public IEnumerable<Point> Points { get; }
		
		public PointConnectionsRemoved(IEnumerable<Point> points)
		{
			Points = points;
		}
	}
	
	private struct Connection
	{
		public Point Point { get; }
		public bool IsDiagonallyAdjacent { get; }
		
		public Connection(Point point, bool isDiagonallyAdjacent)
		{
			Point = point;
			IsDiagonallyAdjacent = isDiagonallyAdjacent;
		}
	}

	#endregion

	public IList<int> SupportedTeams { get; } = new List<int>();
	public IList<int> SupportedSizes { get; } = new List<int>();
	
	private const int MaxSizeForPathfinding = Constants.Pathfinding.MaxSizeForPathfinding;
    private const float DiagonalCost = Constants.Pathfinding.DiagonalCost;
    private const int ImpassableIndex = Constants.Pathfinding.ImpassableIndex;
    private const int HighGroundIndex = Constants.Pathfinding.HighGroundIndex;
    
    private Vector2 MapSize { get; set; }
    private int PointIdIterator { get; set; }
    private Blueprint Blueprint { get; set; }

    private static Godot.Collections.Dictionary<int, float> _terrainWeights =
	    new Godot.Collections.Dictionary<int, float>
	    {
		    { Terrain.Grass.ToIndex(), 1.0f },
		    { Terrain.Mountains.ToIndex(), float.PositiveInfinity },
		    { Terrain.Marsh.ToIndex(), 2.0f },
		    { ImpassableIndex, float.PositiveInfinity },
		    { HighGroundIndex, 1.0f }
	    };

    private class PointById : Dictionary<int, Point> { }
    private class PointByIdBySize : Dictionary<int, PointById> { }
    private Dictionary<int, PointByIdBySize> PointByIdBySizeByTeam { get; } = new Dictionary<int, PointByIdBySize>();
	
	private class PointIdByPosition : Dictionary<(Vector2, bool), int> { }
	private class PointIdByPositionBySize : Dictionary<int, PointIdByPosition> { }
	private Dictionary<int, PointIdByPositionBySize> PointIdByPositionBySizeByTeam { get; } = new Dictionary<int, PointIdByPositionBySize>();
	
	private class ChangedPointById : Dictionary<int, IPointChange> { }
	private class ChangedPointByIdBySize : Dictionary<int, ChangedPointById> { }
	private Dictionary<int, ChangedPointByIdBySize> ChangedPointByIdBySizeByTeam { get; } = new Dictionary<int, ChangedPointByIdBySize>();
	
	private class TerrainWeightByPointId : Dictionary<int, float> { }
	private class PointTerrainWeightByIdBySize : Dictionary<int, TerrainWeightByPointId> { }
	private Dictionary<int, PointTerrainWeightByIdBySize> PointTerrainWeightByIdBySizeByTeam { get; } = new Dictionary<int, PointTerrainWeightByIdBySize>();
	
	private class DijkstraMapBySize : Dictionary<int, DijkstraMap> { }
	private Dictionary<int, DijkstraMapBySize> DijkstraMapBySizeByTeam { get; } = new Dictionary<int, DijkstraMapBySize>();
	
	private DijkstraMap MainDijkstraMap(int team) => DijkstraMapBySizeByTeam[team][1];
	private Dictionary<int, DijkstraMap> NonMainDijkstraMaps(int team) => DijkstraMapBySizeByTeam[team]
		.OrderBy(pair => pair.Key).Skip(1).ToDictionary(pair => pair.Key, pair => pair.Value);

	#region Initialization

	public void Initialize(Vector2 mapSize, int amountOfTeams = 1)
	{
		MapSize = mapSize;
		Blueprint = Data.Instance.Blueprint;
		PointIdIterator = (int)MapSize.x * (int)MapSize.y;
		
		for (var team = 1; team <= amountOfTeams; team++)
		{
			SupportedTeams.Add(team);

			PointByIdBySizeByTeam[team] = new PointByIdBySize();
			PointIdByPositionBySizeByTeam[team] = new PointIdByPositionBySize();
			ChangedPointByIdBySizeByTeam[team] = new ChangedPointByIdBySize();
			PointTerrainWeightByIdBySizeByTeam[team] = new PointTerrainWeightByIdBySize();
			DijkstraMapBySizeByTeam[team] = new DijkstraMapBySize();
			
			for (var size = 1; size <= MaxSizeForPathfinding; size++)
			{
				if (size != 1 && Blueprint.Entities.Units.Any(u => u.Size == size) is false)
					continue;
			
				SupportedSizes.Add(size);

				PointByIdBySizeByTeam[team][size] = new PointById();
				PointIdByPositionBySizeByTeam[team][size] = new PointIdByPosition();
				ChangedPointByIdBySizeByTeam[team][size] = new ChangedPointById();
				PointTerrainWeightByIdBySizeByTeam[team][size] = new TerrainWeightByPointId();
				
				var dijkstraMap = new DijkstraMap(); // TODO inject as a dependency and mock its interface for tests
				DijkstraMapBySizeByTeam[team][size] = dijkstraMap;
			}
		}
		
		_terrainWeights = new Godot.Collections.Dictionary<int, float>();
		foreach (var tile in Blueprint.Tiles)
		{
			_terrainWeights.Add(tile.Terrain.ToIndex(), tile.MovementCost);
		}

		_terrainWeights.Add(ImpassableIndex, float.PositiveInfinity);
		_terrainWeights.Add(HighGroundIndex, 1.0f);
	}
	
	/// <summary>
	/// Creates a square grid of points for all teams and entity sizes, and returns all points and their IDs (which
	/// are same across all graphs for all teams and entity sizes).
	/// </summary>
	/// <param name="bounds">Area in which the square grid is created.</param>
	/// <param name="terrainIndex">Base terrain index to set to all the points initially.</param>
	public Dictionary<Vector2, int> InitializeSquareGrid(Rect2 bounds, int terrainIndex)
	{
		var pointIdByPositionResult = new Dictionary<Vector2, int>();
		
		foreach (var team in SupportedTeams)
		{
			var mainDijkstraMap = MainDijkstraMap(team);
			var graph = mainDijkstraMap.AddSquareGrid(
				bounds,
				terrainIndex,
				1.0f,
				DiagonalCost);
			pointIdByPositionResult = SetBasePoints(team, 1, graph, terrainIndex);

			foreach (var nonMainDijkstraMap in NonMainDijkstraMaps(team))
			{
				nonMainDijkstraMap.Value.DuplicateGraphFrom(mainDijkstraMap);
				SetBasePoints(team, nonMainDijkstraMap.Key, graph, terrainIndex);
			}
		}

		return pointIdByPositionResult;
	}
	
	private Dictionary<Vector2, int> SetBasePoints(int team, int size, Godot.Collections.Dictionary<Vector2, int> graph, 
		int basePointTerrainIndex)
	{
		var pointIdByPosition = new Dictionary<Vector2, int>();
		
		foreach (var (position, id) in graph)
		{
			var point = new Point
			{
				Id = id,
				Position = position,
				IsHighGround = false,
				HighGroundAscensionLevel = 0,
				TerrainIndex = basePointTerrainIndex,
				IsImpassable = false
			};

			PointByIdBySizeByTeam[team][size][id] = point;
			PointIdByPositionBySizeByTeam[team][size][(position, false)] = id;
			pointIdByPosition[position] = id;
		}

		return pointIdByPosition;
	}

	#endregion

	public void SaveChanges()
	{
		foreach (var (team, changedPointBySize) in ChangedPointByIdBySizeByTeam)
		{
			foreach (var (size, changedPoints) in changedPointBySize)
			{
				foreach (var (pointId, status) in changedPoints)
				{
					var point = GetPoint(pointId, team, size);
					var terrainIndex = point.IsHighGround 
						? HighGroundIndex 
						: point.IsImpassable 
							? ImpassableIndex 
							: point.TerrainIndex;

					switch (status)
					{
						case PointAdded _:
							DijkstraMapBySizeByTeam[team][size].AddPoint(point.Id, terrainIndex);
							break;
						
						case PointRemoved _:
							DijkstraMapBySizeByTeam[team][size].RemovePoint(point.Id);
							break;
							
						case PointTerrainChanged _:
							DijkstraMapBySizeByTeam[team][size].SetTerrainForPoint(point.Id, terrainIndex);
							break;
						
						case PointConnectionsAdded pointConnectionsAdded:
							foreach (var connection in pointConnectionsAdded.Connections)
							{
								DijkstraMapBySizeByTeam[team][size].ConnectPoints(point.Id, 
									connection.Point.Id, connection.IsDiagonallyAdjacent ? DiagonalCost : 1f);
							}
							break;
						
						case PointConnectionsRemoved pointConnectionsRemoved:
							foreach (var connection in pointConnectionsRemoved.Points)
							{
								DijkstraMapBySizeByTeam[team][size].RemoveConnection(point.Id, connection.Id);
							}
							break;
						
						default:
							throw new ArgumentOutOfRangeException($"{nameof(Graph)}.{nameof(SaveChanges)}: " +
							                                      $"{nameof(status)}");
					}
				}
			}
		}

		foreach (var team in SupportedTeams)
		{
			ChangedPointByIdBySizeByTeam[team] = new ChangedPointByIdBySize();

			foreach (var size in SupportedSizes)
			{
				ChangedPointByIdBySizeByTeam[team][size] = new ChangedPointById();
			}
		}
	}

	public bool IsSupported(int team, int size) => SupportedTeams.Contains(team) && SupportedSizes.Contains(size);

	#region Terrain
	
	public void SetTerrainForPoint(Vector2 coordinates, int terrainIndex)
	{
		if (TryGetPointId(coordinates, 1, 1, out _) is false)
			return;

		foreach (var team in SupportedTeams)
		{
			foreach (var size in SupportedSizes)
			{
				var dilatedPositions = Iterate.Positions(coordinates, coordinates + Vector2.One * size);
				foreach (var position in dilatedPositions)
				{
					if (TryGetPointId(position, team, size, out var pointId) is false)
						continue;
				
					var point = GetPoint(pointId, team, size);
					point.TerrainIndex = terrainIndex;
					ChangedPointByIdBySizeByTeam[team][size][pointId] = new PointTerrainChanged();
					PointTerrainWeightByIdBySizeByTeam[team][size][pointId] = _terrainWeights[terrainIndex];
				}
			}
		}
	}

	public void SetPointsImpassable(bool to, IEnumerable<Point> points)
	{
		foreach (var originPoint in points)
		{
			foreach (var team in SupportedTeams)
			{
				foreach (var size in SupportedSizes)
				{
					var dilatedPositions = Iterate.Positions(
						originPoint.Position, 
						originPoint.Position + Vector2.One * size);
					
					foreach (var position in dilatedPositions)
					{
						if (TryGetPointId(position, team, size, out var pointId) is false)
							continue;
				
						var point = GetPoint(pointId, team, size);
						SetPointImpassible(to, point, team, size);
					}
				}
			}
		}
	}
	
	public void SetPointImpassible(bool to, Point point, int team, int size)
	{
		point.IsImpassable = to;
		ChangedPointByIdBySizeByTeam[team][size][point.Id] = new PointTerrainChanged();
	}

	public void UpdateDiagonalConnection(Vector2 position)
	{
		var diagonalPosition = position + new Vector2(1, 1);
		if (diagonalPosition.IsInBoundsOf(MapSize) is false)
			return;
		
		var rightPosition = position + new Vector2(1, 0);
		var bottomPosition = position + new Vector2(0, 1);

		foreach (var team in SupportedTeams)
		{
			foreach (var size in SupportedSizes)
			{
				var points = new List<Point>();
				if (TryGetPointId(position, team, size, out var highGroundPointId, true)) 
					points.Add(GetPoint(highGroundPointId, team, size));
				if (TryGetPointId(position, team, size, out var lowGroundPointId, false)) 
					points.Add(GetPoint(lowGroundPointId, team, size));

				foreach (var point in points)
				{
					if (TryGetHighestPoint(rightPosition, team, size, point.IsHighGround, 
						    out var rightNeighbour) is false)
						continue;
					if (TryGetHighestPoint(bottomPosition, team, size, point.IsHighGround, 
						    out var bottomNeighbour) is false)
						continue;
					if (TryGetHighestPoint(diagonalPosition, team, size, point.IsHighGround, 
						    out var diagonalNeighbour) is false)
						continue;
					
					// x.
					// .x
					if (IsInfiniteTerrainWeight(point.Id, team, size)
					    && IsInfiniteTerrainWeight(diagonalNeighbour.Id, team, size)
					    && IsInfiniteTerrainWeight(rightNeighbour.Id, team, size) is false
					    && IsInfiniteTerrainWeight(bottomNeighbour.Id, team, size) is false
					    && rightNeighbour.IsHighGround == bottomNeighbour.IsHighGround) // TODO not tested
					{
						ChangedPointByIdBySizeByTeam[team][size][rightNeighbour.Id] =
							new PointConnectionsRemoved(new[] { bottomNeighbour });
					}
					else
					{
						ChangedPointByIdBySizeByTeam[team][size][rightNeighbour.Id] =
							new PointConnectionsAdded(new[] { new Connection(bottomNeighbour, true) });
					}

					// .x
					// x.
					if (IsInfiniteTerrainWeight(point.Id, team, size) is false
					    && IsInfiniteTerrainWeight(diagonalNeighbour.Id, team, size) is false
					    && IsInfiniteTerrainWeight(rightNeighbour.Id, team, size)
					    && IsInfiniteTerrainWeight(bottomNeighbour.Id, team, size)
					    && point.IsHighGround == diagonalNeighbour.IsHighGround) // TODO not tested
					{
						ChangedPointByIdBySizeByTeam[team][size][point.Id] =
							new PointConnectionsRemoved(new[] { diagonalNeighbour });
					}
					else
					{
						ChangedPointByIdBySizeByTeam[team][size][point.Id] =
							new PointConnectionsAdded(new[] { new Connection(diagonalNeighbour, true) });
					}
				}
			}
		}
	}
	
	public bool IsInfiniteTerrainWeight(int pointId, int team, int size)
		=> PointTerrainWeightByIdBySizeByTeam[team][size][pointId].Equals(float.PositiveInfinity) 
		   || PointByIdBySizeByTeam[team][size][pointId].IsImpassable;

	#endregion

	#region Points

	/// <summary>
	/// Gets all terrain (low ground) points from the main (1x1) graph. Can be used to get actual terrain of a point.
	/// Terrain is the same for all teams.
	/// </summary>
	public IEnumerable<Point> GetTerrainPoints() => PointByIdBySizeByTeam[1][1].Values.Where(x => 
		x.IsHighGround is false);

	public Point? GetTerrainPoint(int id)
	{
		var point = GetPoint(id, 1, 1);
		if (point.IsHighGround)
			return null;
		return point;
	}

	public IList<Point> GetPointsForEntity(EntityNode entity)
	{
		var points = new List<Point>();
		var isOnHighGround = entity is UnitNode unit && unit.IsOnHighGround;
		var size = (int)entity.EntitySize.x;
		var team = 1; // TODO add team
		
		for (var x = (int)entity.EntityPrimaryPosition.x - MaxSizeForPathfinding; 
		     x < entity.EntityPrimaryPosition.x + entity.EntitySize.x + MaxSizeForPathfinding; 
		     x++)
		{
			for (var y = (int)entity.EntityPrimaryPosition.y - MaxSizeForPathfinding;
			     y < entity.EntityPrimaryPosition.y + entity.EntitySize.y + MaxSizeForPathfinding;
			     y++)
			{
				var position = new Vector2(x, y);
				var point = GetHighestPoint(position, team, size, isOnHighGround);
				if (point is null)
					continue;
				
				points.Add(point.Value);
			}
		}

		return points;
	}
	
	public Point GetPoint(int id, int team, int size) => PointByIdBySizeByTeam[team][size][id];

	public Point GetPoint(Vector2 position, int team, int size, bool isHighGround = false) 
		=> GetPoint(GetPointId(position, team, size, isHighGround), team, size);

	public Point GetMainPoint(int id, int team = 1) => GetPoint(id, team, 1);
	
	public Point GetMainPoint(Vector2 position, bool isHighGround = false, int team = 1)
		=> GetPoint(position, team, 1, isHighGround);

	public bool TryGetHighestPoint(Vector2 position, int team, int size, bool lookingFromHighGround, out Point point)
	{
		point = new Point();
		var result = GetHighestPoint(position, team, size, lookingFromHighGround);
		if (result is null)
			return false;
		
		point = result.Value;
		return true;
	}
	
	public Point? GetHighestPoint(Vector2 position, int team, int size, bool lookingFromHighGround)
	{
		var highGroundPointFound = TryGetPointId(position, team, size, out var highGroundPointId, true);
		var lowGroundPointFound = TryGetPointId(position, team, size, out var lowGroundPointId);
				
		if (highGroundPointFound && lookingFromHighGround)
			return GetPoint(highGroundPointId, team, size);

		if (lowGroundPointFound is false)
			return null;

		return GetPoint(lowGroundPointId, team, size);
	}

	public bool ContainsMainPoint(Vector2 position, bool isHighGround = false, int team = 1)
		=> ContainsPoint(position, team, 1, isHighGround);

	public bool ContainsPoint(Vector2 position, int team, int size, bool isHighGround = false)
		=> PointIdByPositionBySizeByTeam[team][size].ContainsKey((position, isHighGround));

	public bool TryGetPointId(Vector2 position, int team, int size, out int id, bool isHighGround = false)
	{
		id = -1;
		
		var result = PointIdByPositionBySizeByTeam.TryGetValue(team, out _);
		if (result is false)
			return false;

		result = PointIdByPositionBySizeByTeam[team].TryGetValue(size, out _);
		if (result is false)
			return false;
		
		result = PointIdByPositionBySizeByTeam[team][size].TryGetValue((position, isHighGround), out var idResult);
		
		id = idResult;
		return result;
	}
	
	public int GetPointId(Vector2 position, int team, int size, bool isHighGround = false) 
		=> PointIdByPositionBySizeByTeam[team][size][(position, isHighGround)];
	
	public int GetMainPointId(Vector2 position, bool isHighGround = false, int team = 1)
		=> GetPointId(position, team, 1, isHighGround);

	public Point AddPoint(Vector2 position, bool isHighGround, int highGroundAscensionLevel, int ySpriteOffset, 
		int terrainIndex = ImpassableIndex)
	{
		var point = new Point
		{
			Id = PointIdIterator++,
			Position = position,
			IsHighGround = isHighGround,
			HighGroundAscensionLevel = highGroundAscensionLevel,
			YSpriteOffset = ySpriteOffset,
			TerrainIndex = isHighGround ? HighGroundIndex : terrainIndex,
			IsImpassable = false
		};

		foreach (var team in SupportedTeams)
		{
			foreach (var size in SupportedSizes)
			{
				PointByIdBySizeByTeam[team][size][point.Id] = point;
				PointIdByPositionBySizeByTeam[team][size][(point.Position, point.IsHighGround)] = point.Id;
				PointTerrainWeightByIdBySizeByTeam[team][size][point.Id] = _terrainWeights[point.TerrainIndex];
				ChangedPointByIdBySizeByTeam[team][size][point.Id] = new PointAdded();
			}
		}

		return point;
	}

	/// <summary>
	/// Removes point for all teams and all sizes.
	/// </summary>
	/// <param name="id">Point ID</param>
	public void RemoveAllPoints(int id)
	{
		foreach (var size in SupportedSizes)
		{
			RemovePoint(id, size);
		}
	}

	/// <summary>
	/// Removes point for all teams in the specified size graph.
	/// </summary>
	/// <param name="id">Point ID</param>
	/// <param name="size">Size graph</param>
	public void RemovePoint(int id, int size)
	{
		foreach (var team in SupportedTeams)
		{
			RemovePoint(id, team, size);
		}
	}

	/// <summary>
	/// Removes point for specified team and size graph.
	/// </summary>
	/// <param name="id">Point ID</param>
	/// <param name="team">Team graph</param>
	/// <param name="size">Size graph</param>
	public void RemovePoint(int id, int team, int size)
	{
		var point = PointByIdBySizeByTeam[team][size][id];

		PointByIdBySizeByTeam[team][size].Remove(point.Id);
		PointIdByPositionBySizeByTeam[team][size].Remove((point.Position, point.IsHighGround));

		ChangedPointByIdBySizeByTeam[team][size][point.Id] = new PointRemoved();
	}

	#endregion

	#region Path

	public void Recalculate(Vector2 from, float range, bool isOnHighGround, int team, int size)
	{
		var point = GetPoint(from, team, size, isOnHighGround);
		Recalculate(point.Id, range, team, size);
	}

	public void Recalculate(int pointId, float range, int team, int size)
	{
		DijkstraMapBySizeByTeam[team][size].Recalculate(pointId, new Godot.Collections.Dictionary<string, object>
		{
			{ "maximum_cost", range },
			{ "terrain_weights", _terrainWeights }
		});
	}

	public IEnumerable<Point> GetAllPointsWithCostBetween(float min, float max, int team, int size)
	{
		var pointIds = DijkstraMapBySizeByTeam[team][size].GetAllPointsWithCostBetween(min, max);
		var points = pointIds.Select(id => GetPoint(id, team, size));
		return points;
	}

	public IEnumerable<Point> GetShortestPathFromPoint(int pointId, int team, int size)
	{
		var pointIds = DijkstraMapBySizeByTeam[team][size].GetShortestPathFromPoint(pointId);
		var points = pointIds.Select(id => GetPoint(id, team, size));
		return points;
	}

	#endregion

	#region Connections

	public bool HasConnection(Point pointA, Point pointB, int team, int size)
		=> DijkstraMapBySizeByTeam[team][size].HasConnection(pointA.Id, pointB.Id);
	
	public bool HasMainConnection(Point pointA, Point pointB, int team = 1)
		=> HasConnection(pointA, pointB, team, 1);

	public void ConnectPoints(Point pointA, Point pointB, bool isDiagonallyAdjacent, int size)
	{
		foreach (var team in SupportedTeams)
		{
			ChangedPointByIdBySizeByTeam[team][size][pointA.Id] = new PointConnectionsAdded(new[]
			{
				new Connection(pointB, isDiagonallyAdjacent)
			});
		}
	}

	#endregion
}

// Documentation: https://github.com/MatejSloboda/Dijkstra_map_for_Godot/blob/master/addons/dijkstra-map/doc/DijkstraMap.md
public class DijkstraMap : Node
{
	private Object _dijkstraMap;
	
	public DijkstraMap()
	{
		var dijkstraMapScript = GD.Load("res://addons/dijkstra-map/Dijkstra_map_library/nativescript.gdns") as NativeScript;
		_dijkstraMap = dijkstraMapScript?.New() as Object;
		if (_dijkstraMap is null) 
			throw new ArgumentNullException($"{nameof(_dijkstraMap)} cannot be null.");
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