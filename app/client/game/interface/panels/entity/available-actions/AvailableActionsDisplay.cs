using Godot;
using System;
using System.Linq;

public partial class AvailableActionsDisplay : VFlowContainer
{
	[Flags]
	public enum AvailableActionType
	{
		Movement,
		MeleeAttack,
		RangedAttack,
		Ability
	}
	
	[Export] public Godot.Collections.Dictionary<AvailableActionType, string> IconPaths { get; set; } 
	
	public override void _Ready()
	{
		Reset();

		var economy = new ActionEconomy();
		economy.Restore();
		Populate(economy);
	}

	public void Populate(ActionEconomy actionEconomy)
	{
		Reset();
		
		for (var i = 0; i < actionEconomy.MeleeAttackActions; i++)
		{
			var itemScene = GD.Load<PackedScene>(AvailableActionItem.ScenePath);
			var item = itemScene.Instantiate<AvailableActionItem>();
			if (IconPaths.TryGetValue(AvailableActionType.MeleeAttack, out var icon) is false)
			{
				icon = IconPaths.Values.First();
			}
			item.Set(icon, "Melee attack action is available!");
			AddChild(item);
		}
		
		for (var i = 0; i < actionEconomy.RangedAttackActions; i++)
		{
			var itemScene = GD.Load<PackedScene>(AvailableActionItem.ScenePath);
			var item = itemScene.Instantiate<AvailableActionItem>();
			if (IconPaths.TryGetValue(AvailableActionType.RangedAttack, out var icon) is false)
			{
				icon = IconPaths.Values.First();
			}
			item.Set(icon, "Ranged attack action is available!");
			AddChild(item);
		}
		
		for (var i = 0; i < actionEconomy.AbilityActions; i++)
		{
			var itemScene = GD.Load<PackedScene>(AvailableActionItem.ScenePath);
			var item = itemScene.Instantiate<AvailableActionItem>();
			if (IconPaths.TryGetValue(AvailableActionType.Ability, out var icon) is false)
			{
				icon = IconPaths.Values.First();
			}
			item.Set(icon, "Ability action is available!");
			AddChild(item);
		}

		if (actionEconomy.CanMove)
		{
			var itemScene = GD.Load<PackedScene>(AvailableActionItem.ScenePath);
			var item = itemScene.Instantiate<AvailableActionItem>();
			if (IconPaths.TryGetValue(AvailableActionType.Movement, out var icon) is false)
			{
				icon = IconPaths.Values.First();
			}
			item.Set(icon, "Movement action is available!");
			AddChild(item);
		}
	}
	
	public void Reset()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
	}
}
