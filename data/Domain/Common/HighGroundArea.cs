using LowAgeCommon;

namespace LowAgeData.Domain.Common
{
    public struct HighGroundArea
    {
        public HighGroundArea(Area area, Vector2Int spriteOffset)
        {
            Area = area;
            SpriteOffset = spriteOffset;
        }
        
        public Area Area { get; }
        
        /// <summary>
        /// How much the entities need to be offset to appear on top of this high-ground area.
        /// </summary>
        public Vector2Int SpriteOffset { get; }
    }
}