using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using low_age_dijkstra;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Quadtree;
using Newtonsoft.Json;

namespace multipurpose_pathfinding
{
    #region Interface
    
    /// <summary>
    /// Multipurpose pathfinding features:
    /// <list type="bullet">
    /// <item>
    /// Easy setup of a square grid filled with graph nodes and their connections;
    /// </item>
    /// <item>
    /// Algorithms to calculate available points and paths;
    /// </item>
    /// <item>
    /// Adding occupation: places where pathing is temporarily blocked;
    /// </item>
    /// <item>
    /// Support of different pathfinding graphs depending on the teams (useful when allies should be able to path
    /// through each other, while pathing through enemies is blocked);
    /// </item>
    /// <item>
    /// Support of pathfinding for MxM sized entities;
    /// </item>
    /// <item>
    /// Support for setting up a second layer of pathfinding on a high-ground and navigating between the different
    /// ground levels;
    /// </item>
    /// <item>
    /// Automatically keeping track of the interactions between occupation, high-grounds, sizes, teams.
    /// </item>
    /// </list>
    /// </summary>
    public interface IMultipurposePathfinding
    {
        /// <summary>
        /// Invoked when the pathfinding has finished initializing.
        /// </summary>
        event Action FinishedInitializing;

        /// <summary>
        /// Invoked when there's a new point added to the pathfinding graph. 
        /// </summary>
        event Action<Point> PointAdded;

        /// <summary>
        /// Invoked when there's an existing point removed from the pathfinding graph. 
        /// </summary>
        event Action<Point> PointRemoved;

        /// <summary>
        /// Initializes the pathfinding graph and prepares it for iterations. <see cref="IterateInitialization"/>
        /// should be called afterward, until the pathfinding graph has <see cref="FinishedInitializing"/>.
        /// </summary>
        /// <param name="initialPositionsAndTerrainIndexes">Positions and their terrain indexes to initialize.</param>
        /// <param name="configuration">Configuration of the pathfinding graph. If null, default values will be
        /// used.</param>
        void Initialize(IEnumerable<(Vector2<int>, Terrain)> initialPositionsAndTerrainIndexes,
            Configuration configuration = null);

        /// <summary>
        /// Runs the next iterations for initializing the pathfinding graph. Invokes the event
        /// <see cref="FinishedInitializing"/> when done.
        /// </summary>
        /// <param name="deltaTime">Time in seconds that the next initialization iteration should run for.</param>
        void IterateInitialization(float deltaTime);

        /// <summary>
        /// Recalculates the graph and returns all available points within range. 
        /// </summary>
        /// <param name="from">Position from which all available points are calculated.</param>
        /// <param name="range">Maximum cost of movement allowed to be searched.</param>
        /// <param name="lookingFromHighGround">Should be true if the '<see cref="from"/>' position is on the
        /// high-ground.</param>
        /// <param name="team">Which team is pathfinding.</param>
        /// <param name="pathfindingSize">What is the MxM size of the pathfinding entity.</param>
        /// <param name="temporary">If true, the calculation will not be cached. False by default.</param>
        /// <returns>All points available for pathfinding.</returns>
        IEnumerable<Point> GetAvailablePoints(Vector2<int> from, float range, bool lookingFromHighGround,
            Team team, PathfindingSize pathfindingSize, bool temporary = false);

        /// <summary>
        /// Clears the cached calculation that was done during <see cref="GetAvailablePoints"/>.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Returns the shortest path from the point used to <see cref="GetAvailablePoints"/> to the given point.
        /// </summary>
        /// <param name="to">Destination point.</param>
        /// <param name="team">Which team is pathfinding.</param>
        /// <param name="pathfindingSize">What is the MxM size of the pathfinding entity.</param>
        /// <returns>Points along the shortest path.</returns>
        IEnumerable<Point> FindPath(Point to, Team team, PathfindingSize pathfindingSize);

        /// <summary>
        /// Returns true if two points have a connection between them.
        /// </summary>
        /// <param name="pointA">Point A.</param>
        /// <param name="pointB">Point B.</param>
        /// <param name="team">Which team is pathfinding.</param>
        /// <param name="pathfindingSize">What is the MxM size of the pathfinding entity.</param>
        /// <returns>True if the two points have a connection.</returns>
        bool HasConnection(Point pointA, Point pointB, Team team, PathfindingSize pathfindingSize);

