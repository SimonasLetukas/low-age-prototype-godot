using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Common;
using low_age_data.Domain.Effects;

public class Effects
{
    private IList<EffectNode> Chain { get; set; } = new List<EffectNode>();

    public EntityNode SourceOrNull => Chain.LastOrDefault()?.Initiator;
    public EntityNode OriginOrNull => Chain.FirstOrDefault()?.Initiator;
    
    public Effects(EffectId effectId, EntityNode initiator)
    {
        var effect = CreateEffect(effectId, initiator);
        
        Chain.Add(effect);
    }
    
    public Effects(Effects previousEffects, EffectId effectId, EntityNode initiator)
    {
        Chain = previousEffects.Chain;
        var effect = CreateEffect(effectId, initiator);
        
        Chain.Add(effect);
    }

    public bool ValidateLast() => Chain.LastOrDefault()?.Validate() ?? false;

    public void ExecuteLast() => Chain.LastOrDefault()?.Execute();

    private EffectNode CreateEffect(EffectId effectId, EntityNode initiator)
    {
        var allEffects = Data.Instance.Blueprint.Effects;
        var foundEffect = allEffects.FirstOrDefault(x => x.Id.Equals(effectId));
        
        switch (foundEffect)
        {
            case ApplyBehaviour applyBehaviour:
                var effect = new ApplyBehaviourNode(applyBehaviour, initiator, this);
                var target = GetTarget(applyBehaviour.Target, initiator);
                effect.SetTarget(target);
                return effect;
            default:
                return new EffectNode(this);
                // TODO once all types are implemented switch to (and change constructor access to protected):
                throw new NotImplementedException($"{nameof(Effects)}.{nameof(CreateEffect)}: Could not find " +
                                                  $"{nameof(Effect)} of type '{foundEffect?.GetType()}'");
        }
    }
    
    private EntityNode GetTarget(Location location, EntityNode initiator)
    { 
        switch (location)
        {
            case var _ when location.Equals(Location.Inherited): // TODO
            case var _ when location.Equals(Location.Self):
                return initiator;
            case var _ when location.Equals(Location.Source):
                return SourceOrNull ?? initiator;
            case var _ when location.Equals(Location.Origin):
                return OriginOrNull ?? initiator;
            default:
                throw new NotImplementedException($"{nameof(Effects)}.{nameof(GetTarget)}: Could not find " +
                                                  $"{nameof(Location)} of value '{location}'");
        }
    }
}