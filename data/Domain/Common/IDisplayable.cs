using low_age_prototype_common;

{
    public interface IDisplayable
    {
        /// <summary>
        /// Location to sprite which is displayed in-game. Example: "res://assets/sprites/units/revs/slave indexed 1x1.png".
        /// A value of null means that nothing will be displayed.
        /// </summary>
        public string? Sprite { get; }
        
        /// <summary>
        /// Counting from top-left, how many pixels to move to center the sprite.
        /// </summary>
        public Vector2<int> CenterOffset { get; }
    }
}