        /// <summary>
        /// Gets all terrain (low ground) points from the main (1x1) graph. Can be used to get actual terrain of a
        /// point. Terrain is the same for all teams.
        /// </summary>
        IEnumerable<Point> GetTerrainPoints();

        /// <summary>
        /// Adds or updates an existing entity which helps to keep track of all the different pathfinding elements
        /// like occupation and high-ground.
        /// </summary>
        void AddOrUpdateEntity(PathfindingEntity entity);

        /// <summary>
        /// Removes entity from the pathfinding, then updates the pathfinding graph.
        /// </summary>
        void RemoveEntity(Guid entityId);

        /// <summary>
        /// Adds <see cref="PathfindingEntity"/> <see cref="PathfindingEntity.Bounds"/> to the pathfinding
        /// calculations, then updates the pathfinding graph.
        /// </summary>
        void AddOccupation(Guid entityId);

        /// <summary>
        /// Removes <see cref="PathfindingEntity"/> <see cref="PathfindingEntity.Bounds"/> from the pathfinding
        /// calculations, then updates the pathfinding graph.
        /// </summary>
        void RemoveOccupation(Guid entityId);

        /// <summary>
        /// Adds a path of ascendable high-ground to the pathfinding calculations for the given entity, then
        /// updates the pathfinding graph.
        /// </summary>
        /// <param name="entityId">Reference to the entity for which the ascendable high ground is added.</param>
        /// <param name="path">A list of ascendable steps (from lowest to highest): each step is a collection of
        /// positions and their corresponding sprite offset.</param>
        void AddAscendableHighGround(Guid entityId, IList<IEnumerable<Vector2<int>>> path);

        /// <summary>
        /// Removes the path of ascendable high-ground from the pathfinding calculations for the given entity, then
        /// updates the pathfinding graph.
        /// </summary>
        void RemoveAscendableHighGround(Guid entityId);

        /// <summary>
        /// Adds the positions of high-ground to the pathfinding calculations for the given entity, then
        /// updates the pathfinding graph.
        /// </summary>
        void AddHighGround(Guid entityId, IEnumerable<Vector2<int>> positions);

        /// <summary>
        /// Removes the positions of high-ground from the pathfinding calculations for the given entity, then
        /// updates the pathfinding graph.
        /// </summary>
        void RemoveHighGround(Guid entityId);
    }
    
    #endregion Interface

    public class MultipurposePathfinding : IMultipurposePathfinding
    {
        public event Action FinishedInitializing = delegate { };
        public event Action<Point> PointAdded = delegate { };
        public event Action<Point> PointRemoved = delegate { };

        private Configuration Config { get; set; }
        private Graph Graph { get; } = new Graph();

        #region Initialization

        private bool _initialized = true;
        private bool _terrainGraphFirstPassInitialized = false;
        private bool _terrainGraphFurtherPassesInitialized = false;
        private bool _diagonalConnectionsInitialized = false;
        private Terrain _terrainWithSmallestMovementCost;
        private IList<(Vector2<int>, Terrain)> _positionsAndTerrainsForFirstPassInitialization;
        private Dictionary<Terrain, IList<Vector2<int>>> _positionsByTerrainForFurtherInitialization;
        private Dictionary<Vector2<int>, int> _pointIdsByPositionsForInitialization;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void Initialize(IEnumerable<(Vector2<int>, Terrain)> initialPositionsAndTerrainIndexes,
            Configuration configuration = null)
        {
            Config = configuration ?? new Configuration();
            Graph.Initialize(configuration);

            var terrainsWithAscendingMovementCost = Config.TerrainWeights
                .OrderBy(x => x.Value)
                .Select(x => new Terrain(x.Key))
                .ToList();
            _terrainWithSmallestMovementCost = terrainsWithAscendingMovementCost.First();
            _positionsByTerrainForFurtherInitialization = new Dictionary<Terrain, IList<Vector2<int>>>();
            foreach (var terrain in terrainsWithAscendingMovementCost
                         .Where(terrain => terrain.Equals(_terrainWithSmallestMovementCost) is false))
            {
                _positionsByTerrainForFurtherInitialization[terrain] = new List<Vector2<int>>();
            }

            _positionsAndTerrainsForFirstPassInitialization = initialPositionsAndTerrainIndexes.ToList();
            _pointIdsByPositionsForInitialization = Graph.InitializeSquareGrid();

            _initialized = false;
        }

