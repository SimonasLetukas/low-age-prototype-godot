using System;
using System.Collections.Generic;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Entities.Tiles;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

namespace low_age_data.Domain.Behaviours
{
    /// <summary>
    /// Can be added to an <see cref="Actor"/> to create and specify an entry point of going up to the <see cref="HighGround"/>.
    /// </summary>
    public class Ascendable : Behaviour
    {
        public Ascendable(
            BehaviourName name,
            string displayName, 
            string description,
            IList<Area> path) : base(name, $"{nameof(Behaviour)}.{nameof(Ascendable)}", displayName, description, EndsAt.Death)
        {
            Path = path.Count == 0
                ? throw new ArgumentOutOfRangeException(nameof(path), 
                    $"Must contain at least one {nameof(path)} element")
                : path;
        }
        
        /// <summary>
        /// Configures a path from low to high ground, this path has to be inside the area (size) of the
        /// <see cref="Actor"/>. The first path element connects to all adjacent low ground <see cref="Tile"/>s;
        /// the last element in the list connects to all adjacent high ground <see cref="Tile"/>s; the path elements
        /// between the first and the last connect to each other in sequence. Only the last element can also be layered
        /// with a <see cref="HighGround"/>'s <see cref="HighGround.HighGroundAreas"/>. 
        /// </summary>
        public IList<Area> Path { get; }
    }
}