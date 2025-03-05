namespace DijkstraMap.Methods
{
    public static class Getters
    {
        /// Gives the smallest PointId not yet used, starting search at `above` or `0` if null.
        public static PointId GetAvailableId(this Dijkstra dijkstraMap, PointId above = null)
        {
            var id = above ?? new PointId(0);
            while (dijkstraMap.HasPoint(id))
            {
                id = new PointId(id.Value + 1);
            }

            return id;
        }
        
        /// Returns true if `point` exists in the map.
        public static bool HasPoint(this Dijkstra dijkstraMap, PointId point) 
            => dijkstraMap.Points.ContainsKey(point);

        /// Returns true if both `source` and `target` exist, and there's a connection from `source` to `target`.
        public static bool HasConnection(this Dijkstra dijkstraMap, PointId source, PointId target) 
            => dijkstraMap.Points.TryGetValue(source, out var pointInfo)
               && pointInfo.Connections.ContainsKey(target);
        
        /// Gets the terrain type for the given point, or <see cref="Terrain.Default"/> if it doesn't exist.
        public static Terrain GetTerrainForPoint(this Dijkstra dijkstraMap, PointId id) 
            => dijkstraMap.Points.TryGetValue(id, out var pointInfo) 
                ? pointInfo.Terrain 
                : Terrain.Default;

        /// Returns true if `point` exists and is disabled.
        public static bool IsPointDisabled(this Dijkstra dijkstraMap, PointId point) 
            => dijkstraMap.DisabledPoints.Contains(point);

        /// Returns an iterator over the components of the shortest path from the given point.
        public static IEnumerable<PointId> GetShortestPathFromPoint(this Dijkstra dijkstraMap, PointId point) 
            => new ShortestPathIterator(dijkstraMap, point);

        private class ShortestPathIterator : IEnumerable<PointId>
        {
            private readonly Dijkstra _dijkstraMap;
            private PointId _nextPoint;

            public ShortestPathIterator(Dijkstra map, PointId startPoint)
            {
                _dijkstraMap = map;
                _nextPoint = map.GetDirectionAtPoint(startPoint);
            }

            public IEnumerator<PointId> GetEnumerator()
            {
                while (_nextPoint != null)
                {
                    var currentPoint = _nextPoint;
                    _nextPoint = _dijkstraMap.GetDirectionAtPoint(currentPoint);

                    // Stop if next point loops back to itself
                    if (_nextPoint.Equals(currentPoint))
                        _nextPoint = null;

                    yield return currentPoint;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}