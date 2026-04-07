using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Effects;

/// <summary>
/// A new effect chain is always created from abilities. Behaviours and effects then continue the chain. Effects that
/// create other effects retain the same initiator. Behaviours that create effects set their attached entities as
/// initiators, and entity owners as initiator players.
/// </summary>
public class Effects
{
    private IList<EffectNode> Chain { get; set; } = new List<EffectNode>();

    public EffectNode First => Chain.First();
    public EffectNode Last => Chain.Last();
    public EffectNode? PreviousOrNull => Chain.Count > 1 ? Chain[^2] : null;
    public EntityNode? SourceEntityOrNull => PreviousOrNull?.InitiatorEntity;
    public EntityNode? OriginEntityOrNull => First.InitiatorEntity;
    
    public Effects(EffectId effectId, IList<ITargetable> initialTargets, Player initiatorPlayer, 
        EntityNode? initiatorEntity)
    {
        var effect = CreateEffect(effectId, initialTargets, initiatorPlayer, initiatorEntity);
        
        Chain.Add(effect);
        effect.UpdateFoundTargets();
    }
    
    public Effects(Effects previousEffects, EffectId effectId, IList<ITargetable> initialTargets, 
        Player initiatorPlayer, EntityNode? initiatorEntity)
    {
        var effect = CreateEffect(effectId, initialTargets, initiatorPlayer, initiatorEntity);
        
        Chain = previousEffects.Chain.ToList();
        Chain.Add(effect);
        effect.UpdateFoundTargets();
    }

    public ValidationResult ValidateLast() => Chain.LastOrDefault()?.Validate() 
                                              ?? ValidationResult.Invalid("Validation failed.");

    public void ExecuteLast() => Chain.LastOrDefault()?.Execute();

    private EffectNode CreateEffect(EffectId effectId, IList<ITargetable> initialTargets, Player initiatorPlayer, 
        EntityNode? initiatorEntity)
    {
        var foundEffect = FindEffectBlueprint(effectId);
        switch (foundEffect)
        {
            case ApplyBehaviour applyBehaviour:
                var applyBehaviourNode = new ApplyBehaviourNode(applyBehaviour, this, initialTargets, 
                    initiatorPlayer, initiatorEntity);
                return applyBehaviourNode;
            case Damage damage:
                var damageNode = new DamageNode(damage, this, initialTargets, initiatorPlayer, 
                    initiatorEntity);
                return damageNode;
            case Destroy destroy:
                var destroyNode = new DestroyNode(destroy, this, initialTargets, initiatorPlayer, 
                    initiatorEntity);
                return destroyNode;
            case Heal heal:
                var healNode = new HealNode(heal, this, initialTargets, initiatorPlayer, 
                    initiatorEntity);
                return healNode;
            case ModifyPlayer modifyPlayer:
                var modifyPlayerNode = new ModifyPlayerNode(modifyPlayer, this, initialTargets, initiatorPlayer, 
                    initiatorEntity);
                return modifyPlayerNode;
            case Search search:
                var searchNode = new SearchNode(search, this, initialTargets, initiatorPlayer, initiatorEntity);
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

    public override string ToString() => string.Join(", ", Chain.Select(e => e.Id));
}