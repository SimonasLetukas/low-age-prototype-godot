namespace low_age_data.Domain.Common
{
    public interface IDisplayable
    {
        /// <summary>
        /// Location to sprite which is displayed in-game. Example: "res://assets/sprites/units/revs/slave indexed 1x1.png".
        /// A value of null means that nothing will be displayed.
        /// </summary>
        public string? Sprite { get; }
    }
}