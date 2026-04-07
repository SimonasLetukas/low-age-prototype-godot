using LowAgeCommon;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Factions;
using LowAgeData.Domain.Researches;
using Research = LowAgeData.Domain.Researches.Research;

namespace LowAgeData.Collections;

public static class ResearchesCollection
{
    public static List<Research> Get()
    {
        return
        [
            new Research(
                id: ResearchId.Revelators.PoisonedSlits,
                displayName: nameof(ResearchId.Revelators.PoisonedSlits).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Quickdraw.Cripple).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Quickdraw).CamelCaseToWords()}.",
                faction: FactionId.Revelators),
            
            new Research(
                id: ResearchId.Revelators.RadioactiveChyme,
                displayName: nameof(ResearchId.Revelators.RadioactiveChyme).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Gorger.Disease).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Gorger).CamelCaseToWords()}.",
                faction: FactionId.Revelators),
            
            new Research(
                id: ResearchId.Revelators.SpikedRope,
                displayName: nameof(ResearchId.Revelators.SpikedRope).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Camou.Infiltration).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Camou).CamelCaseToWords()}.",
                faction: FactionId.Revelators),
            
            new Research(
                id: ResearchId.Revelators.QuestionableCargo,
                displayName: nameof(ResearchId.Revelators.QuestionableCargo).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Pyre.PhantomMenace).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Pyre).CamelCaseToWords()}.",
                faction: FactionId.Revelators),
            
            new Research(
                id: ResearchId.Revelators.HumanfleshRations,
                displayName: nameof(ResearchId.Revelators.HumanfleshRations).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Mummy.LeapOfHunger).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Mummy).CamelCaseToWords()}.",
                faction: FactionId.Revelators),
            
            new Research(
                id: ResearchId.Revelators.AdaptiveDigestion,
                displayName: nameof(ResearchId.Revelators.AdaptiveDigestion).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Roach.CorrosiveSpit).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Roach).CamelCaseToWords()}.",
                faction: FactionId.Revelators),
            
            new Research(
                id: ResearchId.Uee.FusionCoreUpgrade,
                displayName: nameof(ResearchId.Uee.FusionCoreUpgrade).CamelCaseToWords(),
                description: $"Upgrades {nameof(StructureId.BatteryCore).CamelCaseToWords()} to " +
                             $"{nameof(StructureId.FusionCore).CamelCaseToWords()}.",
                faction: FactionId.Uee),
            
            new Research(
                id: ResearchId.Uee.CelestiumCoreUpgrade,
                displayName: nameof(ResearchId.Uee.CelestiumCoreUpgrade).CamelCaseToWords(),
                description: $"Upgrades {nameof(StructureId.FusionCore).CamelCaseToWords()} to " +
                             $"{nameof(StructureId.CelestiumCore).CamelCaseToWords()}.",
                faction: FactionId.Uee),
            
            new Research(
                id: ResearchId.Uee.HeightenedConductivity,
                displayName: nameof(ResearchId.Uee.HeightenedConductivity).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.PowerPole.ImprovedPowerGrid).CamelCaseToWords()} ability for " +
                             $"{nameof(StructureId.PowerPole).CamelCaseToWords()}.",
                faction: FactionId.Uee),
            
            new Research(
                id: ResearchId.Uee.HoverboardReignition,
                displayName: nameof(ResearchId.Uee.HoverboardReignition).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Horrior.Mount).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Horrior).CamelCaseToWords()}.",
                faction: FactionId.Uee),
            
            new Research(
                id: ResearchId.Uee.ExplosiveShrapnel,
                displayName: nameof(ResearchId.Uee.ExplosiveShrapnel).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Mortar.PiercingBlast).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Mortar).CamelCaseToWords()}.",
                faction: FactionId.Uee),
            
            new Research(
                id: ResearchId.Uee.MdPractice,
                displayName: nameof(ResearchId.Uee.MdPractice).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Hawk.HealthKit).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Hawk).CamelCaseToWords()}.",
                faction: FactionId.Uee),
            
            new Research(
                id: ResearchId.Uee.CelestiumCoatedMaterials,
                displayName: nameof(ResearchId.Uee.CelestiumCoatedMaterials).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Radar.RadioLocation).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Radar).CamelCaseToWords()}.",
                faction: FactionId.Uee),
            
            new Research(
                id: ResearchId.Uee.HardenedMatrix,
                displayName: nameof(ResearchId.Uee.HardenedMatrix).CamelCaseToWords(),
                description: $"Unlocks {nameof(AbilityId.Vessel.Fortify).CamelCaseToWords()} ability for " +
                             $"{nameof(UnitId.Vessel).CamelCaseToWords()}.",
                faction: FactionId.Uee),
        ];
    }
}