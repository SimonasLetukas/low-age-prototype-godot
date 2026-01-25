using Godot;

public partial class InitiativePossibleActionItem : Panel
{
	public const string ScenePath = @"res://app/client/game/interface/panels/initiative/InitiativePossibleActionItem.tscn";
	public static InitiativePossibleActionItem Instance() 
		=> (InitiativePossibleActionItem) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static InitiativePossibleActionItem InstantiateAsChild(Node parentNode)
	{
		var stat = Instance();
		parentNode.AddChild(stat);
		return stat;
	}
}
