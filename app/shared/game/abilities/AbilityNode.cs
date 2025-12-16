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
    
    public override void _ExitTree()
    {
        RemainingCooldown.Completed -= OnCooldownEnded;
        base._ExitTree();
    }

    // TODO might not be needed -- need to think of how the "instant" abilities will work first
    public virtual void Preview()
    {
    }

    public virtual ValidationResult CanActivate(TurnPhase currentTurnPhase, ActorNode? actorInAction)
    {
        if (IsActive is false)
            return ValidationResult.Invalid("Cannot activate an ability which is already activated.");
        
        if (RemainingCooldown.HasCompleted() is false)
            return ValidationResult.Invalid("This ability is still on cooldown.");
        
        if (currentTurnPhase.Equals(TurnPhase) is false)
            return ValidationResult.Invalid($"This ability can only be activated in the " +
                                            $"{TurnPhase.ToDisplayValue()} Phase.");
        
        if ((TurnPhase.Equals(TurnPhase.Action) && OwnerActor.Equals(actorInAction) is false))
            return ValidationResult.Invalid("It's not your turn!");

        return ValidationResult.Valid;
    }
    
    public virtual void Activate()
    {
        OwnerActor.ActionEconomy.UsedAbilityAction();
        IsActive = false;
        StartCooldown();
        // TODO start paying and execute ability only after it's paid
        RaiseActivated();
    }

    public bool IsPaid()
    {
        return true; // TODO implement later
        
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

    protected void RaiseActivated() => Activated(this);

    protected virtual void StartCooldown()
    {
        RemainingCooldown.ResetDuration();
    }
    
    protected virtual void OnCooldownEnded()
    {
        IsActive = IsPaid() && IsResearched;
        CooldownEnded(this);
    }
    
    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}