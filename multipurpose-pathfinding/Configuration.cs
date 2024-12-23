using System;
using System.Collections.Generic;
using System.Linq;
using low_age_dijkstra;
using low_age_prototype_common;

namespace multipurpose_pathfinding
{
    public class Configuration
    {
        public Vector2<int> MapSize { get; set; } = new Vector2<int>(10, 10);
        public int BaseTerrain { get; set; } = 0;
        public Dictionary<int, float> TerrainWeights { get; set; } =
            new Dictionary<int, float>
            {
                { 0, 1.0f },
                { 1, float.PositiveInfinity },
                { 2, 2.0f }
            };
        public PathfindingSize MaxSizeForPathfinding { get; set; } = 3;
        public Team MaxNumberOfTeams { get; set; } = 1;
        public float DiagonalCost { get; set; } = (float)Math.Sqrt(2);
        public int HighGroundTolerance { get; set; } = 13; // works until approx 18 levels of ascension
        public int ImpassableIndex { get; set; } = -1;
        public int HighGroundIndex { get; set; } = -2;
        public bool DebugEnabled { get; set; }
        
        internal IEnumerable<Terrain> TerrainsInAscendingWeights => TerrainWeights
            .OrderBy(x => x.Value)
            .Select(x => new Terrain(x.Key));
    }
}