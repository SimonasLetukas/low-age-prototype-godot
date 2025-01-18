using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Behaviours;

public partial class Behaviours : Node2D
{
    public IList<BehaviourNode> GetAll() => GetChildren().OfType<BehaviourNode>().ToList();
    public IList<BuildableNode> GetBuildables() => GetChildren().OfType<BuildableNode>().ToList();

    public void AddOnBuildBehaviours(IEnumerable<PassiveNode> passiveAbilities)
    {
        var allBehaviours = Data.Instance.Blueprint.Behaviours;
        foreach (var passive in passiveAbilities)
        {
            var onBuildBehaviourId = passive.GetOnBuildBehaviourOrDefault();
            
            if (onBuildBehaviourId is null)
                continue;

            var blueprint = allBehaviours.First(x => x.Id.Equals(onBuildBehaviourId));
            BuildableNode.InstantiateAsChild((Buildable)blueprint, this);
        }
    }

    public void RemoveAll<T>() where T : BehaviourNode
    {
        foreach (var behaviour in GetChildren().OfType<T>()) 
            behaviour.QueueFree();
    }
}
