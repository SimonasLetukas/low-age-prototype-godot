using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Effects;
using Newtonsoft.Json;

public partial class BuffNode : BehaviourNode, INodeFromBlueprint<Buff>
{
    private const string ScenePath = @"res://app/shared/game/behaviours/BuffNode.tscn";
    private static BuffNode Instance() => (BuffNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static BuffNode InstantiateAsChild(Buff blueprint, Node parentNode, Effects history, 
        EntityNode parentEntity)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.History = history;
        behaviour.Parent = parentEntity;
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    private Buff Blueprint { get; set; } = null!;

    public void SetBlueprint(Buff blueprint)
    {
        Blueprint = blueprint;
        base.SetBlueprint(blueprint);

        AddModificationFlags();
        
        HandleInitialModifications();
        HandleEffects(Blueprint.InitialEffects);
    }

    protected override void EndBehaviour(bool triggersOnDeathBehaviours)
    {
        if (IsBeingDestroyed)
            return;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(BuffNode), nameof(EndBehaviour), ToString());
        
        RemoveModificationFlags();
        
        if (Blueprint.RestoreChangesOnEnd)
            RestoreInitialModifications();

        if (triggersOnDeathBehaviours)
        {
            HandleFinalModifications();
            HandleEffects(Blueprint.FinalEffects);
        }
        
        base.EndBehaviour(triggersOnDeathBehaviours);
    }

    private void AddModificationFlags()
    {
        foreach (var flag in Blueprint.ModificationFlags)
        {
            Parent.AddModificationFlag(flag);
        }
    }

    private void RemoveModificationFlags()
    {
        foreach (var flag in Blueprint.ModificationFlags)
        {
            Parent.RemoveModificationFlag(flag);
        }
    }

    private void HandleInitialModifications()
    {
        var applier = new ModificationApplier(Parent, Blueprint.RestoreChangesOnEnd);

        foreach (var modification in Blueprint.InitialModifications)
            modification.Accept(applier);
    }

    private void RestoreInitialModifications()
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BuffNode), nameof(RestoreInitialModifications), 
                $"{this} restoring modifications '{JsonConvert.SerializeObject(Blueprint.InitialModifications)}'.");
        
        var restorer = new ModificationRestorer(Parent);
        
        foreach (var modification in Blueprint.InitialModifications)
            modification.Accept(restorer);
    }
    
    private void HandleFinalModifications()
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BuffNode), nameof(HandleFinalModifications), 
                $"{this} adding final modifications '{JsonConvert.SerializeObject(Blueprint.FinalModifications)}'.");
        
        var applier = new ModificationApplier(Parent, false);
        
        foreach (var modification in Blueprint.FinalModifications)
            modification.Accept(applier);
    }
}