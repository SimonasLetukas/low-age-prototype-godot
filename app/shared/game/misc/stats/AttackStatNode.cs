using Godot;
using LowAgeData.Domain.Common;

public partial class AttackStatNode : StatNode, INodeFromBlueprint<AttackStat>
{
	public const string ScenePath = @"res://app/shared/game/misc/stats/AttackStatNode.tscn";
	public static AttackStatNode Instance() => (AttackStatNode) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static AttackStatNode InstantiateAsChild(AttackStat blueprint, Node parentNode)
	{
		var stat = Instance();
		parentNode.AddChild(stat);
		stat.SetBlueprint(blueprint);
		return stat;
	}

	public bool IsMelee => Blueprint.AttackType.Equals(Attacks.Melee);
	public bool IsRanged => Blueprint.AttackType.Equals(Attacks.Ranged);
	public int MinimumDistance { get; private set; }
	public int MaximumDistance { get; private set; }
	public int Damage => (int)MaxAmount;
	public bool HasBonusDamage => BonusTo is not null || BonusDamage is 0;
	public ActorAttribute? BonusTo { get; private set; }
	public int BonusDamage { get; private set; }
	
	private AttackStat Blueprint { get; set; } = null!;
	
	public void SetBlueprint(AttackStat blueprint)
	{
		base.SetBlueprint(blueprint);
		Blueprint = blueprint;
		
		MinimumDistance = blueprint.MinimumDistance;
		MaximumDistance = blueprint.MaximumDistance;
		BonusTo = blueprint.BonusTo;
		BonusDamage = blueprint.BonusAmount;
	}
}
