using low_age_data.Shared;
using low_age_prototype_common;

namespace low_age_data.Domain.Common
{
    public struct HighGroundArea
    {
        public HighGroundArea(Area area, Vector2<int> spriteOffset)
        {
            Area = area;
            SpriteOffset = spriteOffset;
        }
        
        public Area Area { get; }
        
        /// <summary>
        /// How much the entities need to be offset to appear on top of this high-ground area.
        /// </summary>
        public Vector2<int> SpriteOffset { get; }
    }
}