using System;
using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Entities.Actors;
using LowAgeData.Domain.Tiles;

namespace LowAgeData.Domain.Behaviours
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
            IList<HighGroundArea> path,
            bool closingEnabled) 
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
            ClosingEnabled = closingEnabled;
        }
        
        /// <summary>
        /// Configures a path from low to high ground, this path has to be inside the area (size) of the
        /// <see cref="Actor"/>. The first path element connects to all adjacent high ground <see cref="Tile"/>s;
        /// the last element in the list connects to all adjacent low ground <see cref="Tile"/>s; the path elements
        /// between the first and the last connect to each other in sequence, and also to any adjacent high-ground
        /// within ascension tolerance. 
        /// </summary>
        public IList<HighGroundArea> Path { get; }
        
        /// <summary>
        /// If true, when allies occupy the <see cref="HighGroundArea"/>s in <see cref="Path"/>, the
        /// <see cref="Ascendable"/> is considered 'closed' for enemies. When enemies occupy that area, the
        /// <see cref="Ascendable"/> is considered 'opened' for allies and enemies. The 'closed' and 'opened' states
        /// define what is allowed to pass through the last element of <see cref="Path"/> and the low ground that it
        /// connects to.
        /// <p></p>
        /// If false, the <see cref="Ascendable"/> has constant 'opened' state and is accessible the same way for
        /// both allies and enemies.
        /// </summary>
        public bool ClosingEnabled { get; }
    }
}