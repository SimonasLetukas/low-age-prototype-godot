namespace DijkstraMap.Methods
{
    public static class Setters
    {
        /// <summary>
        /// Adds a new point with the given ID and terrain type to the graph.
        /// </summary>
        /// <param name="dijkstraMap"><see cref="Dijkstra"/></param>
        /// <param name="id"><see cref="PointId"/></param>
        /// <param name="terrain"><see cref="Terrain"/>. Default = -1</param>
        /// <returns>False if a point with that ID already exists</returns>
        public static bool AddPoint(this Dijkstra dijkstraMap, PointId id, Terrain? terrain = null)
        {
            terrain = terrain ?? Terrain.Default;
            
            if (dijkstraMap.HasPoint(id))
                return false;
            
            dijkstraMap.AddOrReplacePoint(id, terrain.Value);
            return true;
        }

        /// Adds or replaces a point with the given ID and terrain type in the graph.
        internal static void AddOrReplacePoint(this Dijkstra dijkstraMap, PointId id, Terrain terrain)
        {
            dijkstraMap.Points[id] = new PointInfo(terrain);
        }

        /// Removes a point from the graph along with all its connections. Returns true if the point was found
        /// and removed.
        public static bool RemovePoint(this Dijkstra dijkstraMap, PointId id)
        {
            dijkstraMap.DisabledPoints.Remove(id);
            if (dijkstraMap.Points.TryGetValue(id, out var pointInfo) is false)
                return false;

            foreach (var neighbor in pointInfo.Connections.Keys)
            {
                if (dijkstraMap.Points.TryGetValue(neighbor, out var neighborInfo))
                {
                    neighborInfo.ReverseConnections.Remove(id);
                }
            }

            foreach (var neighbor in pointInfo.ReverseConnections.Keys)
            {
                if (dijkstraMap.Points.TryGetValue(neighbor, out var neighborInfo))
                {
                    neighborInfo.Connections.Remove(id);
                }
            }

            dijkstraMap.Points.Remove(id);
            return true;
        }

        /// Disables a point from pathfinding. Returns true if the point was found and was successfully disabled.
        public static bool DisablePoint(this Dijkstra dijkstraMap, PointId id)
        {
            if (dijkstraMap.HasPoint(id) is false 
                || dijkstraMap.DisabledPoints.Contains(id))
                return false;
            
            dijkstraMap.DisabledPoints.Add(id);
            return true;
        }

        /// Enables a previously disabled point for pathfinding. Returns true if the point was found and was
        /// successfully enabled.
        public static bool EnablePoint(this Dijkstra dijkstraMap, PointId id)
        {
            if (dijkstraMap.HasPoint(id) is false 
                || dijkstraMap.DisabledPoints.Contains(id) is false)
                return false;
            
            dijkstraMap.DisabledPoints.Remove(id);
            return true;
        }

        /// Adds a connection with a specified weight between two points. Returns false if one or both points
        /// were not found.
        public static bool ConnectPoints(this Dijkstra dijkstraMap, PointId source, PointId target, 
            float? weight = null, bool? bidirectional = true)
        {
            if (weight is null) weight = 1.0f;
            if (bidirectional is null) bidirectional = true;

            if (dijkstraMap.HasPoint(source) is false || dijkstraMap.HasPoint(target) is false)
                return false;

            if (bidirectional == true)
            {
                dijkstraMap.ConnectPoints(source, target, weight, false);
                dijkstraMap.ConnectPoints(target, source, weight, false);
                return true;
            }

            dijkstraMap.Points[source].Connections[target] = new Weight(weight.Value);
            dijkstraMap.Points[target].ReverseConnections[source] = new Weight(weight.Value);
            return true;
        }

        /// Removes a connection between two points. Returns false if one or both points were not found.
        public static bool RemoveConnection(this Dijkstra dijkstraMap, PointId source, PointId target, 
            bool? bidirectional = true)
        {
            if (bidirectional is null) bidirectional = true;

            if (dijkstraMap.HasPoint(source) is false || dijkstraMap.HasPoint(target) is false)
                return false;

            if (bidirectional == true)
            {
                dijkstraMap.RemoveConnection(source, target, false);
                dijkstraMap.RemoveConnection(target, source, false);
                return true;
            }

            dijkstraMap.Points[source].Connections.Remove(target);
            dijkstraMap.Points[target].ReverseConnections.Remove(source);
            return true;
        }

        /// Sets the terrain type for a specific point. Returns false if the point was not found.
        public static bool SetTerrainForPoint(this Dijkstra dijkstraMap, PointId id, Terrain terrain)
        {
            if (dijkstraMap.Points.TryGetValue(id, out var pointInfo) is false)
                return false;
            
            pointInfo.Terrain = terrain;
            return true;
        }
    }
}