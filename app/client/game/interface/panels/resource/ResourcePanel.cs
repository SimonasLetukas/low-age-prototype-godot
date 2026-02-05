using System.Linq;
using Godot;

public partial class ResourcePanel : GridContainer
{
	public override void _Ready()
	{
		base._Ready();

		Reset();

		var player = Players.Instance.Current;
		var blueprint = Data.Instance.Blueprint;
		var faction = blueprint.Factions.FirstOrDefault(f => f.Id.Equals(player.Faction));
		if (faction is null)
			return;
		
		foreach (var resourceId in faction.AvailableResources)
		{
			var resource = blueprint.Resources.FirstOrDefault(r => r.Id.Equals(resourceId));
			if (resource is null)
				continue;

			ResourceItem.InstantiateAsChild(this, resource, player);
		}
	}

	private void Reset()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
	}
}
