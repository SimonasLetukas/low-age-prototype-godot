using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;

namespace LowAgeData.Domain.Abilities
{
    /// <summary>
    /// Used for opening a selection of <see cref="ResearchId"/> to be researched by paying the <see cref="Payment"/>.
    /// </summary>
    public class Research : Ability
    {
        public Research(
            AbilityId id,
            string displayName,
            string description,
            string sprite,
            IList<Selection<ResearchId>> selection,
            bool casterConsumesAction = false,
            bool canHelp = false,
            float? helpEfficiency = null, 
            IList<ResearchId>? researchNeeded = null)
            : base(
                id,
                TurnPhase.Planning,
                researchNeeded ?? new List<ResearchId>(),
                true,
                displayName,
                description,
                sprite)
        {
            Selection = selection;
            CasterConsumesAction = casterConsumesAction;
            CanHelp = canHelp;
            HelpEfficiency = helpEfficiency ?? 1;
        }

        /// <summary>
        /// Selection of <see cref="Research"/> to be unlocked. To lock it again, use
        /// <see cref="Effects.ModifyResearch"/>.
        /// </summary>
        public IList<Selection<ResearchId>> Selection { get; }
        
        /// <summary>
        /// <para>
        /// If true, to continue paying for the <see cref="ResearchId"/> item, the caster of this
        /// <see cref="Research"/> <see cref="Ability"/> has to spend its action, effectively meaning that only one
        /// <see cref="ResearchId"/> can be worked on at the same time. 
        /// </para>
        /// <para>
        /// False by default.
        /// </para>
        /// </summary>
        public bool CasterConsumesAction { get; }
        
        /// <summary>
        /// <para>
        /// If true, multiple <see cref="Entity"/>ies can cast their <see cref="Research"/> on the same 
        /// <see cref="ResearchId"/> (if they have that <see cref="ResearchId"/> as part of their
        /// <see cref="Selection"/>), providing additional amount of production according to
        /// <see cref="HelpEfficiency"/>.
        /// </para>
        /// <para>
        /// False by default.
        /// </para>
        /// </summary>
        public bool CanHelp { get; }

        /// <summary>
        /// <para>
        /// If <see cref="CanHelp"/> is true, this describes the factor of production each additional helper provides
        /// compared to the previous one.
        /// </para>
        /// <para>
        /// E.g. with 0.5 efficiency first builder provides 100% production, second 50%, third 25%, etc.
        /// </para>
        /// <para>
        /// 1.0 by default.
        /// </para>
        /// </summary>
        public float HelpEfficiency { get; }
    }
}