using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Modifications;

public partial class CombatStatNode : StatNode, INodeFromBlueprint<CombatStat>
{
	private const string ScenePath = @"res://app/shared/game/misc/stats/CombatStatNode.tscn";
	private static CombatStatNode Instance() => (CombatStatNode) GD.Load<PackedScene>(ScenePath).Instantiate();
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

	public override void Apply(Change what, float amount) => Apply(
		new StatModification(what, amount, Blueprint.CombatType),
		true);

	public override void Apply(Modification modification, bool applyToBaseValue)
	{
		if (modification is not StatModification statModification 
		    || statModification.StatType.Equals(Blueprint.CombatType) is false)
			return;

		if (applyToBaseValue)
		{
			var (newAmount, newMax) = GetUpdated(statModification.Change, statModification.Amount, 
				BaseCurrentAmount, BaseMaxAmount);
			BaseCurrentAmount = newAmount;
			BaseMaxAmount = newMax;
		}
		else
		{
			Modifications.Add(statModification);
		}
		
		base.Apply(modification, applyToBaseValue);
	}

	public override void Remove(Modification modification)
	{
		if (modification is not StatModification statModification 
		    || statModification.StatType.Equals(Blueprint.CombatType) is false)
			return;
		
		Modifications.Remove(statModification);
		
		base.Remove(modification);
	}
}
