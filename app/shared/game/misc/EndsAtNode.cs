using Godot;
using System;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Durations;

// TODO unit tests
public partial class EndsAtNode : Node2D, INodeFromBlueprint<EndsAt>
{
    public const string ScenePath = @"res://app/shared/game/misc/EndsAtNode.tscn";
    public static EndsAtNode Instance() => (EndsAtNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static EndsAtNode InstantiateAsChild(EndsAt blueprint, Node parentNode)
    {
        var endsAt = Instance();
        parentNode.AddChild(endsAt);
        endsAt.SetBlueprint(blueprint);
        return endsAt;
    }
    
    public event Action Completed = delegate { };

    public Guid InstanceId { get; set; } = Guid.NewGuid();

    private bool IsInstant { get; set; }
    private bool EndsOnDeath { get; set; }
    private bool OnActorAction { get; set; }
    private int Counter { get; set; }
    private int MaxCounter { get; set; }
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
            MaxCounter = 1;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Next.Planning))
        {
            MaxCounter = 1;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Next.ActionPhase))
        {
            MaxCounter = 1;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Second.Action))
        {
            MaxCounter = 2;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Second.Planning))
        {
            MaxCounter = 2;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Second.ActionPhase))
        {
            MaxCounter = 2;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Third.Action))
        {
            MaxCounter = 3;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Third.Planning))
        {
            MaxCounter = 3;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Third.ActionPhase))
        {
            MaxCounter = 3;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Fourth.Action))
        {
            MaxCounter = 4;
            TriggersOn = TriggersOnEnum.Start;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Fourth.Planning))
        {
            MaxCounter = 4;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.StartOf.Fourth.ActionPhase))
        {
            MaxCounter = 4;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Action;
        }
        
        // ...

        if (endsAt.Equals(EndsAt.StartOf.Tenth.Planning))
        {
            MaxCounter = 10;
            TriggersOn = TriggersOnEnum.Start;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.This.Action))
        {
            MaxCounter = 1;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.This.Planning))
        {
            MaxCounter = 1;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.This.ActionPhase))
        {
            MaxCounter = 1;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Next.Action))
        {
            MaxCounter = 2;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Next.Planning))
        {
            MaxCounter = 2;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Next.ActionPhase))
        {
            MaxCounter = 2;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Second.Action))
        {
            MaxCounter = 3;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Second.Planning))
        {
            MaxCounter = 3;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Second.ActionPhase))
        {
            MaxCounter = 3;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Third.Action))
        {
            MaxCounter = 4;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Third.Planning))
        {
            MaxCounter = 4;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Third.ActionPhase))
        {
            MaxCounter = 4;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Action;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Fourth.Action))
        {
            MaxCounter = 5;
            TriggersOn = TriggersOnEnum.End;
            OnActorAction = true;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Fourth.Planning))
        {
            MaxCounter = 5;
            TriggersOn = TriggersOnEnum.End;
            Phase = TurnPhase.Planning;
        }
        
        if (endsAt.Equals(EndsAt.EndOf.Fourth.ActionPhase))
        {
            MaxCounter = 5;
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
    /// Resets the duration to the full amount if this <see cref="EndsAt"/> <see cref="HasDuration"/>.
    /// </summary>
    public void ResetDuration()
    {
        if (HasDuration()) Counter = MaxCounter;
    }

    /// <summary>
    /// Returns true if <see cref="EndsAt"/> has completed.
    /// </summary>
    /// <returns></returns>
    public bool HasCompleted() => IsInstant || Counter <= 0;

    /// <summary>
    /// Returns true if <see cref="EndsAt"/> has a duration.
    /// </summary>
    /// <returns></returns>
    public bool HasDuration() => IsInstant is false;

    public string GetText(bool current = true, bool capitalizeFirstLetter = false)
    {
        var text = string.Empty;
        
        if (TriggersOn is TriggersOnEnum.Start) text += "start of ";
        else text += "end of ";

        if (current) text += Counter;
        else text += MaxCounter;

        if (OnActorAction) text += " actions";
        else
        {
            if (Phase.Equals(TurnPhase.Action)) text += " action phases";
            if (Phase.Equals(TurnPhase.Planning)) text += " planning phases";
        }
            
        if (EndsOnDeath) text = "until death";
        
        return capitalizeFirstLetter ? text[0].ToString().ToUpper() + text.Substring(1) : text;
    }

    private enum TriggersOnEnum
    {
        Start,
        End
    }
}
