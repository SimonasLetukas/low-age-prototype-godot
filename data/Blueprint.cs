using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Doodads;
using low_age_data.Domain.Factions;
using low_age_data.Domain.Masks;
using low_age_data.Domain.Resources;
using low_age_data.Domain.Tiles;

namespace low_age_data
{
    public class Blueprint
    {
        public IList<Resource> Resources { get; set; } = null!;
        public IList<Faction> Factions { get; set; } = null!;
        public EntityBlueprint Entities { get; set; } = null!;
        public IList<Tile> Tiles { get; set; } = null!;
        public IList<Ability> Abilities { get; set; } = null!;
        public IList<Effect> Effects { get; set; } = null!;
        public IList<Behaviour> Behaviours { get; set; } = null!;
        public IList<Mask> Masks { get; set; } = null!;
    }

    public class EntityBlueprint
    {
        public IList<Unit> Units { get; set; } = null!;
        public IList<Structure> Structures { get; set; } = null!;
        public IList<Doodad> Doodads { get; set; } = null!;
    }
}