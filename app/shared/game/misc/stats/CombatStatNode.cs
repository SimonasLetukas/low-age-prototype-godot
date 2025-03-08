using Godot;
using LowAgeData.Domain.Common;

public partial class CombatStatNode : StatNode, INodeFromBlueprint<CombatStat>
{
	public const string ScenePath = @"res://app/shared/game/misc/stats/CombatStatNode.tscn";
	public static CombatStatNode Instance() => (CombatStatNode) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static CombatStatNode InstantiateAsChild(CombatStat blueprint, Node parentNode)
	{
		var stat = Instance();
		parentNode.AddChild(stat);
		stat.SetBlueprint(blueprint);
		return stat;
	}

	public StatType StatType { get; private set; } = null!;
	
	private CombatStat Blueprint { get; set; } = null!;

	public void SetBlueprint(CombatStat blueprint)
	{
		base.SetBlueprint(blueprint);
		Blueprint = blueprint;
		
		StatType = blueprint.CombatType;
	}
}
