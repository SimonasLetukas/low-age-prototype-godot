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

        private int PointIdIterator { get; set; }

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

            PointIdIterator = Config.MapSize.X * Config.MapSize.Y;

            Config.TerrainWeights.Add(Config.ImpassableIndex, float.PositiveInfinity);
            Config.TerrainWeights.Add(Config.HighGroundIndex, 1.0f);

            for (var team = 1; team <= Config.MaxNumberOfTeams.Value; team++)
            {
                SupportedTeams.Add(team);

                PointByIdBySizeByTeam[team] = new PointByIdBySize();
                PointIdByPositionBySizeByTeam[team] = new PointIdByPositionBySize();
                DijkstraMapBySizeByTeam[team] = new DijkstraMapBySize();

                for (var size = 1; size <= Config.MaxSizeForPathfinding.Value; size++)
                {
                    SupportedSizes.Add(size);

                    PointByIdBySizeByTeam[team][size] = new PointById();
                    PointIdByPositionBySizeByTeam[team][size] = new PointIdByPosition();

                    var dijkstraMap = new DijkstraMap();
                    DijkstraMapBySizeByTeam[team][size] = dijkstraMap;
                }
            }
        }

        /// <summary>
        /// Creates a square grid of points for all teams and entity sizes, and returns all points and their IDs (which
        /// are same across all graphs for all teams and entity sizes).
        /// </summary>
        /// <param name="terrainIndex">Base terrain index to set to all the points initially.</param>
        public Dictionary<Vector2<int>, int> InitializeSquareGrid(int terrainIndex)
        {
            var pointIdByPositionResult = new Dictionary<Vector2<int>, int>();

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

        public void SetTerrainForPoint(Vector2<int> coordinates, int terrainIndex)
        {
            if (TryGetPointId(coordinates, 1, 1, out _) is false)
                return;

            foreach (var team in SupportedTeams)
            {
                foreach (var size in SupportedSizes)
                {
                    var dilatedPositions = IterateVector2Int.Positions(
                        coordinates - Vector2Int.One * size.Value + Vector2Int.One,
                        coordinates + Vector2Int.One);
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
                        var dilatedPositions = IterateVector2Int.Positions(
                            originPoint.Position - Vector2Int.One * size.Value + Vector2Int.One,
                            originPoint.Position + Vector2Int.One);

                        foreach (var position in dilatedPositions)
                        {
                            if (TryGetPointId(position, team, size, out var pointId) is false)
                                continue;

                            var point = GetPoint(pointId, team, size);
                            SetPointImpassable(to, point, team, size);
                        }
                    }
                }
            }
        }

        public void SetPointImpassable(bool to, Point point, Team team, PathfindingSize size)
        {
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
            Point bottomNeighbour, Team team, PathfindingSize size)
        {
            // x.
            // .x
            if (point.IsImpassable
                && diagonalNeighbour.IsImpassable
                && rightNeighbour.IsImpassable is false
                && bottomNeighbour.IsImpassable is false)
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
                    Config.DiagonalCost);
            }

            // .x
            // x.
            if (point.IsImpassable is false
                && diagonalNeighbour.IsImpassable is false
                && rightNeighbour.IsImpassable
                && bottomNeighbour.IsImpassable)
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
                    Config.DiagonalCost);
            }
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

        public Point GetPoint(int id, Team team, PathfindingSize size) => PointByIdBySizeByTeam[team][size][id];

        public Point GetPoint(Vector2<int> position, Team team, PathfindingSize size, bool isHighGround = false)
            => GetPoint(GetPointId(position, team, size, isHighGround), team, size);

        public Point GetMainPoint(Vector2<int> position, bool isHighGround = false, Team? team = null)
            => GetPoint(position, team ?? new Team(1), 1, isHighGround);

        public bool TryGetHighestPoint(Vector2<int> position, Team team, PathfindingSize size,
            bool lookingFromHighGround,
            out Point point)
        {
            point = new Point();
            var result = GetHighestPoint(position, team, size, lookingFromHighGround);
            if (result is null)
                return false;

            point = result.Value;
            return true;
        }

        public Point? GetHighestPoint(Vector2<int> position, Team team, PathfindingSize size,
            bool lookingFromHighGround)
        {
            var highGroundPointFound = TryGetPointId(position, team, size, out var highGroundPointId, true);
            var lowGroundPointFound = TryGetPointId(position, team, size, out var lowGroundPointId);

            if (highGroundPointFound && lookingFromHighGround)
                return GetPoint(highGroundPointId, team, size);

            if (lowGroundPointFound is false)
                return null;

            return GetPoint(lowGroundPointId, team, size);
        }

        public bool ContainsMainPoint(Vector2<int> position, bool isHighGround = false, Team? team = null)
            => ContainsPoint(position, team ?? new Team(1), 1, isHighGround);

        public bool ContainsPoint(Vector2<int> position, Team team, PathfindingSize size, bool isHighGround = false)
            => PointIdByPositionBySizeByTeam[team][size].ContainsKey((position, isHighGround));

        public bool TryGetPointId(Vector2<int> position, Team team, PathfindingSize size, out int id,
            bool isHighGround = false)
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

        public int GetPointId(Vector2<int> position, Team team, PathfindingSize size, bool isHighGround = false)
            => PointIdByPositionBySizeByTeam[team][size][(position, isHighGround)];

        public int GetMainPointId(Vector2<int> position, bool isHighGround = false, Team? team = null)
            => GetPointId(position, team ?? new Team(1), 1, isHighGround);

        public Point AddPoint(Vector2<int> position, bool isHighGround, int highGroundAscensionLevel, int ySpriteOffset,
            int? terrainIndex = null)
        {
            var originalTerrainIndex = terrainIndex is null
                ? Config.ImpassableIndex
                : isHighGround
                    ? Config.HighGroundIndex
                    : terrainIndex.Value;
            var point = new Point
            {
                Id = PointIdIterator++,
                Position = position,
                IsHighGround = isHighGround,
                HighGroundAscensionLevel = highGroundAscensionLevel,
                YSpriteOffset = ySpriteOffset,
                OriginalTerrainIndex = originalTerrainIndex,
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

        public void ConnectPoints(Point pointA, Point pointB, bool isDiagonallyAdjacent, PathfindingSize size)
        {
            foreach (var team in SupportedTeams)
            {
                DijkstraMapBySizeByTeam[team][size].ConnectPoints(
                    pointA.Id,
                    pointB.Id,
                    isDiagonallyAdjacent ? Config.DiagonalCost : 1f);
            }
        }

        #endregion
    }
}