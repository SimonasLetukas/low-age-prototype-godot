using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon.Extensions;
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
    
    private IncomeProvider _provider = null!;
    private IList<Payment> _currentPayment = null!;

    public void SetBlueprint(Income blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;

        _provider = GetProvider();
        
        EventBus.Instance.RaiseIncomeProviderRegistered(_provider);

        EventBus.Instance.IncomeProviderPaymentUpdated += OnIncomeProviderPaymentUpdated;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.RaiseIncomeProviderUnregistered(_provider);
        
        EventBus.Instance.IncomeProviderPaymentUpdated -= OnIncomeProviderPaymentUpdated;
        
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

    private void UpdateDescription()
    {
        Description = _currentPayment.IsEmpty()
            ? Blueprint.Description
            : Blueprint.Description + GetCurrentPaymentDescription();
    }

    private string GetCurrentPaymentDescription()
    {
        var result = "\n\nCurrently paid: ";
        for (var i = 0; i < _currentPayment.Count; i++)
        {
            var amount = _currentPayment[i].Amount;
            var resource = Data.Instance.Blueprint.Resources.FirstOrDefault(r => 
                r.Id.Equals(_currentPayment[i].Resource));
            if (resource is null)
                continue;

            result += $"{amount} {resource.DisplayName}";
            if (i < _currentPayment.Count - 1)
                result += ", ";
        }

        return result + ".";
    }
    
    private void OnIncomeProviderPaymentUpdated(IncomeProvider incomeProvider, IList<Payment> updatedPayment)
    {
        if (incomeProvider.Equals(_provider) is false)
            return;

        _currentPayment = updatedPayment;
        UpdateDescription();
    }
}