        public void IterateInitialization(float deltaTime)
        {
            if (_initialized)
                return;

            CheckInitialization();

            var deltaMs = (int)(deltaTime * 1000);
            _stopwatch.Reset();
            _stopwatch.Start();

            while (_stopwatch.ElapsedMilliseconds < deltaMs
                   || (CheckInitialization() is false))
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

        private void IterateTerrainGraphFirstPassUpdate()
        {
            if (_positionsAndTerrainsForFirstPassInitialization.IsEmpty())
            {
                if (Config.DebugEnabled)
                    Console.WriteLine($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                                      $"{nameof(MultipurposePathfinding)}.{nameof(IterateTerrainGraphFirstPassUpdate)} " +
                                      $"completed");
                _terrainGraphFirstPassInitialized = true;
                TrimEmptyTerrainsForFurtherInitialization();
                return;
            }

            var entry = _positionsAndTerrainsForFirstPassInitialization[0];
            _positionsAndTerrainsForFirstPassInitialization.RemoveAt(0);
            var (coordinates, terrain) = entry;

            if (terrain.Equals(_terrainWithSmallestMovementCost))
            {
                Graph.SetTerrainForPoint(coordinates, terrain);
                var point = Graph.GetMainPoint(coordinates);
                PointAdded(point);
                return;
            }

            _positionsByTerrainForFurtherInitialization[terrain].Add(coordinates);
        }

        private void IterateTerrainGraphFurtherPassUpdate()
        {
            if (_positionsByTerrainForFurtherInitialization.IsEmpty())
            {
                if (Config.DebugEnabled)
                    Console.WriteLine($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                                      $"{nameof(MultipurposePathfinding)}.{nameof(IterateTerrainGraphFurtherPassUpdate)} " +
                                      $"completed");
                _terrainGraphFurtherPassesInitialized = true;
                return;
            }

            var terrain = FindRemainingTerrainWithSmallestMovementCost();
            if (terrain is null)
                return;

            var coordinates = _positionsByTerrainForFurtherInitialization[terrain.Value][0];
            _positionsByTerrainForFurtherInitialization[terrain.Value].RemoveAt(0);
            TrimEmptyTerrainsForFurtherInitialization();

            Graph.SetTerrainForPoint(coordinates, terrain.Value);
            var point = Graph.GetMainPoint(coordinates);
            PointAdded(point);
        }

        private void IterateDiagonalConnectionUpdate()
        {
            if (_pointIdsByPositionsForInitialization.IsEmpty())
            {
                if (Config.DebugEnabled)
                    Console.WriteLine($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                                      $"{nameof(MultipurposePathfinding)}.{nameof(IterateDiagonalConnectionUpdate)} " +
                                      $"completed");
                _diagonalConnectionsInitialized = true;
                return;
            }

            var position = _pointIdsByPositionsForInitialization.First().Key;
            _pointIdsByPositionsForInitialization.Remove(position);

            Graph.UpdateDiagonalConnection(position);
        }

        private Terrain? FindRemainingTerrainWithSmallestMovementCost() =>
            Config.TerrainsInAscendingWeights.FirstOrDefault(nextTerrain =>
                _positionsByTerrainForFurtherInitialization.ContainsKey(nextTerrain));

        private void TrimEmptyTerrainsForFurtherInitialization()
        {
            var keysToRemove = _positionsByTerrainForFurtherInitialization
                .Where(entry => entry.Value.Count == 0)
                .Select(entry => entry.Key).ToList();

            foreach (var key in keysToRemove)
            {
                _positionsByTerrainForFurtherInitialization.Remove(key);
            }
        }

        #endregion Initialization

        #region Cache

        private Vector2<int> _previousPosition = Vector2Int.Max;
        private float _previousRange = -1.0f;
        private bool _previousLookingFromHighGround = false;
        private Team _previousTeam = 1;
        private PathfindingSize _previousSize = 1;

        public void ClearCache() => SetCache(Vector2Int.Max, -1.0f, false, 1, 1);

        private bool IsCached(Vector2<int> position, float range, bool lookingFromHighGround, Team team,
            PathfindingSize size)
            => position.Equals(_previousPosition)
               && range.Equals(_previousRange)
               && lookingFromHighGround.Equals(_previousLookingFromHighGround)
               && team.Equals(_previousTeam)
               && size.Equals(_previousSize);

        private void SetCache(Vector2<int> position, float range, bool lookingFromHighGround, Team team,
            PathfindingSize size)
        {
            _previousPosition = position;
            _previousRange = range;
            _previousLookingFromHighGround = lookingFromHighGround;
            _previousTeam = team;
            _previousSize = size;
        }

        #endregion Cache

        #region Getters

        public IEnumerable<Point> GetAvailablePoints(Vector2<int> from, float range, bool lookingFromHighGround,
            Team team, PathfindingSize pathfindingSize, bool temporary = false)
        {
            if (pathfindingSize > Config.MaxSizeForPathfinding)
                throw new ArgumentException($"{nameof(MultipurposePathfinding)}." +
                                            $"{nameof(GetAvailablePoints)}: argument " +
                                            $"{nameof(pathfindingSize)} '{pathfindingSize}' exceeded maximum " +
                                            $"value of '{Config.MaxSizeForPathfinding}'.");

            if (Graph.IsSupported(team, pathfindingSize) is false)
                return Enumerable.Empty<Point>();

            if (IsCached(from, range, lookingFromHighGround, team, pathfindingSize) is false)
            {
                Graph.Recalculate(from, range, lookingFromHighGround, team, pathfindingSize);

                if (temporary is false)
                    SetCache(from, range, lookingFromHighGround, team, pathfindingSize);
            }

            var availablePositions = Graph.GetAllPointsWithCostBetween(0, range, team, pathfindingSize);

            if (temporary && team.Equals(_previousTeam)
                          && pathfindingSize.Equals(_previousSize)
                          && Graph.TryGetPointId(_previousPosition, team, pathfindingSize, out var previousPointId,
                              _previousLookingFromHighGround))
            {
                Graph.Recalculate(previousPointId, _previousRange, _previousTeam, _previousSize);
            }

            return availablePositions;
        }

        public IEnumerable<Point> FindPath(Point to, Team team, PathfindingSize pathfindingSize)
        {
            if (to.Position.IsInBoundsOf(Config.MapSize) is false
                || Graph.IsSupported(team, pathfindingSize) is false)
                return Enumerable.Empty<Point>();

            var points = Graph.GetShortestPathFromPoint(to.Id, team, pathfindingSize);

            var path = new List<Point> { Graph.GetPoint(to.Id, team, pathfindingSize) };
            path.AddRange(points);
            path.Reverse();

            return path;
        }

        public bool HasConnection(Point pointA, Point pointB, Team team, PathfindingSize pathfindingSize)
        {
            if (Graph.IsSupported(team, pathfindingSize) is false)
                return false;

            return Graph.HasConnection(pointA, pointB, team, pathfindingSize);
        }

        public IEnumerable<Point> GetTerrainPoints() => Graph.GetTerrainPoints();

        #endregion Getters

        #region Setters
        
        private class PipelineItem
        {
            public bool HasAscendableHighGround => AscendablePath.Any();

            public IList<IEnumerable<Vector2<int>>> AscendablePath { get; set; } =
                new List<IEnumerable<Vector2<int>>>();

            public void RemoveAscendablePath() => AscendablePath = new List<IEnumerable<Vector2<int>>>();
            public bool HasHighGround => HighGroundPositions.Any();
            public IEnumerable<Vector2<int>> HighGroundPositions { get; set; } = new List<Vector2<int>>();
            public void RemoveHighGroundPositions() => HighGroundPositions = new List<Vector2<int>>();
            public bool HasOccupation => OccupyingEntity.HasOccupation;
            public PathfindingEntity OccupyingEntity { get; set; }
        }
        
        private Dictionary<Guid, PipelineItem> PipelineItemsByEntityId { get; } = new Dictionary<Guid, PipelineItem>();
        private Quadtree<PathfindingEntity> EntitiesSpatialMap { get; } = new Quadtree<PathfindingEntity>();

        public void AddOrUpdateEntity(PathfindingEntity entity)
        {
            var entityIsBeingTracked = PipelineItemsByEntityId.ContainsKey(entity.Id);
            var pipelineItem = new PipelineItem();

            if (entityIsBeingTracked)
            {
                var existingEntity = PipelineItemsByEntityId[entity.Id].OccupyingEntity;
                EntitiesSpatialMap.Remove(existingEntity.Bounds, existingEntity);

                pipelineItem = PipelineItemsByEntityId[entity.Id];
            }

            pipelineItem.OccupyingEntity = entity;
            PipelineItemsByEntityId[entity.Id] = pipelineItem;
            EntitiesSpatialMap.Insert(entity.Bounds, entity);
        }

        public void RemoveEntity(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var entity = item.OccupyingEntity;
            EntitiesSpatialMap.Remove(entity.Bounds, entity);
            PipelineItemsByEntityId.Remove(entityId);
        }

        public void AddOccupation(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            RunPathfindingPipeline(item);
        }

        public void RemoveOccupation(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var entity = item.OccupyingEntity;
            var updatedEntity = PathfindingEntity.WithoutOccupation(entity);
            AddOrUpdateEntity(updatedEntity);

            RunPathfindingPipeline(item);
        }

        public void AddAscendableHighGround(Guid entityId, IList<IEnumerable<Vector2<int>>> path)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            item.AscendablePath = path;

            RunPathfindingPipeline(item);

            foreach (var section in path)
            {
                foreach (var coordinates in section)
                {
                    var point = Graph.GetMainPoint(coordinates, true);
                    PointAdded(point);
                }
            }
        }

