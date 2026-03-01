using Godot;
using LowAgeData.Domain.Behaviours;

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

    protected override void Destroy()
    {
        if (DebugEnabled)
            GD.Print($"Buff on {Parent.DisplayName} at {Parent.EntityPrimaryPosition}: destroy called.");
        
        if (Blueprint.RestoreChangesOnEnd)
            RestoreInitialModifications();
        
        base.Destroy();
    }

    private void HandleInitialModifications()
    {
        var applier = new ModificationApplier(Parent, Blueprint.RestoreChangesOnEnd);

        foreach (var modification in Blueprint.InitialModifications)
            modification.Accept(applier);
    }

    private void RestoreInitialModifications()
    {
        if (DebugEnabled)
            GD.Print($"Buff on {Parent.DisplayName} at {Parent.EntityPrimaryPosition}: restoring modifications.");
        
        var restorer = new ModificationRestorer(Parent);
        
        foreach (var modification in Blueprint.InitialModifications)
            modification.Accept(restorer);
    }
}