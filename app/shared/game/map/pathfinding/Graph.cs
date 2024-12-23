using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data;
using low_age_dijkstra;
using low_age_dijkstra.Methods;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using Terrain = low_age_data.Domain.Common.Terrain;

public struct Point : IEquatable<Point>
{
	public int Id { get; set; }
	public Vector2 Position { get; set; }
	public int HighGroundAscensionLevel { get; set; }
	public bool IsHighGround { get; set; }
	public bool IsImpassable { get; set; }
	public int OriginalTerrainIndex { get; set; }
	public int CalculatedTerrainIndex => IsImpassable
		? Constants.Pathfinding.ImpassableIndex
		: IsHighGround
			? Constants.Pathfinding.HighGroundIndex
			: OriginalTerrainIndex;

	public bool Equals(Point other)
	{
		return Id == other.Id 
		       && Position.Equals(other.Position) 
		       && IsHighGround == other.IsHighGround 
		       && HighGroundAscensionLevel == other.HighGroundAscensionLevel 
		       && OriginalTerrainIndex == other.OriginalTerrainIndex 
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
			hashCode = (hashCode * 397) ^ OriginalTerrainIndex;
			hashCode = (hashCode * 397) ^ IsImpassable.GetHashCode();
			return hashCode;
		}
	}
}

/// <summary>
/// <para>
/// Extends <see cref="low_age_dijkstra.DijkstraMap"/> with additional dimensions: size, team. Manages the resulting multiple
/// <see cref="low_age_dijkstra.DijkstraMap"/>s and <see cref="Point"/>s within.
/// </para>
/// <para>
/// "Main" graph implies a size of 1 and *any* team.
/// </para>
/// </summary>
public class Graph
{
	public IList<int> SupportedTeams { get; } = new List<int>();
	public IList<int> SupportedSizes { get; } = new List<int>();

	private const int MaxSizeForPathfinding = Constants.Pathfinding.MaxSizeForPathfinding;
    private const float DiagonalCost = Constants.Pathfinding.DiagonalCost;
    private const int ImpassableIndex = Constants.Pathfinding.ImpassableIndex;
    private const int HighGroundIndex = Constants.Pathfinding.HighGroundIndex;
    
    private Vector2 MapSize { get; set; }
    private int PointIdIterator { get; set; }
    private Blueprint Blueprint { get; set; }

    private static Dictionary<int, float> _terrainWeights =
	    new Dictionary<int, float>
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
	
	private class DijkstraMapBySize : Dictionary<int, DijkstraMap> { }
	private Dictionary<int, DijkstraMapBySize> DijkstraMapBySizeByTeam { get; } = new Dictionary<int, DijkstraMapBySize>();
	
	private DijkstraMap MainDijkstraMap(int team) => DijkstraMapBySizeByTeam[team][1];
	private Dictionary<int, DijkstraMap> NonMainDijkstraMaps(int team) => DijkstraMapBySizeByTeam[team]
		.OrderBy(pair => pair.Key).Skip(1).ToDictionary(pair => pair.Key, pair => pair.Value);

	#region Initialization

	public void Initialize(Vector2 mapSize, int amountOfTeams = 1, int? forceSizesUpTo = null)
	{
		forceSizesUpTo = forceSizesUpTo ?? 1;
		MapSize = mapSize;
		Blueprint = Data.Instance.Blueprint;
		PointIdIterator = (int)MapSize.x * (int)MapSize.y;
		
		for (var team = 1; team <= amountOfTeams; team++)
		{
			SupportedTeams.Add(team);

			PointByIdBySizeByTeam[team] = new PointByIdBySize();
			PointIdByPositionBySizeByTeam[team] = new PointIdByPositionBySize();
			DijkstraMapBySizeByTeam[team] = new DijkstraMapBySize();
			
			for (var size = 1; size <= MaxSizeForPathfinding; size++)
			{
				var sizeExistsInBlueprint = Blueprint.Entities.Units.Any(u => u.Size == size);
				if (size != 1 && size > forceSizesUpTo && sizeExistsInBlueprint is false) 
					continue;
				
				SupportedSizes.Add(size);

				PointByIdBySizeByTeam[team][size] = new PointById();
				PointIdByPositionBySizeByTeam[team][size] = new PointIdByPosition();
				
				var dijkstraMap = new DijkstraMap(); // TODO inject as a dependency and mock its interface for tests
				DijkstraMapBySizeByTeam[team][size] = dijkstraMap;
			}
		}
		
		_terrainWeights = new Dictionary<int, float>();
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
				(int)bounds.Size.x,
				(int)bounds.Size.y,
				initialOffset: new Vector2<int>((int)bounds.Position.x, (int)bounds.Position.y),
				defaultTerrain: terrainIndex,
				orthogonalCost: 1.0f,
				diagonalCost: DiagonalCost);
			pointIdByPositionResult = SetBasePoints(team, 1, graph, terrainIndex);
			