        public void RemoveAscendableHighGround(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var path = item.AscendablePath;
            var points = path.SelectMany(section => section,
                (entry, pos) => Graph.GetMainPoint(pos, true));
            item.RemoveAscendablePath();

            RunPathfindingPipeline(item);

            foreach (var point in points)
            {
                PointRemoved(point);
            }
        }

        public void AddHighGround(Guid entityId, IEnumerable<Vector2<int>> positions)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var positionsList = positions.ToList();
            item.HighGroundPositions = positionsList;

            RunPathfindingPipeline(item);

            foreach (var position in positionsList)
            {
                var point = Graph.GetMainPoint(position, true);
                PointAdded(point);
            }
        }

        public void RemoveHighGround(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var points = item.HighGroundPositions
                .Select(pos => Graph.GetMainPoint(pos, true));
            item.RemoveHighGroundPositions();

            RunPathfindingPipeline(item);

            foreach (var point in points)
            {
                PointRemoved(point);
            }
        }

        private void RunPathfindingPipeline(PipelineItem mainItem)
        {
            // TODO there's a chance that a recursive check of all the entities would be needed to collect all
            // affected positions instead of relying on a simple area

            // TODO rethink high ground and ascendable algorithm so that points and connections are always created for
            // all sizes and teams, and control the pathfinding through occupation logic
            
            // TODO transfer unit tests
            
            // TODO add occupation calculation
            
            // TODO add diagonals calculation
            
            // TODO connect with Pathfinding.cs
            
            // Find all nearby entities
            var mainEntity = mainItem.OccupyingEntity;
            var boundsOffset = Vector2Int.One * Config.MaxSizeForPathfinding.Value;
            var lowerBounds = mainEntity.Position - boundsOffset;
            var upperBounds = mainEntity.Position + mainEntity.Size + boundsOffset;
            var foundItems = GetEntitiesIntersectingWith(lowerBounds, upperBounds)
                .Select(e => PipelineItemsByEntityId[e.Id])
                .ToList();

            Graph.ResetToTerrainPoints(lowerBounds, upperBounds);

            // For each found entity: run high ground calculations
            foreach (var item in foundItems)
            {
                RunHighGroundCalculation(item.HighGroundPositions);
            }
            
            // For each found entity: run ascendable high ground calculations
            foreach (var item in foundItems)
            {
                // TODO probably needs to take into account teams too
                RunAscendableCalculation(item.AscendablePath);
            }

            foreach (var item in foundItems)
            {
                // TODO probably needs to take into account teams too
                RunOccupationCalculation(item.OccupyingEntity);
            }

            // For each pathfinding size and team:
            foreach (var team in Graph.SupportedTeams)
            {
                foreach (var pathfindingSize in Graph.SupportedSizes)
                {
                    // For each found entity: run occupation calculations

                    // For each found entity: run diagonal calculations
                }
            }
        }

