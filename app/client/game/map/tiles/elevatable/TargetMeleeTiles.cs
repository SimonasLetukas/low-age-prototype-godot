using Godot;

public partial class TargetMeleeTiles : TargetTiles
{
	public const string ScenePath = @"res://app/client/game/map/tiles/elevatable/TargetMeleeTiles.tscn";
	public static TargetMeleeTiles Instance() => (TargetMeleeTiles) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static TargetMeleeTiles InstantiateAsChild(Node parentNode)
	{
		var instance = Instance();
		parentNode.AddChild(instance);
		instance.Clear();
		return instance;
	}
}
