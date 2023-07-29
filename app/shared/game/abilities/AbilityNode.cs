using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Durations;

public abstract class AbilityNode : Node2D, INodeFromBlueprint<Ability>
{
    public event Action<AbilityNode> Activated = delegate { };
    public event Action<AbilityNode> CooldownEnded = delegate { };

    public Guid Id { get; } = Guid.NewGuid();
    public abstract Ability Blueprint { get; protected set; }
    public abstract EndsAtNode RemainingCooldown { get; protected set; }
    public abstract List<Payment> PaymentPaid { get; protected set; }
    public abstract bool IsResearched { get; protected set; }
    public abstract bool IsActive { get; protected set; }
    
    public virtual void SetBlueprint(Ability blueprint)
    {
        Blueprint = blueprint;
        RemainingCooldown = EndsAtNode.InstantiateAsChild(EndsAt.Instant, this);
        RemainingCooldown.Completed += OnCooldownEnded;
        PaymentPaid = blueprint.Cost.Select(paymentRequired => new Payment(paymentRequired.Resource)).ToList();
        IsResearched = true; // TODO fetch from player
        IsActive = IsPaid() && IsResearched;
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