using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;

public partial class AbilityNode : Node2D, INodeFromBlueprint<Ability>
{
    public event Action<AbilityNode> Activated = delegate { };
    public event Action<AbilityNode> CooldownEnded = delegate { };

    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public AbilityId Id { get; protected set; } = null!;
    public string DisplayName { get; protected set; } = null!;
    public string Description { get; protected set; } = null!;
    public TurnPhase TurnPhase { get; protected set; } = null!;
    public IList<ResearchId> ResearchNeeded { get; protected set; } = [];
    public ActorNode OwnerActor { get; protected set; } = null!;
    public EndsAtNode RemainingCooldown { get; protected set; } = null!;
    public List<Payment> PaymentPaid { get; protected set; } = null!;
    public bool IsResearched { get; protected set; }
    public bool IsActive { get; protected set; }
    public bool HasButton { get; protected set; }
    
    private Ability Blueprint { get; set; } = null!;

    public void SetBlueprint(Ability blueprint)
    {
        Blueprint = blueprint;
        Id = Blueprint.Id;
        DisplayName = Blueprint.DisplayName;
        Description = Blueprint.Description;
        TurnPhase = Blueprint.TurnPhase;
        ResearchNeeded = Blueprint.ResearchNeeded; // TODO right now never updated, to be fetched from player and updated upon new research
        RemainingCooldown = EndsAtNode.InstantiateAsChild(Blueprint.Cooldown, this, OwnerActor);
        RemainingCooldown.Completed += OnCooldownEnded;
        PaymentPaid = blueprint.Cost.Select(paymentRequired => new Payment(paymentRequired.Resource)).ToList();
        IsResearched = (Config.Instance.ResearchEnabled && ResearchNeeded.Any()) is false;
        IsActive = IsPaid() && IsResearched;
        HasButton = Blueprint.HasButton;
    }

    // TODO might not be needed -- need to think of how the "instant" abilities will work first
    public virtual void Preview()
    {
    }
    
    public virtual bool TryActivate(TurnPhase currentTurnPhase, ActorNode? actorInAction)
    {
        if (IsActive is false 
            || currentTurnPhase.Equals(TurnPhase) is false 
            || OwnerActor.Equals(actorInAction) is false)
            return false;

        IsActive = TryStartCooldown() is false;
        // TODO start paying and execute ability only after it's paid
        Activated(this);
        return true;
    }

    public override void _ExitTree()
    {
        RemainingCooldown.Completed -= OnCooldownEnded;
        base._ExitTree();
    }

    public bool IsPaid()
    {
        foreach (var paymentRequired in Blueprint.Cost)
        {
            var paid = PaymentPaid.Single(x => x.Resource.Equals(paymentRequired.Resource));
            if (paid.Amount < paymentRequired.Amount)
            {
                IsActive = false;
                return false;
            }
        }

        IsActive = IsResearched;
        return true;
    }

    protected virtual bool TryStartCooldown()
    {
        if (RemainingCooldown.HasDuration() is false)
            return false;
        
        RemainingCooldown.ResetDuration();
        return true;
    }
    
    protected virtual void OnCooldownEnded()
    {
        IsActive = IsPaid() && IsResearched;
        CooldownEnded(this);
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}