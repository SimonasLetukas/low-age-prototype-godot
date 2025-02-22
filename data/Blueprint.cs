using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Entities.Doodads;
using LowAgeData.Domain.Factions;
using LowAgeData.Domain.Masks;
using LowAgeData.Domain.Resources;
using LowAgeData.Domain.Tiles;

namespace LowAgeData
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