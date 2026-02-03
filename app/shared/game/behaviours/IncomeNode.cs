using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;

public partial class IncomeNode : BehaviourNode, INodeFromBlueprint<Income>
{
    private const string ScenePath = @"res://app/shared/game/behaviours/IncomeNode.tscn";
    private static IncomeNode Instance() => (IncomeNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static IncomeNode InstantiateAsChild(Income blueprint, Node parentNode, Effects history, 
        EntityNode parentEntity)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.History = history;
        behaviour.Parent = parentEntity;
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    private Income Blueprint { get; set; } = null!;

    public void SetBlueprint(Income blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        
        EventBus.Instance.RaiseIncomeProviderRegistered(GetProvider());
    }

    public override void _ExitTree()
    {
        EventBus.Instance.RaiseIncomeProviderUnregistered(GetProvider());
        
        base._ExitTree();
    }

    private IncomeProvider GetProvider() => new()
    {
        Id = InstanceId,
        EntityId = Parent.InstanceId,
        Player = Parent.Player,
        ProviderType = Blueprint.Id,
        ResourcesGained = Blueprint.Resources
            .Select(r => new Payment(r.Resource, (int)r.Amount))
            .ToList(),
        DiminishingReturn = Blueprint.DiminishingReturn,
        Cost = Blueprint.Cost,
        WaitForAvailableStorage = Blueprint.WaitForAvailableStorage
    };
}