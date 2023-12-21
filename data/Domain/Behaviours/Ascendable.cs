using System;
using System.Collections.Generic;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Tiles;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Can be added to an <see cref="Actor"/> to create and specify an entry point of going up to the <see cref="HighGround"/>.
    /// </summary>
    public class Ascendable : Behaviour
    {
        public Ascendable(
            BehaviourId id,
            string displayName, 
            string description,
            string sprite,
            IList<Area> path) 
            : base(
                id, 
                displayName, 
                description, 
                sprite,
                EndsAt.Death,
                Alignment.Neutral)
        {
            Path = path.Count == 0
                ? throw new ArgumentOutOfRangeException(nameof(path), 
                    $"Must contain at least one {nameof(path)} element")
                : path;
        }
        
        /// <summary>
        /// Configures a path from low to high ground, this path has to be inside the area (size) of the
        /// <see cref="Actor"/>. The first path element connects to all adjacent high ground <see cref="Tile"/>s;
        /// the last element in the list connects to all adjacent low ground <see cref="Tile"/>s; the path elements
        /// between the first and the last connect to each other in sequence. Only the first element can also be layered
        /// with a <see cref="HighGround"/>'s <see cref="HighGround.HighGroundAreas"/>. 
        /// </summary>
        public IList<Area> Path { get; }
    }
}