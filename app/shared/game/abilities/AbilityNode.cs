using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;

public partial class AbilityNode : Node2D, INodeFromBlueprint<Ability>
{
    public event Action<AbilityNode> Activated = delegate { };
    public event Action<AbilityNode> CooldownEnded = delegate { };

    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public AbilityId Id { get; protected set; } = null!;
    public string DisplayName { get; protected set; } = null!;
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
        RemainingCooldown = EndsAtNode.InstantiateAsChild(Blueprint.Cooldown, this);
        RemainingCooldown.Completed += OnCooldownEnded;
        PaymentPaid = blueprint.Cost.Select(paymentRequired => new Payment(paymentRequired.Resource)).ToList();
        IsResearched = true; // TODO fetch from player
        IsActive = IsPaid() && IsResearched;
        HasButton = Blueprint.HasButton;
    }

    public virtual void Preview()
    {
    }
    
    public virtual bool TryActivate()
    {
        if (IsActive is false)
            return false;

        IsActive = TryStartCooldown() is false;
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
        if (Blueprint.Cooldown.Equals(EndsAt.Instant))
            return false;
        
        RemainingCooldown.SetBlueprint(Blueprint.Cooldown);
        return true;
    }
    
    protected virtual void OnCooldownEnded()
    {
        IsActive = IsResearched;
        CooldownEnded(this);
    }
}