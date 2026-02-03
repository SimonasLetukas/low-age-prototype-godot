using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;

public partial class Behaviours : Node2D
{
    private EntityNode Parent { get; set; } = null!;

    public override void _Ready()
    {
        base._Ready();
        Parent = (EntityNode)GetParent();
    }

    public IList<BehaviourNode> GetAll() => GetChildren().OfType<BehaviourNode>().ToList();
    public IList<BuildableNode> GetBuildables() => GetChildren().OfType<BuildableNode>().ToList();
    public IList<IPathfindingUpdatable> GetPathfindingUpdatables => GetChildren().OfType<IPathfindingUpdatable>().ToList();

    public void AddOnBuildBehaviours(IEnumerable<PassiveNode> passiveAbilities)
    {
        var allBehaviours = Data.Instance.Blueprint.Behaviours;
        foreach (var passive in passiveAbilities)
        {
            var onBuildBehaviourId = passive.GetOnBuildBehaviourOrDefault();
            
            if (onBuildBehaviourId is null)
                continue;

            var blueprint = allBehaviours.First(x => x.Id.Equals(onBuildBehaviourId));
            BuildableNode.InstantiateAsChild((Buildable)blueprint, this, Parent);
        }
    }

    public void AddBehaviours(IEnumerable<BehaviourId> behaviourIds, Effects history)
    {
        var allBehaviours = Data.Instance.Blueprint.Behaviours;
        foreach (var behaviourBlueprint in allBehaviours.Join(behaviourIds.Distinct(), ability => ability.Id, 
                     id => id, (ability, id) => ability))
        {
            AddBehaviour(behaviourBlueprint, history);
        }
    }

    public void AddBehaviour(BehaviourId behaviourId, Effects history)
    {
        var allBehaviours = Data.Instance.Blueprint.Behaviours;
        var behaviourBlueprint = allBehaviours.FirstOrDefault(x => x.Id.Equals(behaviourId));
        AddBehaviour(behaviourBlueprint, history);
    }

    public void RemoveAll<T>() where T : BehaviourNode
    {
        foreach (var behaviour in GetChildren().OfType<T>()) 
            behaviour.QueueFree();
    }

    /// <b>Avoid calling directly</b>, use other overloads instead. Method is public to make automated testing easier.
    public BehaviourNode AddBehaviour(Behaviour behaviourBlueprint, Effects history)
    {
        switch (behaviourBlueprint)
        {
            case Ascendable ascendable:
                return AscendableNode.InstantiateAsChild(ascendable, this, history, Parent);
            
            case HighGround highGround:
                return HighGroundNode.InstantiateAsChild(highGround, this, history, Parent);
            
            case Income income:
                return IncomeNode.InstantiateAsChild(income, this, history, Parent);
            
            default:
                return null;
                // TODO once all types are implemented switch to (and change constructor access to protected):
                throw new NotImplementedException($"{nameof(Behaviours)}.{nameof(AddBehaviour)}: Could not find " +
                                                  $"{nameof(Behaviour)} of type '{behaviourBlueprint?.GetType()}'");
        }
    }
}
