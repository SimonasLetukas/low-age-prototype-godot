using Godot;

public partial class InfoTextRow : RichTextLabel
{
    public const string ScenePath = @"res://app/client/game/interface/info-text/InfoTextRow.tscn";
    public static InfoTextRow Instance() => (InfoTextRow) GD.Load<PackedScene>(ScenePath).Instantiate();
}
