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

    public EffectNode? PreviousOrNull => Chain.Count > 1 ? Chain[^2] : null;
    public EntityNode? SourceEntityOrNull => Chain.LastOrDefault()?.InitiatorEntity;
    public EntityNode? OriginEntityOrNull => Chain.FirstOrDefault()?.InitiatorEntity;
    
    public Effects(EffectId effectId, ITargetable? initialTarget, EntityNode? initiator)
    {
        var effect = CreateEffect(effectId, initialTarget, initiator);
        
        Chain.Add(effect);
    }
    
    public Effects(Effects previousEffects, EffectId effectId, ITargetable? initialTarget, EntityNode? initiator)
    {
        Chain = previousEffects.Chain;
        var effect = CreateEffect(effectId, initialTarget, initiator);
        
        Chain.Add(effect);
    }

    public bool ValidateLast() => Chain.LastOrDefault()?.Validate() ?? false;

    public void ExecuteLast() => Chain.LastOrDefault()?.Execute();

    private EffectNode CreateEffect(EffectId effectId, ITargetable? initialTarget, EntityNode? initiator)
    {
        var foundEffect = FindEffectBlueprint(effectId);
        switch (foundEffect)
        {
            case ApplyBehaviour applyBehaviour:
                var applyBehaviourNode = new ApplyBehaviourNode(applyBehaviour, this, initialTarget, initiator);
                return applyBehaviourNode;
            case Search search:
                var searchNode = new SearchNode(search, this, initialTarget, initiator);
                return searchNode;
            default:
                return null;
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