        private IEnumerable<PathfindingEntity> GetEntitiesIntersectingWith(Vector2<int> lowerBounds,
            Vector2<int> upperBounds)
        {
            var searchBounds = new Envelope(
                new Coordinate(lowerBounds.X, lowerBounds.Y),
                new Coordinate(upperBounds.X, upperBounds.Y));
            return EntitiesSpatialMap
                .Query(searchBounds)
                .Where(x => x.Bounds.Intersects(searchBounds));
        }

        private void RunHighGroundCalculation(IEnumerable<Vector2<int>> highGroundPositions)
        {
            foreach (var pos in highGroundPositions)
            {
                if (pos.IsInBoundsOf(Config.MapSize) is false)
                    continue;

                if (Graph.ContainsMainPoint(pos, true))
                    continue;

                var point = Graph.AddPoint(pos, true, 100);

                if (Config.DebugEnabled)
                    Console.WriteLine($"1. Creating point {JsonConvert.SerializeObject(point)}");

                foreach (var offset in IterateVector2Int.Positions(
                             new Vector2<int>(-1, -1), new Vector2<int>(2, 2)))
                {
                    var otherPoint = GetAdjacentConnectablePoint(point, offset, true);
                    if (otherPoint is null)
                        continue;

                    Graph.ConnectPoints(point, (Point)otherPoint);

                    if (Config.DebugEnabled)
                        Console.WriteLine($"5. Connecting {JsonConvert.SerializeObject(point)} to adjacent high " +
                                          $"ground point {JsonConvert.SerializeObject(otherPoint)}. ");
                }

                UpdateHighGroundForNon1XPathfinding(point);
            }
        }

