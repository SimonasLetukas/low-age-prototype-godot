using DijkstraMap.Methods;
using LowAgeCommon.Extensions;
using Priority_Queue;

namespace DijkstraMap
{
    /// <summary>
    /// Controls the direction of the <see cref="Dijkstra"/> in <see cref="Dijkstra.Recalculate"/>.
    /// </summary>
    public enum InputDirection
    {
        /// <summary>
        /// Input points are seen as <b>destinations</b>.
        ///
        /// This means the algorithm will compute path <b>from</b> various points
        /// <b>to</b> the closest input point.
        /// </summary>
        InputIsDestination,

        /// <summary>
        /// Input points are seen as <b>origin</b>.
        ///
        /// This means the algorithm will compute path <b>from</b> an origin <b>to</b>
        /// various points.
        /// </summary>
        InputIsOrigin,
    }

    /// <summary>
    /// Information about a point.
    ///
    /// Contains the <see cref="Connections"/>, <see cref="ReverseConnections"/> and <see cref="Terrain"/> type for
    /// a point.
    /// </summary>
    internal class PointInfo : IEquatable<PointInfo>
    {
        /// <summary>
        /// Connections from this point to others.
        /// </summary>
        public Dictionary<PointId, Weight> Connections { get; }

        /// <summary>
        /// Connections from other points to this one.
        /// </summary>
        public Dictionary<PointId, Weight> ReverseConnections { get; }

        /// <summary>
        /// Point's <see cref="Terrain"/>.
        /// </summary>
        public Terrain Terrain { get; set; }

        public PointInfo(PointInfo pointInfo)
        {
            Connections = pointInfo.Connections.ToDictionary(
                entry => new PointId(entry.Key.Value),
                entry => new Weight(entry.Value.Value));
            ReverseConnections = pointInfo.ReverseConnections.ToDictionary(
                entry => new PointId(entry.Key.Value),
                entry => new Weight(entry.Value.Value));
            Terrain = new Terrain(pointInfo.Terrain.Value);
        }

        public PointInfo(Dictionary<PointId, Weight> connections, Dictionary<PointId, Weight> reverseConnections,
            Terrain terrain)
        {
            Connections = connections;
            ReverseConnections = reverseConnections;
            Terrain = terrain;
        }

        public PointInfo(Terrain terrain)
        {
            Connections = new Dictionary<PointId, Weight>();
            ReverseConnections = new Dictionary<PointId, Weight>();
            Terrain = terrain;
        }

        public bool Equals(PointInfo other)
            => other != null
               && Connections.IsEquivalentTo(other.Connections)
               && ReverseConnections.IsEquivalentTo(other.ReverseConnections)
               && Terrain.Equals(other.Terrain);