			foreach (var nonMainDijkstraMap in NonMainDijkstraMaps(team))
			{
				nonMainDijkstraMap.Value.DuplicateGraphFrom(mainDijkstraMap);
				SetBasePoints(team, nonMainDijkstraMap.Key, graph, terrainIndex);
			}
		}

		return pointIdByPositionResult;
	}
	
	private Dictionary<Vector2, int> SetBasePoints(int team, int size, Dictionary<Vector2<int>, PointId> graph, 
		int basePointTerrainIndex)
	{
		var pointIdByPosition = new Dictionary<Vector2, int>();
		
		foreach (var (pos, pointId) in graph)
		{
			var id = pointId.Value;
			var position = pos.ToGodotVector2();
			var point = new Point
			{
				Id = id,
				Position = position,
				IsHighGround = false,
				HighGroundAscensionLevel = 0,
				OriginalTerrainIndex = basePointTerrainIndex,
				IsImpassable = false
			};

			PointByIdBySizeByTeam[team][size][id] = point;
			PointIdByPositionBySizeByTeam[team][size][(position, false)] = id;
			pointIdByPosition[position] = id;
		}

		return pointIdByPosition;
	}

	#endregion

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
				var dilatedPositions = Iterate.Positions(
					coordinates - Vector2.One * size + Vector2.One, 
					coordinates + Vector2.One);
				foreach (var position in dilatedPositions)
				{
					if (TryGetPointId(position, team, size, out var pointId) is false)
						continue;
				
					var point = GetPoint(pointId, team, size);
					point.OriginalTerrainIndex = terrainIndex;
					DijkstraMapBySizeByTeam[team][size].SetTerrainForPoint(point.Id, terrainIndex);
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
						originPoint.Position - Vector2.One * size + Vector2.One, 
						originPoint.Position + Vector2.One);
					
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
		DijkstraMapBySizeByTeam[team][size].SetTerrainForPoint(point.Id, point.CalculatedTerrainIndex);
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
					
					ApplyDiagonalConnectionsForPoints(
						point, 
						diagonalNeighbour, 
						rightNeighbour, 
						bottomNeighbour, 
						team, 
						size);
				}
			}
		}
	}

	private void ApplyDiagonalConnectionsForPoints(Point point, Point diagonalNeighbour, Point rightNeighbour,
		Point bottomNeighbour, int team, int size)
	{
		// x.
		// .x
		if (IsCurrentlyImpassable(point.Id, team, size)
		    && IsCurrentlyImpassable(diagonalNeighbour.Id, team, size)
		    && IsCurrentlyImpassable(rightNeighbour.Id, team, size) is false
		    && IsCurrentlyImpassable(bottomNeighbour.Id, team, size) is false)
		    //&& rightNeighbour.IsHighGround == bottomNeighbour.IsHighGround) // TODO not tested
		{
			DijkstraMapBySizeByTeam[team][size].RemoveConnection(
				rightNeighbour.Id, 
				bottomNeighbour.Id);
		}
		else
		{
			DijkstraMapBySizeByTeam[team][size].ConnectPoints(
				rightNeighbour.Id, 
				bottomNeighbour.Id, 
				DiagonalCost);
		}

		// .x
		// x.
		if (IsCurrentlyImpassable(point.Id, team, size) is false
		    && IsCurrentlyImpassable(diagonalNeighbour.Id, team, size) is false
		    && IsCurrentlyImpassable(rightNeighbour.Id, team, size)
		    && IsCurrentlyImpassable(bottomNeighbour.Id, team, size))
		    //&& point.IsHighGround == diagonalNeighbour.IsHighGround) // TODO not tested
		{
			DijkstraMapBySizeByTeam[team][size].RemoveConnection(
				point.Id, 
				diagonalNeighbour.Id);
		}
		else
		{
			DijkstraMapBySizeByTeam[team][size].ConnectPoints(
				point.Id, 
				diagonalNeighbour.Id, 
				DiagonalCost);
		}
	}
	

	public bool IsCurrentlyImpassable(int pointId, int team, int size)
	{
		var calculatedTerrainIndex = PointByIdBySizeByTeam[team][size][pointId].CalculatedTerrainIndex;
		var weight = _terrainWeights[calculatedTerrainIndex];
		return weight.Equals(float.PositiveInfinity);
	}

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

	public Point AddPoint(Vector2 position, bool isHighGround, int highGroundAscensionLevel, 
		int terrainIndex = ImpassableIndex)
	{
		var point = new Point
		{
			Id = PointIdIterator++,
			Position = position,
			IsHighGround = isHighGround,
			HighGroundAscensionLevel = highGroundAscensionLevel,
			OriginalTerrainIndex = isHighGround ? HighGroundIndex : terrainIndex,
			IsImpassable = false
		};

		foreach (var team in SupportedTeams)
		{
			foreach (var size in SupportedSizes)
			{
				PointByIdBySizeByTeam[team][size][point.Id] = point;
				PointIdByPositionBySizeByTeam[team][size][(point.Position, point.IsHighGround)] = point.Id;
				
				DijkstraMapBySizeByTeam[team][size].AddPoint(point.Id, point.CalculatedTerrainIndex);
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

		DijkstraMapBySizeByTeam[team][size].RemovePoint(point.Id);
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
		DijkstraMapBySizeByTeam[team][size].Recalculate(pointId,
			maximumCost: range,
			terrainWeights: _terrainWeights);
	}

	public IEnumerable<Point> GetAllPointsWithCostBetween(float min, float max, int team, int size)
	{
		var pointIds = DijkstraMapBySizeByTeam[team][size].GetAllPointsWithCostBetween(min, max);
		var points = pointIds.Select(id => GetPoint(id.Value, team, size));
		return points;
	}

	public IEnumerable<Point> GetShortestPathFromPoint(int pointId, int team, int size)
	{
		var pointIds = DijkstraMapBySizeByTeam[team][size].GetShortestPathFromPoint(pointId);
		var points = pointIds.Select(id => GetPoint(id.Value, team, size));
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
			DijkstraMapBySizeByTeam[team][size].ConnectPoints(
				pointA.Id, 
				pointB.Id, 
				isDiagonallyAdjacent ? DiagonalCost : 1f);
		}
	}

	#endregion
}