        private void RunAscendableCalculation(IList<IEnumerable<Vector2<int>>> path)
        {
            if (path.IsEmpty())
                return;

            var currentAscensionLevel = 0;
            var ascensionGain = (int)(100f / path.Count);
            for (var i = 0; i < path.Count; i++)
            {
                currentAscensionLevel += ascensionGain;
                foreach (var pos in path[i])
                {
                    if (pos.IsInBoundsOf(Config.MapSize) is false)
                        continue;

                    Point point;
                    if (Graph.ContainsMainPoint(pos, true))
                    {
                        point = Graph.GetMainPoint(pos, true);
                        if (Config.DebugEnabled)
                            Console.WriteLine($"1. Found existing point {JsonConvert.SerializeObject(point)}");
                    }
                    else
                    {
                        point = Graph.AddPoint(pos, true, currentAscensionLevel);
                        if (Config.DebugEnabled)
                            Console.WriteLine($"2. Creating point {JsonConvert.SerializeObject(point)}");
                    }

                    foreach (var offset in IterateVector2Int.Positions(
                                 new Vector2<int>(-1, -1), new Vector2<int>(2, 2)))
                    {
                        var lowGroundPoint = GetAdjacentConnectablePoint(point, offset, false);
                        if (i == 0 && lowGroundPoint != null)
                        {
                            Graph.ConnectPoints(point, (Point)lowGroundPoint);
                            if (Config.DebugEnabled)
                                Console.WriteLine($"3. Connecting {JsonConvert.SerializeObject(point)} to low ground " +
                                                  $"point {JsonConvert.SerializeObject(lowGroundPoint)}. ");
                        }

                        if (path.Count > 1 && i != 0)
                        {
                            var offsetPosition = point.Position + offset;
                            var previousStepPositionExists = path[i - 1].Any(x =>
                                x.Equals(offsetPosition));
                            if (previousStepPositionExists)
                            {
                                var previousStepPosition = path[i - 1].FirstOrDefault(x =>
                                    x.Equals(offsetPosition));
                                var previousStepPoint = Graph.GetMainPoint(previousStepPosition, true);
                                Graph.ConnectPoints(point, previousStepPoint);
                                if (Config.DebugEnabled)
                                    Console.WriteLine($"4. Connecting {JsonConvert.SerializeObject(point)} to previous " +
                                                      $"step point {JsonConvert.SerializeObject(previousStepPoint)}. ");
                            }
                        }

                        var highGroundPoint = GetAdjacentConnectablePoint(point, offset, true);
                        if (highGroundPoint is null)
                            continue;

                        Graph.ConnectPoints(point, (Point)highGroundPoint);
                        if (Config.DebugEnabled)
                            Console.WriteLine($"5. Connecting {JsonConvert.SerializeObject(point)} to adjacent high " +
                                              $"ground point {JsonConvert.SerializeObject(highGroundPoint)}. ");
                    }

                    UpdateHighGroundForNon1XPathfinding(point);
                }
            }
        }
        
