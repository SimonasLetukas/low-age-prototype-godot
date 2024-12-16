using System;
using System.Collections.Generic;
using System.IO;
using low_age_dijkstra;
using low_age_prototype_common;
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

        #region Initialization

        public void Initialize(IEnumerable<(Vector2<int>, Terrain)> initialPositionsAndTerrainIndexes,
            Configuration configuration = null)
        {
            throw new NotImplementedException();
        }

        public void IterateInitialization(float deltaTime)
        {
            throw new NotImplementedException();
        }

        #endregion Initialization

        public IEnumerable<Point> GetAvailablePoints(Vector2<int> from, float range, bool lookingFromHighGround,
            Team team, PathfindingSize pathfindingSize, bool temporary = false)
        {
            throw new NotImplementedException();
        }

        public void ClearCache()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Point> FindPath(Point to, Team team, PathfindingSize pathfindingSize)
        {
            throw new NotImplementedException();
        }

        public bool HasConnection(Point pointA, Point pointB, Team team, PathfindingSize pathfindingSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Point> GetTerrainPoints()
        {
            throw new NotImplementedException();
        }

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