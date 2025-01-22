using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Tiles;

namespace LowAgeData.Domain.Logic
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
            AmountOfTilesRequired = amountOfTilesRequired ?? 1;
        }
        
        /// <summary>
        /// Specify the <see cref="Tile"/> to be targeted by this <see cref="TileCondition"/>.
        /// </summary>
        public TileId ConditionedTile { get; }
        
        /// <summary>
        /// How many <see cref="Tile"/>s should be found for this condition to return true. 
        /// </summary>
        public int AmountOfTilesRequired { get; }
    }
}