        public void RunOccupationCalculation(PathfindingEntity entity)
        {
            var team = 1; // TODO add team
            var size = 1; // TODO move AddOccupation to Graph and automatically handle adding
            // occupation for all graph sizes

            var maxSizeForPathfinding = Config.MaxSizeForPathfinding.Value;

            var foundPoints = GetPointsProjectedDownFromEntity(entity)
                .Where(point => entity.CanBeMovedThroughAt(point, team) is false)
                .ToList();

            Graph.SetPointsImpassable(true, foundPoints);

            foreach (var foundPosition in foundPoints
                         .Where(x => x.IsHighGround is false)
                         .Select(x => x.Position))
            {
                foreach (var position in IterateVector2Int.Positions(
                             foundPosition - Vector2Int.One * maxSizeForPathfinding, 
                             foundPosition + Vector2Int.One * maxSizeForPathfinding))
                {
                    if (Graph.ContainsPoint(position, team, size) is false)
                        continue;

                    Graph.UpdateDiagonalConnection(position);
                }
            }
        }

        private Point GetAdjacentConnectablePoint(Point point, Vector2<int> offset, bool isHighGround)
        {
            var otherPosition = point.Position + offset;
            if (otherPosition.IsInBoundsOf(Config.MapSize) is false)
                return null;

            if (otherPosition.Equals(point.Position))
                return null;

            if (Graph.ContainsMainPoint(otherPosition, isHighGround) is false)
                return null;

            var otherPoint = Graph.GetMainPoint(otherPosition, isHighGround);

            if (isHighGround && PointAscensionLevelsAreWithinTolerance(point, otherPoint) is false)
                return null;

            return otherPoint;
        }

        private bool PointAscensionLevelsAreWithinTolerance(Point point, Point otherPoint)
            => Math.Abs(otherPoint.HighGroundAscensionLevel - point.HighGroundAscensionLevel)
               <= Config.HighGroundTolerance;


