using low_age_data.Domain.Common.Flags;
using low_age_data.Domain.Tiles;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// <see cref="Condition"/> used to target <see cref="Tile"/>s.
    /// </summary>
    public class TileCondition : Condition
    {
        public TileCondition(
            ConditionFlag conditionFlag,
            TileId conditionedTile,
            int? amountOfTilesRequired = null) : base(conditionFlag)
        {
            ConditionedTile = conditionedTile;
            AmountOfEntitiesRequired = amountOfTilesRequired ?? 1;
        }
        
        /// <summary>
        /// Specify the <see cref="Tile"/> to be targeted by this <see cref="TileCondition"/>.
        /// </summary>
        public TileId ConditionedTile { get; }
        
        /// <summary>
        /// How many <see cref="Tile"/>s should be found for this condition to return true. 
        /// </summary>
        public int AmountOfEntitiesRequired { get; }
    }
}