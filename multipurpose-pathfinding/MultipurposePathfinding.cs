using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using low_age_dijkstra;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using NetTopologySuite.Geometries;

namespace multipurpose_pathfinding
{
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
        void AddAscendableHighGround(Guid entityId, IList<(IEnumerable<Vector2<int>>, int)> path);

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

    public class MultipurposePathfinding : IMultipurposePathfinding
    {
        public event Action FinishedInitializing = delegate { };

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

        private Terrain? FindRemainingTerrainWithSmallestMovementCost()
        {
            var terrainsWithAscendingMovementCost = Config.TerrainWeights
                .OrderBy(x => x.Value)
                .Select(x => new Terrain(x.Key));

            return terrainsWithAscendingMovementCost.FirstOrDefault(nextTerrain =>
                _positionsByTerrainForFurtherInitialization.ContainsKey(nextTerrain));
        }

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

        public void AddOrUpdateEntity(PathfindingEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public void AddOccupation(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public void RemoveOccupation(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public void AddAscendableHighGround(Guid entityId, IList<(IEnumerable<Vector2<int>>, int)> path)
        {
            throw new NotImplementedException();
        }

        public void RemoveAscendableHighGround(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public void AddHighGround(Guid entityId, IEnumerable<Vector2<int>> positions)
        {
            throw new NotImplementedException();
        }

        public void RemoveHighGround(Guid entityId)
        {
            throw new NotImplementedException();
        }
    }

    public class PathfindingEntity
    {
        public Guid Id { get; }
        public Envelope Bounds { get; }
        public bool IsOnHighGround { get; }
        public Func<bool, Point, Team> CanBeMovedThroughAt { get; }

        public PathfindingEntity(Guid id, Vector2<int> position, Vector2<int> size, bool isOnHighGround,
            Func<bool, Point, Team> canBeMovedThroughAt)
        {
            Id = id;
            Bounds = new Envelope(position.X, position.X + size.X, position.Y, position.Y + size.Y);
            IsOnHighGround = isOnHighGround;
            CanBeMovedThroughAt = canBeMovedThroughAt;
        }
    }
}