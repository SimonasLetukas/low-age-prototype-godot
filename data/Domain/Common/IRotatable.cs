using low_age_data.Shared;
using low_age_prototype_common;

namespace low_age_data.Domain.Common
{
    /// <summary>
    /// <see cref="IDisplayable.Sprite"/> and <see cref="IDisplayable.CenterOffset"/> refer to the front side of the
    /// object. To be able to rotate the object additional back side sprites are needed. The last two rotations are
    /// then done by flipping the sprite horizontally.
    /// </summary>
    public interface IRotatable : IDisplayable
    {
        /// <summary>
        /// Location to the back side sprite which is displayed in-game when rotated. Example:
        /// "res://assets/sprites/units/revs/slave indexed 1x1.png". A value of null means that nothing will be
        /// displayed.
        /// </summary>
        public string? BackSideSprite { get; }
        
        /// <summary>
        /// Counting from top-left, how many pixels to move to center the back side sprite.
        /// </summary>
        public Vector2<int> BackSideCenterOffset { get; }
    }
}