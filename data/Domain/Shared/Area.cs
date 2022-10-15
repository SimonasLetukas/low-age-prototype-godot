﻿namespace low_age_data.Domain.Shared
{
    public struct Area
    {
        public Area(Vector2<int> size)
        {
            Start = new Vector2<int>(0, 0);
            Size = size;
        }

        public Area(Vector2<int> start, Vector2<int> size)
        {
            Start = start;
            Size = size;
        }

        /// <summary>
        /// Starting coordinates. Starts from 0.
        /// </summary>
        public Vector2<int> Start { get; }
        
        /// <summary>
        /// Size of the area. Starts from 1.
        /// </summary>
        public Vector2<int> Size { get; }
    }
}