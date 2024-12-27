using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace low_age_prototype_common
{
    [Serializable]
    public struct Area
    {
        public Area(Vector2<int> size)
        {
            Start = new Vector2<int>(0, 0);
            Size = size;
        }
        
        [JsonConstructor]
        public Area(Vector2<int> start, Vector2<int> size)
        {
            Start = start;
            Size = size;
        }

        /// <summary>
        /// Starting coordinates. Starts from 0. Could also have negative values to indicate starting outside of
        /// the usual boundaries.
        /// </summary>
        public Vector2<int> Start { get; }
        
        /// <summary>
        /// Size of the area. Starts from 1.
        /// </summary>
        public Vector2<int> Size { get; }
        
        public override string ToString() => $"{Start}, {Size}";

        public IEnumerable<Vector2<int>> ToVectors() => IterateVector2Int.Positions(Start, Start + Size);
    }
}