        public override bool Equals(object obj) => obj is PointInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Connections != null ? Connections.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ReverseConnections != null ? ReverseConnections.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    /// <summary>
    /// Information computed by Dijkstra for a point, grouped in a single structure.
    /// </summary>
    public readonly struct PointComputedInfo : IEquatable<PointComputedInfo>
    {
        /// <summary>
        /// Cost of this point's shortest path.
        /// </summary>
        public Cost Cost { get; }

        /// <summary>
        /// Next point along the shortest path.
        /// </summary>
        public PointId Direction { get; }

        public PointComputedInfo(PointComputedInfo pointComputedInfo)
        {
            Cost = new Cost(pointComputedInfo.Cost.Value);
            Direction = new PointId(pointComputedInfo.Direction.Value);
        }
        
        public PointComputedInfo(Cost cost, PointId direction)
        {
            Cost = cost;
            Direction = direction;
        }

        public bool Equals(PointComputedInfo other)
            => Cost.Equals(other.Cost)
               && Direction.Equals(other.Direction);

        public override bool Equals(object obj) => obj is PointComputedInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Cost.GetHashCode() * 397) ^ Direction.GetHashCode();
            }
        }
    }

    /// <summary>
    /// <para>Representation of the map.</para>
    ///
    /// <para>This holds the necessary information for Dijkstra's algorithm.</para>
    ///
    /// <para>To use it, you should:</para>
    /// <para> - Populate the map with <see cref="Dijkstra.AddPoint"/>, <see cref="Dijkstra.ConnectPoints"/>...</para>
    /// <para> - Compute the shortest paths with <see cref="Dijkstra.Recalculate"/>.</para>
    /// </summary>
    public class Dijkstra
    {
        /// Map a point to its information.
        internal Dictionary<PointId, PointInfo> Points { get; private set; } = new Dictionary<PointId, PointInfo>();

        /// All the points in the map, sorted by their cost.
        internal List<PointId> SortedPoints { get; private set; } = new List<PointId>();

        /// Cost and direction information for each point.
        internal Dictionary<PointId, PointComputedInfo> ComputedInfo { get; private set; } = 
            new Dictionary<PointId, PointComputedInfo>();

        /// Points not treated by the algorithm.
        internal HashSet<PointId> DisabledPoints { get; private set; } = new HashSet<PointId>();

        /// <summary>
        /// <para>
        /// Recalculates cost map and direction map information for each
        /// point, overriding previous results.
        /// </para>
        /// <para>
        /// This is the central function of the library, the one that
        /// actually uses Dijkstra's algorithm.
        /// </para>
        /// </summary>
        /// <param name="origin">ID of the origin point.</param>
        /// <param name="additionalOrigins">Additional origin points.</param>
        /// <param name="inputIsDestination">If true, input points are seen as <b>destinations</b>. This means
        /// the algorithm will compute path <b>from</b> various points <b>to</b> the closest input point.
        /// Default = true.</param>
        /// <param name="maximumCost">Specifies maximum cost. Once all the shortest paths no
        /// longer than the maximum cost are found, the algorithm terminates. All points with
        /// cost bigger than this are treated as inaccessible. Default = <see cref="float.PositiveInfinity"/>.</param>
        /// <param name="initialCosts">Specifies initial costs for the given <see cref="origin"/>s. Values are
        /// paired with corresponding indices in the <see cref="origin"/> and <see cref="additionalOrigins"/>
        /// arguments. Every unspecified cost is defaulted to `0.0`.
        /// Can be used to weigh the <see cref="origin"/>s with a preference.</param>
        /// <param name="terrainWeights">Specifies weights of terrain types. Keys are terrain type IDs and values
        /// are floats. Unspecified terrains will have <see cref="float.PositiveInfinity"/> weight.
        /// <b>Note</b> that `-1` correspond to the default terrain (which have a weight of `1.0`), and will thus be
        /// ignored if it appears in the keys.</param>
        /// <param name="terminationPoints">A set of points that stop the computation if they are
        /// reached by the algorithm.</param>
        public void Recalculate(
            int origin,
            IEnumerable<int>? additionalOrigins = null,
            bool? inputIsDestination = null,
            float? maximumCost = null,
            List<float>? initialCosts = null,
            Dictionary<int, float>? terrainWeights = null,
            HashSet<int>? terminationPoints = null)
        {
            additionalOrigins ??= new List<int>();
            inputIsDestination ??= true;
            initialCosts ??= [];
            terrainWeights ??= new Dictionary<int, float>();
            terminationPoints ??= [];
            
            Recalculate(
                origin,
                additionalOrigins.Select(x => new PointId(x)).ToList(),
                inputIsDestination is true ? InputDirection.InputIsDestination : InputDirection.InputIsOrigin,
                maximumCost,
                initialCosts.Select(x => new Cost(x)).ToList(),
                terrainWeights.ToDictionary(kvp => new Terrain(kvp.Key), kvp => new Weight(kvp.Value)),
                terminationPoints.Select(x => new PointId(x)).ToHashSet());
        }
        
        /// <summary>
        /// <para>
        /// Recalculates cost map and direction map information for each
        /// point, overriding previous results.
        /// </para>
        /// <para>
        /// This is the central function of the library, the one that
        /// actually uses Dijkstra's algorithm.
        /// </para>
        /// </summary>
        /// <param name="origin">ID of the origin point. <see cref="int"/> type can be used.</param>
        /// <param name="additionalOrigins">Additional origin points.</param>
        /// <param name="inputDirection">Specify how the <see cref="origin"/> point(s) are seen as.
        /// Default = <see cref="InputDirection.InputIsDestination"/>.</param>
        /// <param name="maximumCost">Specifies maximum cost. Once all the shortest paths no
        /// longer than the maximum cost are found, the algorithm terminates. All points with
        /// cost bigger than this are treated as inaccessible. <see cref="float"/> type can be used.
        /// Default = <see cref="float.PositiveInfinity"/>.</param>
        /// <param name="initialCosts">Specifies initial costs for the given <see cref="origin"/>s. Values are
        /// paired with corresponding indices in the <see cref="origin"/> and <see cref="additionalOrigins"/>
        /// arguments. Every unspecified cost is defaulted to `0.0`.
        /// Can be used to weigh the <see cref="origin"/>s with a preference.</param>
        /// <param name="terrainWeights">Specifies weights of terrain types. Keys are terrain type IDs and values
        /// are floats. Unspecified terrains will have <see cref="float.PositiveInfinity"/> weight.
        /// <b>Note</b> that `-1` correspond to the default terrain (which have a weight of `1.0`), and will thus be
        /// ignored if it appears in the keys.</param>
        /// <param name="terminationPoints">A set of points that stop the computation if they are
        /// reached by the algorithm.</param>
        public void Recalculate(
            PointId origin,
            IEnumerable<PointId>? additionalOrigins = null,
            InputDirection? inputDirection = null,
            Cost? maximumCost = null,
            List<Cost>? initialCosts = null,
            Dictionary<Terrain, Weight>? terrainWeights = null,
            HashSet<PointId>? terminationPoints = null)
        {
            var origins = new List<PointId> { origin }; 
            origins.AddRange(additionalOrigins ?? new List<PointId>());
            inputDirection ??= InputDirection.InputIsDestination;
            maximumCost ??= new Cost(float.PositiveInfinity);
            initialCosts ??= [];
            terrainWeights ??= new Dictionary<Terrain, Weight>();
            terminationPoints ??= [];

            ComputedInfo.Clear();
            SortedPoints.Clear();

            var openQueue = new StablePriorityQueue<PointId>(Points.Count);

            // Add origin points to the open queue
            for (var i = 0; i < origins.Count; i++)
            {
                var src = origins[i];
                if (GetConnections(src, inputDirection.Value) != null)
                {
                    ComputedInfo[src] = new PointComputedInfo(initialCosts.ElementAtOrDefault(i), src);
                    openQueue.Enqueue(src, this.GetCostAtPoint(src).Value);
                }
            }

            var c = Points.Count;
            // Iterate over the open queue
            while (openQueue.Count > 0)
            {
                var point1 = (PointId)openQueue.Dequeue();

                if (c < 0) break;
                c--;

                SortedPoints.Add(point1);
                if (terminationPoints.Contains(point1))
                {
                    break;
                }

                var point1Cost = this.GetCostAtPoint(point1);
                var point1Terrain = this.GetTerrainForPoint(point1);
                var weightOfPoint1 = point1Terrain.Equals(Terrain.Default)
                    ? Weight.One
                    : terrainWeights.GetValueOrDefault(point1Terrain, Weight.Infinite);

                foreach (var kvp in GetConnections(point1, inputDirection.Value) ?? new Dictionary<PointId, Weight>())
                {
                    var point2 = kvp.Key;
                    var dirCost = kvp.Value;

                    var cost = new Cost(point1Cost.Value + dirCost.Halved *
                        (weightOfPoint1.Value +
                         terrainWeights.GetValueOrDefault(this.GetTerrainForPoint(point2), Weight.One).Value));

                    if (cost.Value < this.GetCostAtPoint(point2).Value
                        && cost.Value <= maximumCost.Value.Value
                        && !DisabledPoints.Contains(point2))
                    {
                        openQueue.EnqueueOrUpdate(point2, cost.Value);
                        ComputedInfo[point2] = new PointComputedInfo(cost, point1);
                    }
                }
            }
        }

        private Dictionary<PointId, Weight>? GetConnections(PointId pointId, InputDirection inputDirectionMode)
        {
            if (Points.TryGetValue(pointId, out var info) is false)
                return null;

            return inputDirectionMode switch
            {
                InputDirection.InputIsDestination => info.ReverseConnections,
                InputDirection.InputIsOrigin => info.Connections,
                _ => null
            };
        }

        /// Clears the <see cref="Dijkstra"/> of all points and connections.
        public void Clear()
        {
            Points.Clear();
            ComputedInfo.Clear();
            SortedPoints.Clear();
            DisabledPoints.Clear();
        }

        /// Duplicates existing <see cref="Dijkstra"/> into a new copy.
        public Dijkstra DuplicateGraph()
        {
            return new Dijkstra
            {
                Points = Points.ToDictionary(
                    entry => new PointId(entry.Key.Value),
                    entry => new PointInfo(entry.Value)
                ),
                SortedPoints = new List<PointId>(SortedPoints.Select(p => new PointId(p.Value))),
                ComputedInfo = ComputedInfo.ToDictionary(
                    entry => new PointId(entry.Key.Value), 
                    entry => new PointComputedInfo(entry.Value)
                ),
                DisabledPoints = new HashSet<PointId>(DisabledPoints.Select(p => new PointId(p.Value)))
            };
        }

        /// Updates current <see cref="Dijkstra"/> with the values copied from another <see cref="Dijkstra"/>.
        public void DuplicateGraphFrom(Dijkstra other)
        {
            var otherCopied = other.DuplicateGraph();
            Points = new Dictionary<PointId, PointInfo>(otherCopied.Points);
            SortedPoints = new List<PointId>(otherCopied.SortedPoints);
            ComputedInfo = new Dictionary<PointId, PointComputedInfo>(otherCopied.ComputedInfo);
            DisabledPoints = new HashSet<PointId>(otherCopied.DisabledPoints);
        }
    }
}