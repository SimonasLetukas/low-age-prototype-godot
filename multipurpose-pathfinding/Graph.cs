using System.Collections.Generic;
using System.Linq;
using low_age_dijkstra;
using low_age_dijkstra.Methods;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;

namespace multipurpose_pathfinding
{
    /// <summary>
    /// <para>
    /// Extends <see cref="low_age_dijkstra.DijkstraMap"/> with additional dimensions: size, team. Manages the resulting
    /// multiple <see cref="low_age_dijkstra.DijkstraMap"/>s and <see cref="Point"/>s within.
    /// </para>
    /// <para>
    /// "Main" graph implies a size of 1 and *any* team.
    /// </para>
    /// </summary>
    internal class Graph
    {
        public IList<Team> SupportedTeams { get; } = new List<Team>();
        public IList<PathfindingSize> SupportedSizes { get; } = new List<PathfindingSize>();

        private Configuration Config { get; set; } = new Configuration();

        private class PointById : Dictionary<int, Point> { }

        private class PointByIdBySize : Dictionary<PathfindingSize, PointById> { }

        private Dictionary<Team, PointByIdBySize> PointByIdBySizeByTeam { get; } = new Dictionary<Team, PointByIdBySize>();

        private class PointIdByPosition : Dictionary<(Vector2<int>, bool), int> { }

        private class PointIdByPositionBySize : Dictionary<PathfindingSize, PointIdByPosition> { }

        private Dictionary<Team, PointIdByPositionBySize> PointIdByPositionBySizeByTeam { get; } = new Dictionary<Team, PointIdByPositionBySize>();

        private class DijkstraMapBySize : Dictionary<PathfindingSize, DijkstraMap> { }

        private Dictionary<Team, DijkstraMapBySize> DijkstraMapBySizeByTeam { get; } = new Dictionary<Team, DijkstraMapBySize>();

        private DijkstraMap MainDijkstraMap(Team team) => DijkstraMapBySizeByTeam[team][1];

        private Dictionary<PathfindingSize, DijkstraMap> NonMainDijkstraMaps(Team team) => DijkstraMapBySizeByTeam[team]
            .OrderBy(pair => pair.Key).Skip(1).ToDictionary(pair => pair.Key, pair => pair.Value);

        #region Initialization

        public void Initialize(Configuration configuration)
        {
            Config = configuration;

            Config.TerrainWeights[Config.ImpassableIndex] = float.PositiveInfinity;
            Config.TerrainWeights[Config.HighGroundIndex] = 1.0f;

            for (var team = 1; team <= Config.MaxNumberOfTeams.Value; team++)
            {
                SupportedTeams.Add(team);
            }
            
            for (var size = 1; size <= Config.MaxSizeForPathfinding.Value; size++)
            {
                SupportedSizes.Add(size);
            }

            foreach (var team in SupportedTeams)
            {
                PointByIdBySizeByTeam[team] = new PointByIdBySize();
                PointIdByPositionBySizeByTeam[team] = new PointIdByPositionBySize();
                DijkstraMapBySizeByTeam[team] = new DijkstraMapBySize();
                
                foreach (var size in SupportedSizes)
                {
                    PointByIdBySizeByTeam[team][size] = new PointById();
                    PointIdByPositionBySizeByTeam[team][size] = new PointIdByPosition();

                    var dijkstraMap = new DijkstraMap();
                    DijkstraMapBySizeByTeam[team][size] = dijkstraMap;
                }
            }
        }

        /// <summary>
        /// Creates a square grid of points for all teams and entity sizes, and returns all points and their IDs (which
        /// are same across all graphs for all teams and entity sizes). Uses <see cref="Configuration.BaseTerrain"/>
        /// to set all the points terrain to initially.
        /// </summary>
        public Dictionary<Vector2<int>, int> InitializeSquareGrid()
        {
            var pointIdByPositionResult = new Dictionary<Vector2<int>, int>();
            var terrainIndex = Config.BaseTerrain;
            
            foreach (var team in SupportedTeams)
            {
                var mainDijkstraMap = MainDijkstraMap(team);
                var graph = mainDijkstraMap.AddSquareGrid(
                    Config.MapSize.X,
                    Config.MapSize.Y,
                    initialOffset: new Vector2<int>(),
                    defaultTerrain: terrainIndex,
                    orthogonalCost: 1.0f,
                    diagonalCost: Config.DiagonalCost);
                pointIdByPositionResult = SetBasePoints(team, 1, graph, terrainIndex);

                foreach (var nonMainDijkstraMap in NonMainDijkstraMaps(team))
                {
                    nonMainDijkstraMap.Value.DuplicateGraphFrom(mainDijkstraMap);
                    SetBasePoints(team, nonMainDijkstraMap.Key, graph, terrainIndex);
                }
            }

            return pointIdByPositionResult;
        }

