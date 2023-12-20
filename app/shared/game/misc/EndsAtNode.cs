using Godot;
using System;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

// TODO unit tests
public class EndsAtNode : Node2D, INodeFromBlueprint<EndsAt>
{
    public const string ScenePath = @"res://app/shared/game/misc/EndsAtNode.tscn";
    public static EndsAtNode Instance() => (EndsAtNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static EndsAtNode InstantiateAsChild(EndsAt blueprint, Node parentNode)
    {
        var endsAt = Instance();
        parentNode.AddChild(endsAt);
        endsAt.SetBlueprint(blueprint);
        return endsAt;
    }
    
    public event Action Completed = delegate { };

    public Guid Id { get; set; } = Guid.NewGuid();

    private bool IsInstant { get; set; }
    private bool EndsOnDeath { get; set; }
    private bool OnActorAction { get; set; }
    private int Counter { get; set; }
    private TriggersOnEnum TriggersOn { get; set; }
    private TurnPhase Phase { get; set; }
    
    public void SetBlueprint(EndsAt endsAt)
    {
        if (endsAt.Equals(EndsAt.Death))
            EndsOnDeath = true;

        if (endsAt.Equals(EndsAt.Instant))
            IsInstant = true;

        if (endsAt.Equals(EndsAt.StartOf.Next.Action))
        {
            Counter = 1;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Next.Planning))
        {
            Counter = 1;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Next.ActionPhase))
        {
            Counter = 1;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Second.Action))
        {
            Counter = 2;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Second.Planning))
        {
            Counter = 2;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Second.ActionPhase))
        {
            Counter = 2;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Third.Action))
        {
            Counter = 3;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Third.Planning))
        {
            Counter = 3;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Third.ActionPhase))
        {
            Counter = 3;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Fourth.Action))
        {
            Counter = 4;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Fourth.Planning))
        {
            Counter = 4;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Fourth.ActionPhase))
        {
            Counter = 4;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        // ...

        if (endsAt.Equals(EndsAt.StartOf.Tenth.Planning))
        {
            Counter = 10;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.This.Action))
        {
            Counter = 1;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.This.Planning))
        {
            Counter = 1;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.This.ActionPhase))
        {
            Counter = 1;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Next.Action))
        {
            Counter = 2;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Next.Planning))
        {
            Counter = 2;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Next.ActionPhase))
        {
            Counter = 2;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Second.Action))
        {
            Counter = 3;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Second.Planning))
        {
            Counter = 3;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Second.ActionPhase))
        {
            Counter = 3;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Third.Action))
        {
            Counter = 4;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Third.Planning))
        {
            Counter = 4;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Third.ActionPhase))
        {
            Counter = 4;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Fourth.Action))
        {
            Counter = 5;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Fourth.Planning))
        {
            Counter = 5;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Fourth.ActionPhase))
        {
            Counter = 5;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
    }
    
    /// <summary>
    /// Advances counter when <see cref="TurnPhase"/> started. Returns true and sends a <see cref="Completed"/> event
    /// if <see cref="EndsAt"/> duration has completed.
    /// </summary>
    /// <param name="turnPhase"><see cref="TurnPhase"/> that has started</param>
    public bool OnStarted(TurnPhase turnPhase)
    {
        if (EndsOnDeath 
            || IsInstant
            || TriggersOn is TriggersOnEnum.End
            || OnActorAction
            || turnPhase.Equals(Phase) is false) 
            return false;

        Counter--;

        if (Counter > 0)
            return false;

        Completed();
        return true;
    }
        
    /// <summary>
    /// Advances counter when <see cref="TurnPhase"/> ended. Returns true and sends a <see cref="Completed"/> event
    /// if <see cref="EndsAt"/> duration has completed.
    /// </summary>
    /// <param name="turnPhase"><see cref="TurnPhase"/> that has ended</param>
    public bool OnEnded(TurnPhase turnPhase)
    {
        if (EndsOnDeath 
            || IsInstant
            || TriggersOn is TriggersOnEnum.Start
            || OnActorAction
            || turnPhase.Equals(Phase) is false) 
            return false;

        Counter--;

        if (Counter > 0)
            return false;

        Completed();
        return true;
    }

    /// <summary>
    /// Advances counter when action started. Returns true and sends a <see cref="Completed"/> event if
    /// <see cref="EndsAt"/> duration has completed.
    /// </summary>
    public bool OnActionStarted()
    {
        if (EndsOnDeath
            || IsInstant
            || TriggersOn is TriggersOnEnum.End
            || OnActorAction is false)
            return false;

        Counter--;
        
        if (Counter > 0)
            return false;

        Completed();
        return true;
    }
    
    /// <summary>
    /// Advances counter when action started. Returns true and sends a <see cref="Completed"/> event if
    /// <see cref="EndsAt"/> duration has completed.
    /// </summary>
    public bool OnActionEnded()
    {
        if (EndsOnDeath
            || IsInstant
            || TriggersOn is TriggersOnEnum.Start
            || OnActorAction is false)
            return false;

        Counter--;

        if (Counter > 0)
            return false;

        Completed();
        return true;
    }

    /// <summary>
    /// Returns true if <see cref="EndsAt"/> has completed.
    /// </summary>
    /// <returns></returns>
    public bool HasCompleted() => IsInstant || Counter <= 0;

    private enum TriggersOnEnum
    {
        Start,
        End
    }
}
