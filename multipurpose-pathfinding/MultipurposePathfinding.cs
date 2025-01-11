using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using low_age_dijkstra;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
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
        /// like occupation and high-ground. Should be used to also add or update occupation (data within
        /// <see cref="PathfindingEntity"/> is used to determine occupation). <see cref="UpdateAround"/> should be
        /// called to update the pathfinding graph.
        /// </summary>
        void AddOrUpdateEntity(PathfindingEntity entity);

        /// <summary>
        /// Removes entity from the pathfinding. <see cref="UpdateAround"/> cannot be called after the entity is
        /// removed. This method automatically calls all the necessary methods to clean up the entity occupation
        /// and high ground points, then updates the pathfinding graph.
        /// </summary>
        void RemoveEntity(Guid entityId);

        /// <summary>
        /// Adds a path of ascendable high-ground to the pathfinding calculations for the given entity.
        /// <see cref="UpdateAround"/> should be called to update the pathfinding graph.
        /// </summary>
        /// <param name="entityId">Reference to the entity for which the ascendable high ground is added.</param>
        /// <param name="path">A list of ascendable steps (from lowest to highest): each step is a collection of
        /// positions and their corresponding sprite offset.</param>
        void AddAscendableHighGround(Guid entityId, IList<IEnumerable<Vector2<int>>> path);

        /// <summary>
        /// Removes the path of ascendable high-ground from the pathfinding calculations for the given entity.
        /// <see cref="UpdateAround"/> should be called to update the pathfinding graph.
        /// </summary>
        void RemoveAscendableHighGround(Guid entityId);

        /// <summary>
        /// Adds the positions of high-ground to the pathfinding calculations for the given entity.
        /// <see cref="UpdateAround"/> should be called to update the pathfinding graph.
        /// </summary>
        void AddHighGround(Guid entityId, IEnumerable<Vector2<int>> positions);

        /// <summary>
        /// Removes the positions of high-ground from the pathfinding calculations for the given entity.
        /// <see cref="UpdateAround"/> should be called to update the pathfinding graph.
        /// </summary>
        void RemoveHighGround(Guid entityId);

        /// <summary>
        /// Updates the pathfinding graph around the given <see cref="entityId"/>. <see cref="PathfindingEntity"/> must
        /// have been registered using <see cref="AddOrUpdateEntity"/>. Depending on the changes done using
        /// <see cref="AddAscendableHighGround"/>, <see cref="RemoveAscendableHighGround"/>,
        /// <see cref="AddHighGround"/>, <see cref="RemoveHighGround"/> this method also publishes
        /// <see cref="PointAdded"/> and <see cref="PointRemoved"/> events for this given
        /// <see cref="PathfindingEntity"/>.
        /// </summary>
        void UpdateAround(Guid entityId);
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
                Graph.SetTerrainForPoint(coordinates, terrain, Vector2Int.Zero, Config.MapSize);
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

            Graph.SetTerrainForPoint(coordinates, terrain.Value, Vector2Int.Zero, Config.MapSize);
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
        
        internal Point GetPointAt(Vector2<int> position, bool isHighGround, PathfindingSize size, Team? team = null) 
            => Graph.ContainsPoint(position, team ?? Team.Default, size, isHighGround) 
                ? Graph.GetPoint(position, team ?? Team.Default, size, isHighGround)
                : null;

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
        
        private interface IPathfindingChangeEvent { }

        private class AscendableAdded : IPathfindingChangeEvent
        {
            public IList<IEnumerable<Vector2<int>>> AddedPath { get; set; }
        }

        private class AscendableRemoved : IPathfindingChangeEvent
        {
            public IEnumerable<Point> RemovedPoints { get; set; }
        }

        private class HighGroundAdded : IPathfindingChangeEvent
        {
            public IEnumerable<Vector2<int>> AddedPositions { get; set; }
        }

        private class HighGroundRemoved : IPathfindingChangeEvent
        {
            public IEnumerable<Point> RemovedPoints { get; set; }
        }
        
        private Dictionary<Guid, PipelineItem> PipelineItemsByEntityId { get; } = new Dictionary<Guid, PipelineItem>();
        private Quadtree<PathfindingEntity> EntitiesSpatialMap { get; } = new Quadtree<PathfindingEntity>();
        private Dictionary<Guid, ICollection<IPathfindingChangeEvent>> EventsByEntity { get; } =
            new Dictionary<Guid, ICollection<IPathfindingChangeEvent>>();

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
            EventsByEntity[entity.Id] = new List<IPathfindingChangeEvent>();
        }

        public void RemoveEntity(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;
            
            if (item.HasAscendableHighGround)
                RemoveAscendableHighGround(entityId);
            if (item.HasHighGround)
                RemoveHighGround(entityId);

            var entity = item.OccupyingEntity;
            EntitiesSpatialMap.Remove(entity.Bounds, entity);
            
            RunPathfindingPipeline(entity.Position, entity.UpperBounds);
            HandlePathfindingChangeEvents(entityId);
            
            PipelineItemsByEntityId.Remove(entityId);
            EventsByEntity.Remove(entityId);
        }

        public void AddAscendableHighGround(Guid entityId, IList<IEnumerable<Vector2<int>>> path)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            item.AscendablePath = path;
            
            EventsByEntity[entityId].Add(new AscendableAdded
            {
                AddedPath = path
            });
        }

        public void RemoveAscendableHighGround(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var path = item.AscendablePath;
            var points = path.SelectMany(section => section,
                (entry, pos) => Graph.GetMainPoint(pos, true)).ToList();
            item.RemoveAscendablePath();

            EventsByEntity[entityId].Add(new AscendableRemoved
            {
                RemovedPoints = points
            });
        }

        public void AddHighGround(Guid entityId, IEnumerable<Vector2<int>> positions)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var positionsList = positions.ToList();
            item.HighGroundPositions = positionsList;

            EventsByEntity[entityId].Add(new HighGroundAdded
            {
                AddedPositions = positionsList
            });
        }

        public void RemoveHighGround(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var points = item.HighGroundPositions
                .Select(pos => Graph.GetMainPoint(pos, true)).ToList();
            item.RemoveHighGroundPositions();

            EventsByEntity[entityId].Add(new HighGroundRemoved
            {
                RemovedPoints = points
            });
        }

        public void UpdateAround(Guid entityId)
        {
            if (PipelineItemsByEntityId.TryGetValue(entityId, out var item) is false)
                return;

            var entity = item.OccupyingEntity;
            RunPathfindingPipeline(entity.Position, entity.UpperBounds);

            HandlePathfindingChangeEvents(entityId);
        }

        private void HandlePathfindingChangeEvents(Guid entityId)
        {
            foreach (var @event in EventsByEntity[entityId])
            {
                HandlePathfindingChangeEvent(@event);
            }
        }

        private void HandlePathfindingChangeEvent(IPathfindingChangeEvent @event)
        {
            switch (@event)
            {
                case AscendableAdded ascendableAdded:
                    foreach (var section in ascendableAdded.AddedPath)
                    {
                        foreach (var coordinates in section)
                        {
                            if (Graph.ContainsMainPoint(coordinates, true) is false)
                                continue;
                            
                            var point = Graph.GetMainPoint(coordinates, true);
                            PointAdded(point);
                        }
                    }
                    break;
                case AscendableRemoved ascendableRemoved:
                    foreach (var point in ascendableRemoved.RemovedPoints)
                    {
                        PointRemoved(point);
                    }
                    break;
                case HighGroundAdded highGroundAdded:
                    foreach (var position in highGroundAdded.AddedPositions)
                    {
                        if (Graph.ContainsMainPoint(position, true) is false)
                            continue;
                        
                        var point = Graph.GetMainPoint(position, true);
                        PointAdded(point);
                    }
                    break;
                case HighGroundRemoved highGroundRemoved:
                    foreach (var point in highGroundRemoved.RemovedPoints)
                    {
                        PointRemoved(point);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(@event)} type is not implemented in " +
                                                          $"{nameof(MultipurposePathfinding)}." +
                                                          $"{nameof(HandlePathfindingChangeEvent)}");
            }
        }

        private void RunPathfindingPipeline(Vector2<int> from, Vector2<int> to)
        {
            // TODO connect with Pathfinding.cs
            // TODO write tests for simulating units walking on top of high ground and graph updating
            
            var boundsOffset = Vector2Int.One * Config.MaxSizeForPathfinding.Value;
            var lowerBounds = from - boundsOffset;
            var upperBounds = to + boundsOffset;
            var foundItems = GetEntitiesIntersectingWith(lowerBounds, upperBounds)
                .Select(e => PipelineItemsByEntityId[e.Id])
                .ToList();

            Graph.ResetToTerrainPoints(lowerBounds, upperBounds);

            foreach (var item in foundItems)
            {
                RunHighGroundCalculation(item.HighGroundPositions);
            }
            
            foreach (var item in foundItems)
            {
                RunAscendableCalculation(item.AscendablePath, item.OccupyingEntity);
            }
            
            var occupationPositions = new HashSet<Vector2<int>>();
            foreach (var item in foundItems)
            {
                var foundOccupationPositions = RunOccupationCalculation(item);
                occupationPositions.UnionWith(foundOccupationPositions);
            }
            
            RunDiagonalConnectionCalculation(occupationPositions);
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
                    TryConnectingToAdjacentHighGroundPoint(point, offset);
                }
            }
        }

        private void RunAscendableCalculation(IList<IEnumerable<Vector2<int>>> path, 
            PathfindingEntity ascendableEntity)
        {
            if (path.IsEmpty() || path.Any(x => x is null))
                return;

            var currentAscensionLevel = 100;
            var ascensionGain = (int)(100f / path.Count);
            for (var currentStep = 0; currentStep < path.Count; currentStep++)
            {
                foreach (var pos in path[currentStep])
                {
                    if (pos.IsInBoundsOf(Config.MapSize) is false)
                        continue;

                    var point = GetExistingOrAddNewHighGroundPoint(pos, currentAscensionLevel);

                    foreach (var offset in IterateVector2Int.Positions(
                                 new Vector2<int>(-1, -1), new Vector2<int>(2, 2)))
                    {
                        TryConnectingToAdjacentHighGroundPoint(point, offset);

                        TryConnectingToAdjacentLowGroundPoint(point, offset, currentStep, 
                            path.Count, ascendableEntity);

                        TryConnectingToAdjacentPreviousStepPoint(point, offset, currentStep, path);
                    }
                }
                
                currentAscensionLevel -= ascensionGain;
                currentAscensionLevel = currentAscensionLevel < 0 ? 0 : currentAscensionLevel;
            }
        }
        
        private IEnumerable<Vector2<int>> RunOccupationCalculation(PipelineItem item)
        {
            var entity = item.OccupyingEntity;
            var offset = Vector2Int.One * Config.MaxSizeForPathfinding.Value;
            var entityPositions = IterateVector2Int
                .Positions(entity.Position - offset, entity.UpperBounds + offset, true)
                .Where(position => position.IsInBoundsOf(Config.MapSize))
                .ToList();

            var highGroundPoints = new List<Point>();
            if (entity.IsOnHighGround || item.HasHighGround || item.HasAscendableHighGround)
                highGroundPoints = entityPositions
                    .Where(position => Graph.ContainsMainPoint(position, true))
                    .Select(position => Graph.GetMainPoint(position, true))
                    .ToList();

            var lowGroundPoints = entityPositions
                .Where(position => Graph.ContainsMainPoint(position, false))
                .Select(position => Graph.GetMainPoint(position, false))
                .ToList();
            
            lowGroundPoints.AddRange(highGroundPoints);

            foreach (var mainPoint in lowGroundPoints)
            {
                foreach (var team in Graph.SupportedTeams)
                {
                    foreach (var pathfindingSize in Graph.SupportedSizes)
                    {
                        var isImpassable = PointShouldBeImpassable(mainPoint, entity, team, pathfindingSize);
                        Graph.SetPointImpassable(isImpassable, mainPoint.Id, team, pathfindingSize);
                    }
                }
            }
            
            return entityPositions;
        }
        
        private void RunDiagonalConnectionCalculation(IEnumerable<Vector2<int>> positions)
        {
            foreach (var position in positions)
            {
                Graph.UpdateDiagonalConnection(position);
            }
        }

        private bool PointShouldBeImpassable(Point mainPoint, PathfindingEntity entity, Team team, PathfindingSize size)
        {
            if (Graph.GetPoint(mainPoint.Id, team, size).IsImpassable)
                return true;
            
            if (mainPoint.IsLowGround 
                && Graph.ContainsMainPoint(mainPoint.Position, true))
                return true;

            var lowerBounds = mainPoint.Position;
            var upperBounds = mainPoint.Position + Vector2Int.One * size.Value;
            var positionsToCheck = IterateVector2Int
                .Positions(lowerBounds, upperBounds)
                .ToList();

            if (positionsToCheck.Any(x => x.IsInBoundsOf(Config.MapSize) is false))
                return true;

            var footprintPoints = positionsToCheck
                .Select(x => Graph.GetHighestPoint(x, team, size))
                .ToList();
            
            if (AllPointsWithinEntityCanBeUsedToMoveThrough(footprintPoints, entity, team)
                && AllPointsHaveSmoothGradient(footprintPoints, team, size, lowerBounds, upperBounds))
                return false;
            
            return true;
        }

        private static bool AllPointsWithinEntityCanBeUsedToMoveThrough(IEnumerable<Point> points, 
            PathfindingEntity entity, Team forTeam) 
            => points
                .Where(point => point.Position.IsInBoundsOf(entity.Position, entity.UpperBounds))
                .All(point => entity.CanBeMovedThroughAt(point, forTeam));

        private bool AllPointsHaveSmoothGradient(List<Point> points, Team team, PathfindingSize size, 
            Vector2<int> lowerBounds, Vector2<int> upperBounds)
        {
            if (points.Count <= 1)
                return true;

            foreach (var point in points)
            {
                if (point.IsHighGround)
                    continue;
                
                foreach (var adjacentPosition in point.Position
                             .AdjacentPositions(true, lowerBounds, upperBounds))
                {
                    // Checking that adjacentPoint is within MapSize bounds is not needed because it is assumed
                    // that lowerBounds and upperBounds were checked to be valid before calling this method.
                    var adjacentPoint = Graph.GetHighestPoint(adjacentPosition, team, size);

                    if (adjacentPoint.IsHighGround
                        && Graph.HighGroundPointHasLowGroundConnections(adjacentPoint, team, size) is false) 
                        return false;
                }
            }

            return true;
        }

        private Point GetExistingOrAddNewHighGroundPoint(Vector2<int> at, int currentAscensionLevel)
        {
            Point point;
            if (Graph.ContainsMainPoint(at, true))
            {
                point = Graph.GetMainPoint(at, true);
                if (Config.DebugEnabled)
                    Console.WriteLine($"1. Found existing point {JsonConvert.SerializeObject(point)}");
            }
            else
            {
                point = Graph.AddPoint(at, true, currentAscensionLevel);
                if (Config.DebugEnabled)
                    Console.WriteLine($"2. Creating point {JsonConvert.SerializeObject(point)}");
            }

            return point;
        }

        private void TryConnectingToAdjacentHighGroundPoint(Point point, Vector2<int> offset)
        {
            var highGroundPoint = GetAdjacentConnectableMainPoint(point, offset, true);
            if (highGroundPoint == null) 
                return;
            
            Graph.ConnectPoints(point, highGroundPoint);
            if (Config.DebugEnabled)
                Console.WriteLine($"3. Connecting {JsonConvert.SerializeObject(point)} to high ground " +
                                  $"point {JsonConvert.SerializeObject(highGroundPoint)}. ");
        }

        private void TryConnectingToAdjacentLowGroundPoint(Point point, Vector2<int> offset, int currentStep, 
            int pathCount, PathfindingEntity ascendableEntity)
        {
            var isLastStep = currentStep == pathCount - 1;
            if (isLastStep is false) 
                return;
            
            var lowGroundPoint = GetAdjacentConnectableMainPoint(point, offset, false);
            if (lowGroundPoint is null)
                return;

            foreach (var team in Graph.SupportedTeams)
            {
                if (ascendableEntity.AllowsConnectionBetweenPoints(lowGroundPoint, point, team) is false)
                    continue;
            
                Graph.ConnectPoints(point, lowGroundPoint, team);
                if (Config.DebugEnabled)
                    Console.WriteLine($"3. Connecting {JsonConvert.SerializeObject(point)} to low ground " +
                                      $"point {JsonConvert.SerializeObject(lowGroundPoint)} for team {team}. ");
            }
        }

        private void TryConnectingToAdjacentPreviousStepPoint(Point point, Vector2<int> offset, int currentStep, 
            IList<IEnumerable<Vector2<int>>> path)
        {
            if (path.Count <= 1 || currentStep == 0) 
                return;
            
            var offsetPosition = point.Position + offset;
            var previousStepPositionExists = path[currentStep - 1].Any(x =>
                x.Equals(offsetPosition));
            if (previousStepPositionExists is false) 
                return;
            
            var previousStepPosition = path[currentStep - 1]
                .FirstOrDefault(x => x.Equals(offsetPosition));
            var previousStepPoint = Graph.GetMainPoint(previousStepPosition, true);
            if (previousStepPoint is null)
                return;
            
            Graph.ConnectPoints(point, previousStepPoint);
            if (Config.DebugEnabled)
                Console.WriteLine($"4. Connecting {JsonConvert.SerializeObject(point)} to previous " +
                                  $"step point {JsonConvert.SerializeObject(previousStepPoint)}. ");
        }

        private Point GetAdjacentConnectableMainPoint(Point point, Vector2<int> offset, bool isHighGround)
        {
            var otherPosition = point.Position + offset;
            if (otherPosition.IsInBoundsOf(Config.MapSize) is false)
                return null;

            if (otherPosition.Equals(point.Position))
                return null;

            if (Graph.ContainsMainPoint(otherPosition, isHighGround) is false)
                return null;

            var otherPoint = Graph.GetMainPoint(otherPosition, isHighGround);

            if (isHighGround && AscensionLevelsAreWithinTolerance(point, otherPoint) is false)
                return null;

            return otherPoint;
        }

        private bool AscensionLevelsAreWithinTolerance(Point point, Point otherPoint)
            => Math.Abs(otherPoint.HighGroundAscensionLevel - point.HighGroundAscensionLevel)
               <= Config.HighGroundTolerance;

        #endregion Setters
    }
}