        private Dictionary<Vector2<int>, int> SetBasePoints(Team team, PathfindingSize size,
            Dictionary<Vector2<int>, PointId> graph, int basePointTerrainIndex)
        {
            var pointIdByPosition = new Dictionary<Vector2<int>, int>();

            foreach (var (position, pointId) in graph)
            {
                var id = pointId.Value;
                var point = new Point
                {
                    Id = id,
                    Position = position,
                    IsHighGround = false,
                    HighGroundAscensionLevel = 0,
                    OriginalTerrainIndex = basePointTerrainIndex,
                    IsImpassable = false,
                    Configuration = Config
                };

                PointByIdBySizeByTeam[team][size][id] = point;
                PointIdByPositionBySizeByTeam[team][size][(position, false)] = id;
                pointIdByPosition[position] = id;
            }

            return pointIdByPosition;
        }

        #endregion

        public bool IsSupported(Team team, PathfindingSize size) 
            => SupportedTeams.Contains(team) && SupportedSizes.Contains(size);

        #region Terrain

        public void SetTerrainForPoint(Vector2<int> coordinates, Terrain terrainIndex, 
            Vector2<int> lowerBounds, Vector2<int> upperBounds, bool resetImpassable = false)
        {
            if (TryGetPointId(coordinates, 1, 1, out _) is false)
                return;

            foreach (var team in SupportedTeams)
            {
                foreach (var size in SupportedSizes)
                {
                    var from = coordinates - Vector2Int.One * size.Value + Vector2Int.One;
                    var to = coordinates + Vector2Int.One;
                    var dilatedPositions = IterateVector2Int.Positions(
                        from.IsInBoundsOf(lowerBounds, upperBounds) 
                            ? from 
                            : lowerBounds,
                        to.IsInBoundsOf(lowerBounds, upperBounds) 
                            ? to 
                            : upperBounds);
                    
                    foreach (var position in dilatedPositions)
                    {
                        if (TryGetPointId(position, team, size, out var pointId) is false)
                            continue;

                        var point = GetPoint(pointId, team, size);
                        
                        var isOnBoundary = position.X == Config.MapSize.X - size.Value + 1
                                           || position.Y == Config.MapSize.Y - size.Value + 1;
                        point.OriginalTerrainIndex = isOnBoundary 
                            ? Config.ImpassableIndex 
                            : terrainIndex.Value;
                        
                        if (resetImpassable)
                            point.IsImpassable = false;
                        
                        DijkstraMapBySizeByTeam[team][size].SetTerrainForPoint(
                            point.Id, point.OriginalTerrainIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Sets points as impassable for all <see cref="SupportedTeams"/> and <see cref="SupportedSizes"/>.
        /// </summary>
        public void SetPointsImpassable(bool to, IEnumerable<Point> points)
        {
            foreach (var originPoint in points)
            {
                foreach (var team in SupportedTeams)
                {
                    foreach (var size in SupportedSizes)
                    {
                        var dilatedPositions = IterateVector2Int.Positions(
                            originPoint.Position - Vector2Int.One * size.Value + Vector2Int.One,
                            originPoint.Position + Vector2Int.One);

                        foreach (var position in dilatedPositions)
                        {
                            if (TryGetPointId(position, team, size, out var pointId) is false)
                                continue;

                            var point = GetPoint(pointId, team, size);
                            SetPointImpassable(to, pointId, team, size);
                        }
                    }
                }
            }
        }

        public void SetPointImpassable(bool to, int pointId, Team team, PathfindingSize size)
        {
            var point = GetPoint(pointId, team, size);
            point.IsImpassable = to;
            DijkstraMapBySizeByTeam[team][size].SetTerrainForPoint(point.Id, point.CalculatedTerrainIndex);
        }

        public void UpdateDiagonalConnection(Vector2<int> position)
        {
            var diagonalPosition = position + Vector2Int.One;
            if (diagonalPosition.IsInBoundsOf(Config.MapSize) is false)
                return;

            var rightPosition = position + Vector2Int.Right;
            var bottomPosition = position + Vector2Int.Down;
            
            foreach (var team in SupportedTeams)
            {
                foreach (var size in SupportedSizes)
                {
                    var point = GetHighestPoint(position, team, size);
                    var rightNeighbour = GetHighestPoint(rightPosition, team, size);
                    var bottomNeighbour = GetHighestPoint(bottomPosition, team, size);
                    var diagonalNeighbour = GetHighestPoint(diagonalPosition, team, size);

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

        private void ApplyDiagonalConnectionsForPoints(Point point, Point diagonalNeighbour, Point rightNeighbour,
            Point bottomNeighbour, Team team, PathfindingSize size)
        {
            // .x
            // x.
            if (PointsShouldBeDiagonallyConnected(point, diagonalNeighbour,
                    rightNeighbour, bottomNeighbour, 
                    team, size))
            {
                ConnectPoints(point, diagonalNeighbour, team, size);
            }
            else
            {
                DijkstraMapBySizeByTeam[team][size].RemoveConnection(
                    point.Id,
                    diagonalNeighbour.Id);
            }

            if (PointsShouldBeDiagonallyConnected(
                    rightNeighbour, bottomNeighbour,
                    point, diagonalNeighbour,
                    team, size))
            {
                ConnectPoints(rightNeighbour, bottomNeighbour, team, size);
            }
            else
            {
                DijkstraMapBySizeByTeam[team][size].RemoveConnection(
                    rightNeighbour.Id,
                    bottomNeighbour.Id);
            }
        }

        /// <summary>
        /// Returns true if <see cref="pointA"/> and <see cref="pointB"/> should be diagonally connected, when checked
        /// against the cross points, in a pattern like so (. - point, x - cross point):
        /// <code>
        /// .x
        /// x.
        /// </code>
        /// Note: this method assumes that all given points and cross points are the highest possible points (high
        /// ground, if possible). 
        /// </summary>
        private bool PointsShouldBeDiagonallyConnected(Point pointA, Point pointB, 
            Point crossPointA, Point crossPointB, Team team, PathfindingSize size)
        {
            if (crossPointA.IsImpassable && crossPointB.IsImpassable)
                return false;

            if (pointA.IsHighGround && pointB.IsHighGround)
            {
                if (crossPointA.IsLowGround
                    && crossPointB.IsLowGround
                    && HighGroundPointHasLowGroundConnections(pointA, team, size) is false
                    && HighGroundPointHasLowGroundConnections(pointB, team, size) is false)
                    return false;

                if ((crossPointA.IsLowGround && crossPointB.IsHighGround && crossPointB.IsImpassable)
                    || (crossPointB.IsLowGround && crossPointA.IsHighGround && crossPointA.IsImpassable))
                    return false;
            }

            if (pointA.IsHighGround && pointB.IsLowGround)
            {
                if (HighGroundPointHasLowGroundConnections(pointA, team, size) is false)
                    return false;
            }

            if (pointA.IsLowGround && pointB.IsHighGround)
            {
                if (HighGroundPointHasLowGroundConnections(pointB, team, size) is false)
                    return false;
            }

            return true; 
        }

        public bool HighGroundPointHasLowGroundConnections(Point point, Team team, PathfindingSize size)
        {
            if (point.IsLowGround)
                return false;

            var result = point.Position.AdjacentPositions()
                .Where(adjacentPosition => ContainsPoint(adjacentPosition, team, size, false))
                .Select(adjacentPosition => GetPoint(adjacentPosition, team, size, false))
                .Any(adjacentPoint => HasConnection(point, adjacentPoint, team, size));
            
            return result;
        }

        #endregion

        #region Points

        /// <summary>
        /// Gets all terrain (low ground) points from the main (1x1) graph. Can be used to get actual terrain of a point.
        /// Terrain is the same for all teams.
        /// </summary>
        public IEnumerable<Point> GetTerrainPoints() => PointByIdBySizeByTeam[1][1].Values.Where(x =>
            x.IsLowGround);

        /// <summary>
        /// Returned <see cref="Point"/> can be null.
        /// </summary>
        public Point GetTerrainPoint(int id)
        {
            var point = GetPoint(id, 1, 1);
            if (point.IsHighGround)
                return null;
            return point;
        }

        /// <summary>
        /// Resets the points to the initial terrain and adds connections to all points for all pathfinding sizes and
        /// teams. Removes high ground points in the process.
        /// </summary>
        public void ResetToTerrainPoints(Vector2<int> lowerBounds, Vector2<int> upperBounds)
        {
            var positionsByTerrain = new Dictionary<Terrain, IList<Vector2<int>>>();
            foreach (var terrain in Config.TerrainsInAscendingWeights)
            {
                positionsByTerrain[terrain] = new List<Vector2<int>>();
            }
            
            foreach (var position in IterateVector2Int.Positions(lowerBounds, upperBounds))
            {
                var highGroundPointExists = TryGetMainPointId(position, 
                    out var highGroundPointId, true);
                if (highGroundPointExists)
                    RemoveAllPoints(highGroundPointId);

                var lowGroundPointExists = TryGetMainPointId(position, 
                    out var lowGroundPointId, false);
                if (lowGroundPointExists is false)
                    continue;

                var point = GetMainPoint(lowGroundPointId);
                positionsByTerrain[point.OriginalTerrainIndex].Add(point.Position);

                foreach (var adjacentPosition in point.Position.AdjacentPositions())
                {
                    var adjacentPointExists = TryGetMainPointId(adjacentPosition, 
                        out var adjacentPointId, false);
                    if (adjacentPointExists is false)
                        continue;

                    var adjacentPoint = GetMainPoint(adjacentPointId);
                    ConnectPoints(point, adjacentPoint);
                }
            }

            foreach (var terrain in Config.TerrainsInAscendingWeights)
            {
                foreach (var position in positionsByTerrain[terrain])
                {
                    SetTerrainForPoint(position, terrain, 
                        lowerBounds, upperBounds, true);
                }
            }
        }

        public Point GetPoint(int id, Team team, PathfindingSize size) => PointByIdBySizeByTeam[team][size][id];

        public Point GetPoint(Vector2<int> position, Team team, PathfindingSize size, bool isHighGround = false)
            => GetPoint(GetPointId(position, team, size, isHighGround), team, size);

        public Point GetMainPoint(Vector2<int> position, bool isHighGround = false, Team? team = null)
            => GetPoint(position, team ?? new Team(1), 1, isHighGround);

        public Point GetMainPoint(int id, Team? team = null) => GetPoint(id, team ?? new Team(1), 1);

        /// <summary>
        /// Will throw exception if invalid position, team or size is inputted, these should be validated
        /// before calling this endpoint.
        /// </summary>
        public Point GetHighestPoint(Vector2<int> position, Team team, PathfindingSize size)
        {
            if (ContainsPoint(position, team, size, true))
            {
                return GetPoint(position, team, size, true);
            }

            return GetPoint(position, team, size, false);
        }

        public bool ContainsMainPoint(Vector2<int> position, bool isHighGround = false, Team? team = null)
            => ContainsPoint(position, team ?? new Team(1), 1, isHighGround);

        public bool ContainsPoint(Vector2<int> position, Team team, PathfindingSize size, bool isHighGround = false)
            => PointIdByPositionBySizeByTeam[team][size].ContainsKey((position, isHighGround));

        public bool TryGetPointId(Vector2<int> position, Team team, PathfindingSize size, out int id,
            bool isHighGround = false)
        {
            id = -1;

            if (SupportedTeams.Contains(team) is false || SupportedSizes.Contains(size) is false)
                return false;

            var result = PointIdByPositionBySizeByTeam[team][size].TryGetValue((position, isHighGround), out var idResult);

            id = idResult;
            return result;
        }

        public int GetPointId(Vector2<int> position, Team team, PathfindingSize size, bool isHighGround = false)
            => PointIdByPositionBySizeByTeam[team][size][(position, isHighGround)];

        public int GetMainPointId(Vector2<int> position, bool isHighGround = false, Team? team = null)
            => GetPointId(position, team ?? new Team(1), 1, isHighGround);

        public bool TryGetMainPointId(Vector2<int> position, out int id, bool isHighGround = false, Team? team = null) 
            => TryGetPointId(position, team ?? new Team(1), 1, out id, isHighGround);

        public Point AddPoint(Vector2<int> position, bool isHighGround, int highGroundAscensionLevel,
            int? terrainIndex = null)
        {
            foreach (var team in SupportedTeams)
            {
                foreach (var size in SupportedSizes)
                {
                    var originalTerrainIndex = isHighGround 
                        ? Config.HighGroundIndex 
                        : terrainIndex ?? Config.ImpassableIndex;
                    var point = new Point
                    {
                        Id = GetNewPointId(position.X, position.Y, isHighGround),
                        Position = position,
                        IsHighGround = isHighGround,
                        HighGroundAscensionLevel = highGroundAscensionLevel,
                        OriginalTerrainIndex = originalTerrainIndex,
                        IsImpassable = false,
                        Configuration = Config
                    };
                    
                    PointByIdBySizeByTeam[team][size][point.Id] = point;
                    PointIdByPositionBySizeByTeam[team][size][(point.Position, point.IsHighGround)] = point.Id;

                    DijkstraMapBySizeByTeam[team][size].AddPoint(point.Id, point.CalculatedTerrainIndex);
                }
            }

            return GetMainPoint(position, isHighGround);
        }

        private int GetNewPointId(int x, int y, bool isHighGround)
        {
            var offset = isHighGround 
                ? Config.MapSize.X * Config.MapSize.Y 
                : 0;
            return offset + x * Config.MapSize.Y + y + 1;
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
        public void RemovePoint(int id, PathfindingSize size)
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
        public void RemovePoint(int id, Team team, PathfindingSize size)
        {
            var point = PointByIdBySizeByTeam[team][size][id];

            PointByIdBySizeByTeam[team][size].Remove(point.Id);
            PointIdByPositionBySizeByTeam[team][size].Remove((point.Position, point.IsHighGround));

            DijkstraMapBySizeByTeam[team][size].RemovePoint(point.Id);
        }

        #endregion

        #region Path

        public void Recalculate(Vector2<int> from, float range, bool isOnHighGround, Team team, PathfindingSize size)
        {
            var point = GetPoint(from, team, size, isOnHighGround);
            Recalculate(point.Id, range, team, size);
        }

        public void Recalculate(int pointId, float range, Team team, PathfindingSize size)
        {
            DijkstraMapBySizeByTeam[team][size].Recalculate(pointId,
                maximumCost: range,
                terrainWeights: Config.TerrainWeights);
        }

        public IEnumerable<Point> GetAllPointsWithCostBetween(float min, float max, Team team, PathfindingSize size)
        {
            var pointIds = DijkstraMapBySizeByTeam[team][size].GetAllPointsWithCostBetween(min, max);
            var points = pointIds.Select(id => GetPoint(id.Value, team, size));
            return points;
        }

        public IEnumerable<Point> GetShortestPathFromPoint(int pointId, Team team, PathfindingSize size)
        {
            var pointIds = DijkstraMapBySizeByTeam[team][size].GetShortestPathFromPoint(pointId);
            var points = pointIds.Select(id => GetPoint(id.Value, team, size));
            return points;
        }

        #endregion

        #region Connections

        public bool HasConnection(Point pointA, Point pointB, Team team, PathfindingSize size)
            => DijkstraMapBySizeByTeam[team][size].HasConnection(pointA.Id, pointB.Id);

        public bool HasMainConnection(Point pointA, Point pointB, Team? team = null)
            => HasConnection(pointA, pointB, team ?? new Team(1), 1);
        
        /// <summary>
        /// Connects points for all <see cref="SupportedSizes"/> and <see cref="SupportedTeams"/>.
        /// </summary>
        public void ConnectPoints(Point pointA, Point pointB)
        {
            foreach (var size in SupportedSizes)
            {
                ConnectPoints(pointA, pointB, size);
            }
        }
        
        /// <summary>
        /// Connects points for all <see cref="SupportedSizes"/> and given <see cref="Team"/>.
        /// </summary>
        public void ConnectPoints(Point pointA, Point pointB, Team team)
        {
            foreach (var size in SupportedSizes)
            {
                ConnectPoints(pointA, pointB, team, size);
            }
        }
        
        /// <summary>
        /// Connects points for all <see cref="SupportedTeams"/>.
        /// </summary>
        public void ConnectPoints(Point pointA, Point pointB, PathfindingSize size)
        {
            foreach (var team in SupportedTeams)
            {
                ConnectPoints(pointA, pointB, team, size);
            }
        }

        /// <summary>
        /// Connects points for specified <see cref="PathfindingSize"/> and <see cref="Team"/>.
        /// </summary>
        public void ConnectPoints(Point pointA, Point pointB, Team team, PathfindingSize size)
        {
            var isDiagonallyAdjacent = pointA.Position.IsDiagonalTo(pointB.Position);
            DijkstraMapBySizeByTeam[team][size].ConnectPoints(
                pointA.Id,
                pointB.Id,
                isDiagonallyAdjacent ? Config.DiagonalCost : 1f);
        }

        #endregion
    }
}