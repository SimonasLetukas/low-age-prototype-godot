using System.Collections.Generic;
using LowAgeCommon;
using LowAgeCommon.Extensions;

namespace DijkstraMap.Methods
{
    public static class Grids
    {
        /// <summary>
        /// Adds a square grid of connected points.
        /// </summary>
        /// <param name="dijkstraMap"><see cref="Dijkstra"/>.</param>
        /// <param name="width">Width of the grid.</param>
        /// <param name="height">Height of the grid.</param>
        /// <param name="initialOffset">Specifies offset of the grid. Default = (0, 0)</param>
        /// <param name="defaultTerrain"><see cref="Terrain"/> to use for all points of the grid. Default =
        /// <see cref="Terrain.Default"/></param>
        /// <param name="orthogonalCost">Specifies cost of orthogonal connections (up, down, right and left).
        /// If <see cref="orthogonalCost"/> is <see cref="float.PositiveInfinity"/>, orthogonal connections
        /// are disabled.</param>
        /// <param name="diagonalCost">Specifies cost of diagonal connections. If <see cref="diagonalCost"/> is
        /// <see cref="float.PositiveInfinity"/>, diagonal connections are disabled.</param>
        /// <returns>Returns a Dictionary, where keys are coordinates of points (Vector2) and values are the
        /// corresponding point IDs.</returns>
        public static Dictionary<Vector2<int>, PointId> AddSquareGrid(
            this Dijkstra dijkstraMap,
            int width, int height, 
            Vector2<int>? initialOffset = null, 
            Terrain? defaultTerrain = null, 
            float? orthogonalCost = null, 
            float? diagonalCost = null)
        {
            var offset = initialOffset ?? new Vector2<int>(0, 0);
            var terrain = defaultTerrain ?? Terrain.Default;
            var orthogonal = orthogonalCost ?? 1.0f;
            var diagonal = diagonalCost ?? float.PositiveInfinity;
            
            var posToId = AddGridInternal(dijkstraMap, offset.X, offset.Y, 
                width, height, terrain);

            // Orthogonal directions
            var orthos = new[]
            {
                new Vector2<int>(1, 0), new Vector2<int>(-1, 0),
                new Vector2<int>(0, 1), new Vector2<int>(0, -1)
            };

            // Diagonal directions
            var diags = new[]
            {
                new Vector2<int>(1, 1), new Vector2<int>(-1, 1),
                new Vector2<int>(1, -1), new Vector2<int>(-1, -1)
            };

            foreach (var (pos, id1) in posToId)
            {
                // Connect orthogonal neighbors
                if (orthogonal < float.PositiveInfinity)
                {
                    foreach (var offs in orthos)
                    {
                        var neighborPos = pos + offs;
                        if (posToId.TryGetValue(neighborPos, out var id2))
                        {
                            dijkstraMap.ConnectPoints(id1, id2, orthogonal, false);
                        }
                    }
                }

                // Connect diagonal neighbors
                if (diagonal < float.PositiveInfinity)
                {
                    foreach (var offs in diags)
                    {
                        var neighborPos = pos + offs;
                        if (posToId.TryGetValue(neighborPos, out var id2))
                        {
                            dijkstraMap.ConnectPoints(id1, id2, diagonal, false);
                        }
                    }
                }
            }

            return posToId;
        }

        /// <summary>
        /// Hex grid is in the "pointy" orientation by default.
        /// 
        /// To switch to "flat" orientation, swap <see cref="width"/> and <see cref="height"/>, and switch
        /// `x` and `y` coordinates of the keys in the returned Dictionary.
        /// </summary>
        /// <param name="dijkstraMap"><see cref="Dijkstra"/>.</param>
        /// <param name="width">Width of the grid.</param>
        /// <param name="height">Height of the grid.</param>
        /// <param name="initialOffset">Specifies offset of the grid. Default = (0, 0)</param>
        /// <param name="defaultTerrain"><see cref="Terrain"/> to use for all points of the grid.</param>
        /// <param name="weight">Specifies cost of connections. Default = 1.0</param>
        /// <returns>Returns a Dictionary, where keys are coordinates of points (Vector2) and values are their
        /// corresponding point IDs.</returns>
        /// <example> This is what `add_hexagonal_grid(2, 3, (5, 6), ...)` would produce:
        /// <code>
        ///    / \     / \
        ///  /     \ /     \
        /// |  5,6  |  6,6  |
        ///  \     / \     / \
        ///    \ /     \ /     \
        ///     |  5,7  |  6,7  |
        ///    / \     / \     /
        ///  /     \ /     \ /
        /// |  5,8  |  6,8  |
        ///  \     / \     /
        ///    \ /     \ /
        /// </code></example>
        public static Dictionary<Vector2<int>, PointId> AddHexagonalGrid(
            this Dijkstra dijkstraMap,
            int width, int height, 
            Vector2<int>? initialOffset, 
            Terrain defaultTerrain, 
            float? weight)
        {
            var offset = initialOffset ?? new Vector2<int>(0, 0);
            var posToId = AddGridInternal(dijkstraMap, offset.X, offset.Y, 
                width, height, defaultTerrain);

            var connectionWeight = weight ?? 1.0f;

            // Hexagonal connection patterns
            var connections = new[]
            {
                new[]
                {
                    new Vector2<int>(-1, -1), new Vector2<int>(0, -1),
                    new Vector2<int>(-1, 0), new Vector2<int>(1, 0),
                    new Vector2<int>(-1, 1), new Vector2<int>(0, 1)
                }, // Even columns
                new[]
                {
                    new Vector2<int>(0, -1), new Vector2<int>(1, -1),
                    new Vector2<int>(-1, 0), new Vector2<int>(1, 0),
                    new Vector2<int>(0, 1), new Vector2<int>(1, 1)
                } // Odd columns
            };

            foreach (var (pos, id1) in posToId)
            {
                if ((connectionWeight < float.PositiveInfinity) is false) 
                    continue;
                
                var neighborOffsets = connections[pos.X % 2];
                foreach (var offs in neighborOffsets)
                {
                    var neighborPos = pos + offs;
                    if (posToId.TryGetValue(neighborPos, out var id2))
                    {
                        dijkstraMap.ConnectPoints(id1, id2, connectionWeight, false);
                    }
                }
            }

            return posToId;
        }
        
        private static Dictionary<Vector2<int>, PointId> AddGridInternal(this Dijkstra dijkstraMap, 
            int xOffset, int yOffset, int width, int height, Terrain defaultTerrain)
        {
            var id = dijkstraMap.GetAvailableId();
            var posToId = new Dictionary<Vector2<int>, PointId>();

            for (var x = xOffset; x < width + xOffset; x++)
            {
                for (var y = yOffset; y < height + yOffset; y++)
                {
                    var pos = new Vector2<int>(x, y);
                    id = dijkstraMap.GetAvailableId(new PointId(id.Value + 1));
                    dijkstraMap.AddOrReplacePoint(id, defaultTerrain);
                    posToId[pos] = id;
                }
            }

            return posToId;
        }
    }
}