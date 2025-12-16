using Godot;
using System;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;

// TODO unit tests
public partial class EndsAtNode : Node2D, INodeFromBlueprint<EndsAt>
{
    public const string ScenePath = @"res://app/shared/game/misc/EndsAtNode.tscn";
    public static EndsAtNode Instance() => (EndsAtNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static EndsAtNode InstantiateAsChild(EndsAt blueprint, Node parentNode, EntityNode ownerEntity)
    {
        var endsAt = Instance();
        parentNode.AddChild(endsAt);
        endsAt.SetBlueprint(blueprint);
        endsAt.OwnerEntity = ownerEntity;
        return endsAt;
    }
    
    public event Action Completed = delegate { };

    public Guid InstanceId { get; set; } = Guid.NewGuid();

    private bool IsInstant { get; set; }
    private bool EndsOnDeath { get; set; }
    private int Counter { get; set; }
    private int MaxCounter { get; set; }
    private TriggersOnEnum TriggersOn { get; set; }
    private TurnPhase? Phase { get; set; } // when null, the timing is implicitly understood as "action" 
    private EntityNode OwnerEntity { get; set; } = null!;

    public override void _Ready()
    {
        base._Ready();

        EventBus.Instance.PhaseStarted += OnPhaseStarted;
        EventBus.Instance.PhaseEnded += OnPhaseEnded;
        EventBus.Instance.ActionStarted += OnActionStarted;
        EventBus.Instance.ActionEnded += OnActionEnded;
        EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        EventBus.Instance.PhaseEnded -= OnPhaseEnded;
        EventBus.Instance.ActionStarted -= OnActionStarted;
        EventBus.Instance.ActionEnded -= OnActionEnded;
        EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
        
        base._ExitTree();
    }

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
    /// Resets the duration to the full amount if this <see cref="EndsAt"/> <see cref="HasDuration"/>. Otherwise,
    /// immediately triggers <see cref="Completed"/>
    /// </summary>
    public void ResetDuration()
    {
        var hasDuration = HasDuration();
        
        if (hasDuration && EndsOnDeath is false) 
            Counter = MaxCounter;

        if (hasDuration is false)
            Completed();
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

        if (Phase is null) text += " actions";
        else
        {
            if (Phase.Equals(TurnPhase.Action)) text += " action phases";
            if (Phase.Equals(TurnPhase.Planning)) text += " planning phases";
        }
            
        if (EndsOnDeath) text = "until death";
        
        return capitalizeFirstLetter 
            ? string.Concat(text[0].ToString().ToUpper(), text.AsSpan(1)) 
            : text;
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);

    private void OnPhaseStarted(int turn, TurnPhase turnPhase)
    {
        if (EndsOnDeath 
            || IsInstant
            || TriggersOn is TriggersOnEnum.End
            || Phase is null
            || turnPhase.Equals(Phase) is false
            || Counter <= 0) 
            return;

        Counter--;

        if (Counter > 0)
            return;

        Completed();
    }
    
    private void OnPhaseEnded(int turn, TurnPhase turnPhase)
    {
        if (EndsOnDeath 
            || IsInstant
            || TriggersOn is TriggersOnEnum.Start
            || Phase is null
            || turnPhase.Equals(Phase) is false
            || Counter <= 0) 
            return;

        Counter--;

        if (Counter > 0)
            return;

        Completed();
    }
    
    private void OnActionStarted(ActorNode actor)
    {
        if (EndsOnDeath
            || IsInstant
            || TriggersOn is TriggersOnEnum.End
            || Phase is not null
            || actor.Equals(OwnerEntity) is false
            || Counter <= 0)
            return;

        Counter--;
        
        if (Counter > 0)
            return;

        Completed();
    }
    
    private void OnActionEnded(ActorNode actor)
    {
        if (EndsOnDeath
            || IsInstant
            || TriggersOn is TriggersOnEnum.Start
            || Phase is not null
            || actor.Equals(OwnerEntity) is false
            || Counter <= 0)
            return;

        Counter--;

        if (Counter > 0)
            return;

        Completed();
    }

    private void OnEntityDestroyed(EntityNode entity)
    {
        if (EndsOnDeath is false
            || IsInstant
            || entity.Equals(OwnerEntity) is false)
            return;
        
        Completed();
    }
    
    private enum TriggersOnEnum
    {
        Start,
        End
    }
}
