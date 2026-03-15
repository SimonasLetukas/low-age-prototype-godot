using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;
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
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;

        HandleInitialModifications();
    }

    protected override void EndBehaviour()
    {
        if (Log.DebugEnabled)
            Log.Info(nameof(BuffNode), nameof(EndBehaviour), ToString());
        
        if (Blueprint.RestoreChangesOnEnd)
            RestoreInitialModifications();

        HandleFinalModifications();
        
        base.EndBehaviour();
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