        private void UpdateHighGroundForNon1XPathfinding(Point main1XPoint)
        {
            if (Config.DebugEnabled)
                Console.WriteLine($"6. Updating non 1x high ground for main point " +
                                  $"{JsonConvert.SerializeObject(main1XPoint)}.");

            foreach (var currentPosition in IterateVector2Int.Positions(
                         main1XPoint.Position - Vector2Int.One * Config.MaxSizeForPathfinding.Value,
                         main1XPoint.Position + Vector2Int.One * Config.MaxSizeForPathfinding.Value + Vector2Int.One))
            {
                if (Config.DebugEnabled)
                    Console.WriteLine($"6.1. Looking at {JsonConvert.SerializeObject(currentPosition)}.");

                if (Graph.ContainsMainPoint(currentPosition, true) is false)
                {
                    if (Config.DebugEnabled)
                        Console.WriteLine($"6.2. High ground position {JsonConvert.SerializeObject(currentPosition)} " +
                                          $"does not exist.");
                    continue;
                }

                var point = Graph.GetMainPoint(currentPosition, true);

                foreach (var size in Graph.SupportedSizes.Except(new[] { PathfindingSize.Default }))
                {
                    if (Config.DebugEnabled)
                        Console.WriteLine(
                            $"6.3. Looking at size {size} with {nameof(AllMainHighGroundPointsExistForSize)}: " +
                            $"{AllMainHighGroundPointsExistForSize(size, currentPosition)}, " +
                            $"{nameof(MainPointHasAdjacentLowGroundConnections)}: " +
                            $"{MainPointHasAdjacentLowGroundConnections(point)}.");

                    if (AllMainHighGroundPointsExistForSize(size, currentPosition) is false
                        && MainPointHasAdjacentLowGroundConnections(point) is false)
                    {
                        if (Config.DebugEnabled)
                            Console.WriteLine($"6.4. Returning {JsonConvert.SerializeObject(currentPosition)}.");
                        continue;
                    }

                    foreach (var adjacentPosition in currentPosition.AdjacentPositions(false))
                    {
                        if (Config.DebugEnabled)
                            Console.WriteLine($"7. Looking at adjacent position " +
                                              $"{JsonConvert.SerializeObject(adjacentPosition)}.");

                        if (Graph.ContainsMainPoint(adjacentPosition, false))
                        {
                            if (Config.DebugEnabled)
                                Console.WriteLine($"8.1. Adjacent point exists on low ground.");

                            var adjacentLowGroundPoint = Graph.GetMainPoint(adjacentPosition, false);
                            if (Graph.HasMainConnection(point, adjacentLowGroundPoint))
                            {
                                Graph.ConnectPoints(point, adjacentLowGroundPoint, size);
                                if (Config.DebugEnabled)
                                    Console.WriteLine($"8.2. Connecting {size}X pathfinding point " +
                                                      $"{JsonConvert.SerializeObject(point)} with low ground point " +
                                                      $"{JsonConvert.SerializeObject(adjacentLowGroundPoint)}.");
                            }
                        }

                        if (Graph.ContainsMainPoint(adjacentPosition, true))
                        {
                            if (Config.DebugEnabled)
                                Console.WriteLine($"9.1. Adjacent point exists on high ground.");

                            var adjacentHighGroundPoint = Graph.GetMainPoint(adjacentPosition, true);
                            if (Graph.HasMainConnection(point, adjacentHighGroundPoint))
                            {
                                Graph.ConnectPoints(point, adjacentHighGroundPoint, size);
                                if (Config.DebugEnabled)
                                    Console.WriteLine($"9.2. Connecting {size}X pathfinding point " +
                                                      $"{JsonConvert.SerializeObject(point)} with high ground point " +
                                                      $"{JsonConvert.SerializeObject(adjacentHighGroundPoint)}.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the main points (size 1, team 1) all exist for a given size at given coordinate.
        /// Example that would return true (X - point exists):
        /// <code>
        /// Size: 2
        /// X X
        /// X X
        /// </code>
        /// Example that would return false:
        /// <code>
        /// Size: 3
        /// X X O
        /// X X X
        /// X X X
        /// </code>
        /// </summary>
        private bool AllMainHighGroundPointsExistForSize(PathfindingSize size, Vector2<int> at)
            => IterateVector2Int.Positions(at, at + Vector2Int.One * size.Value)
                .All(position => Graph.ContainsMainPoint(position, true));

        private bool MainPointHasAdjacentLowGroundConnections(Point point)
            => point.Position.AdjacentPositions()
                .Where(adjacentPosition => Graph.ContainsMainPoint(adjacentPosition, false))
                .Select(adjacentPosition => Graph.GetMainPoint(adjacentPosition, false))
                .Any(adjacentLowGroundPoint => Graph.HasMainConnection(point, adjacentLowGroundPoint));
        
        public IList<Point> GetPointsProjectedDownFromEntity(PathfindingEntity entity)
        {
            const int pathfindingSize = 1;
            var team = 1; // TODO add team

            var points = IterateVector2Int
                .Positions(entity.Position, entity.Position + entity.Size)
                .Select(position => 
                    Graph.GetHighestPoint(position, team, pathfindingSize, entity.IsOnHighGround))
                .Where(point => point != null)
                .Select(point => point)
                .ToList();

            return points;
        }

        #endregion Setters
    }
}