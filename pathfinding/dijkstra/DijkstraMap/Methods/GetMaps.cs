using System;
using System.Collections.Generic;
using System.Linq;

namespace DijkstraMap.Methods
{
    public static class GetMaps
    {
        /// Given a `point`, returns the next point along the shortest path if it exists; otherwise null.
        public static PointId GetDirectionAtPoint(this Dijkstra dijkstraMap, PointId point) 
            => dijkstraMap.ComputedInfo.TryGetValue(point, out var pointComputedInfo) 
                ? pointComputedInfo.Direction 
                : null;

        /// Returns the cost of the shortest path, or <see cref="Cost.Infinity"/> if not computed.
        public static Cost GetCostAtPoint(this Dijkstra dijkstraMap, PointId point) 
            => dijkstraMap.ComputedInfo.TryGetValue(point, out var pointComputedInfo) 
                ? pointComputedInfo.Cost 
                : Cost.Infinity;
        
        /// Returns the entire Dijkstra map of directions and costs.
        public static Dictionary<PointId, PointComputedInfo> GetDirectionAndCostMap(this Dijkstra dijkstraMap) 
            => dijkstraMap.ComputedInfo;

        /// Returns all points with costs between `minCost` and `maxCost` (inclusive), sorted by cost.
        public static IEnumerable<PointId> GetAllPointsWithCostBetween(this Dijkstra dijkstraMap, 
            float minCost, float maxCost)
        {
            // Custom binary search to find the start index
            var startIndex = dijkstraMap.SortedPoints.FindIndex(pointId => 
                dijkstraMap.GetCostAtPoint(pointId).Value >= minCost);
            if (startIndex == -1)
                return Array.Empty<PointId>(); // No points with cost >= minCost

            // Custom binary search to find the end index
            var endIndex = dijkstraMap.SortedPoints.FindLastIndex(pointId => 
                dijkstraMap.GetCostAtPoint(pointId).Value <= maxCost);
            if (endIndex == -1 || endIndex < startIndex)
                return Array.Empty<PointId>(); // No points within the specified range

            // Return the range of points
            return dijkstraMap.SortedPoints.Skip(startIndex).Take(endIndex - startIndex + 1).ToArray();
        }
    }
}