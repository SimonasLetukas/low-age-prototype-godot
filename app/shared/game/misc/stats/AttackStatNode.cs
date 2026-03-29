using System;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Modifications;
using Newtonsoft.Json;

public partial class AttackStatNode : StatNode, INodeFromBlueprint<AttackStat>
{
	private const string ScenePath = @"res://app/shared/game/misc/stats/AttackStatNode.tscn";
	private static AttackStatNode Instance() => (AttackStatNode) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static AttackStatNode InstantiateAsChild(AttackStat blueprint, Node parentNode)
	{
		var stat = Instance();
		parentNode.AddChild(stat);
		stat.SetBlueprint(blueprint);
		return stat;
	}

	public string DisplayName => Blueprint.DisplayName;
	public bool IsMelee => Blueprint.AttackType.Equals(AttackType.Melee);
	public bool IsRanged => Blueprint.AttackType.Equals(AttackType.Ranged);
	public int MinimumDistance => GetModified(_baseMinimumDistance, _baseMinimumDistance, Modifications
			.Where(m => m is AttackModification a && a.Attribute.Equals(AttackAttribute.MinDistance)))
		.NewMax;
	public int MaximumDistance => GetModified(_baseMaximumDistance, _baseMaximumDistance, Modifications
			.Where(m => m is AttackModification a && a.Attribute.Equals(AttackAttribute.MaxDistance)))
		.NewMax;

	public int Damage => Math.Max(MaxAmount, 1);
	public bool HasBonusDamage => BonusTo is not null && BonusDamage > 0;
	public ActorAttribute? BonusTo { get; private set; }
	public int BonusDamage => GetModified(_baseBonusDamage, _baseBonusDamage, Modifications
			.Where(m => m is AttackModification a && a.Attribute.Equals(AttackAttribute.BonusAmount)))
		.NewMax;
	
	private AttackStat Blueprint { get; set; } = null!;
	
	private int _baseMinimumDistance;
	private int _baseMaximumDistance;
	private int _baseBonusDamage;

	public void SetBlueprint(AttackStat blueprint)
	{
		base.SetBlueprint(blueprint);
		Blueprint = blueprint;
		
		BonusTo = blueprint.BonusTo;

		_baseMinimumDistance = blueprint.MinimumDistance;
		_baseMaximumDistance = blueprint.MaximumDistance;
		_baseBonusDamage = blueprint.BonusAmount;
	}
	
	public override void Apply(Change what, float amount) => Apply(
		new AttackModification(what, amount, Blueprint.AttackType, AttackAttribute.MaxAmount), 
		true);

	public override void Apply(Modification modification, bool applyToBaseValue)
	{
		if (modification is not AttackModification attackModification 
		    || attackModification.AttackType.Equals(Blueprint.AttackType) is false)
			return;

		if (applyToBaseValue)
		{
			switch (attackModification.Attribute)
			{
				case var _ when attackModification.Attribute.Equals(AttackAttribute.MaxAmount):
					var (_, newMax) = GetUpdated(attackModification.Change, attackModification.Amount, 
						BaseCurrentAmount, BaseMaxAmount);
					BaseCurrentAmount = newMax;
					BaseMaxAmount = newMax;
					break;
				
				case var _ when attackModification.Attribute.Equals(AttackAttribute.MinDistance):
					var (_, newMinDistance) = GetUpdated(attackModification.Change, attackModification.Amount, 
						_baseMinimumDistance, _baseMinimumDistance);
					_baseMinimumDistance = newMinDistance;
					break;
				
				case var _ when attackModification.Attribute.Equals(AttackAttribute.MaxDistance):
					var (_, newMaxDistance) = GetUpdated(attackModification.Change, attackModification.Amount, 
						_baseMaximumDistance, _baseMaximumDistance);
					_baseMaximumDistance = newMaxDistance;
					break;
				
				case var _ when attackModification.Attribute.Equals(AttackAttribute.BonusAmount):
					var (_, newBonusDamage) = GetUpdated(attackModification.Change, attackModification.Amount, 
						_baseBonusDamage, _baseBonusDamage);
					_baseBonusDamage = newBonusDamage;
					break;
			}
		}
		else
		{
			if (Log.VerboseDebugEnabled)
				Log.Info(nameof(AttackStatNode), nameof(Apply), 
					$"Adding modification '{JsonConvert.SerializeObject(attackModification)}' to " +
					$"modifications '{JsonConvert.SerializeObject(Modifications)}'.");
			
			Modifications.Add(attackModification);
		}
		
		base.Apply(modification, applyToBaseValue);
	}
	
	public override void Remove(Modification modification)
	{
		if (modification is not AttackModification attackModification 
		    || attackModification.AttackType.Equals(Blueprint.AttackType) is false)
			return;
		
		if (Log.VerboseDebugEnabled)
			Log.Info(nameof(AttackStatNode), nameof(Remove), 
				$"Removing modification '{JsonConvert.SerializeObject(attackModification)}' from " +
				$"modifications '{JsonConvert.SerializeObject(Modifications)}'.");
		
		Modifications.Remove(attackModification);
		
		base.Remove(modification);
	}

	protected override float GetCurrent() => GetMax();

	protected override int GetMax()
	{
		var modifications = Modifications.Where(m => m is AttackModification a 
		                                             && a.Attribute.Equals(AttackAttribute.MaxAmount));
		
		return GetModified(BaseCurrentAmount, BaseMaxAmount, modifications).NewMax;
	}
}
