using System;
using System.Collections.Generic;
using System.Linq;
using LowAge.app.shared.game.effects;
using LowAgeData.Domain.Effects;

/// <summary>
/// A new effect chain is always created from abilities. Behaviours and effects then continue the chain. Effects that
/// create other effects retain the same initiator. Behaviours that create effects set their attached entities as
/// initiators.
/// </summary>
public class Effects
{
    private IList<EffectNode> Chain { get; set; } = new List<EffectNode>();

    public Player? SourcePlayerOrNull => Chain.LastOrDefault()?.InitiatorPlayer;
    public Player? OriginPlayerOrNull => Chain.FirstOrDefault()?.InitiatorPlayer;
    public EntityNode? SourceEntityOrNull => Chain.LastOrDefault()?.InitiatorEntity;
    public EntityNode? OriginEntityOrNull => Chain.FirstOrDefault()?.InitiatorEntity;

    public Effects(EffectId effectId, Player initiator)
    {
        var effect = CreateEffect(effectId, initiator);
        
        Chain.Add(effect);
    }
    
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
    
    private EffectNode CreateEffect(EffectId effectId, Player initiator)
    {
        var foundEffect = FindEffectBlueprint(effectId);
        switch (foundEffect)
        {
            case Search search:
                var effect = new SearchNode(search, initiator, this);
                return effect;
            default:
                return new EffectNode(this, initiator);
                // TODO once all types are implemented switch to (and change constructor access to protected):
                throw new NotImplementedException($"{nameof(Effects)}.{nameof(CreateEffect)}: Could not find " +
                                                  $"{nameof(Effect)} of type '{foundEffect?.GetType()}'");
        }
    }

    private EffectNode CreateEffect(EffectId effectId, EntityNode initiator)
    {
        var foundEffect = FindEffectBlueprint(effectId);
        switch (foundEffect)
        {
            case ApplyBehaviour applyBehaviour:
                var effect = new ApplyBehaviourNode(applyBehaviour, initiator, this);
                return effect;
            default:
                return new EffectNode(this, initiator);
                // TODO once all types are implemented switch to (and change constructor access to protected):
                throw new NotImplementedException($"{nameof(Effects)}.{nameof(CreateEffect)}: Could not find " +
                                                  $"{nameof(Effect)} of type '{foundEffect?.GetType()}'");
        }
    }

    private static Effect? FindEffectBlueprint(EffectId effectId)
    {
        var allEffects = Data.Instance.Blueprint.Effects;
        var foundEffect = allEffects.FirstOrDefault(x => x.Id.Equals(effectId));
        return foundEffect;
    }
}