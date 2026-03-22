using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;

public class ActionEconomy
{
    public event Action Updated = delegate { };
    
    public int MeleeAttackActions => GetMeleeAttackActions();
    public int RangedAttackActions => GetRangedAttackActions();
    public int AbilityActions => GetAbilityActions();
    public bool CanMeleeAttack => MeleeAttackActions > 0;
    public bool CanRangedAttack => RangedAttackActions > 0;
    public bool CanUseAbilityAction => AbilityActions > 0;
    public bool CanMove => GetCanMove();
    public int NumberOfPossibleActions => MeleeAttackActions 
                                          + RangedAttackActions 
                                          + AbilityActions 
                                          + (CanMove ? 1 : 0);

    public Configuration Config { get; set; } = new();
    public class Configuration
    {
        public int MaxMeleeAttackActions { get; set; } = 1;
        public int MaxRangedAttackActions { get; set; } = 1;
        public int MaxAbilitiesInActionPhase { get; set; } = 1;
        public int MaxAbilitiesInPlanningPhase { get; set; } = 1;
        public bool MovementAllowed { get; set; } = true;
        public bool CanMoveAfterAnyAction { get; set; } = false;
        public bool CanMoveAfterMeleeAttackAction { get; set; } = false;
        public bool CanMoveAfterRangedAttackAction { get; set; } = false;
        public bool CanMoveAfterAbilityAction { get; set; } = false;
        public int MinimumAllowedMovement { get; set; } = 1;
        public bool MeleeAttackActionAllowedAfterMinimumMovement { get; set; } = true;
        public bool MeleeAttackActionAllowedAfterFullMovement { get; set; } = true;
        public bool RangedAttackActionAllowedAfterMinimumMovement { get; set; } = false;
        public bool RangedAttackActionAllowedAfterFullMovement { get; set; } = false;
        public bool AbilityActionAllowedAfterMinimumMovement { get; set; } = false;
        public bool AbilityActionAllowedAfterFullMovement { get; set; } = false;
    }

    private readonly IList<IAction> _actionsDone = [];
    
    private float _movementSpent = 0f;
    private bool _isPlanning = false;

    public static ActionEconomy For(ActorNode actor)
    {
        var actionEconomy = new ActionEconomy
        {
            Config =
            {
                MaxMeleeAttackActions = actor.HasMeleeAttack ? 1 : 0,
                MaxRangedAttackActions = actor.HasRangedAttack ? 1 : 0,
                MaxAbilitiesInActionPhase = actor.Abilities.GetForActionPhase().Any() ? 1 : 0,
                MaxAbilitiesInPlanningPhase = actor.Abilities.GetForPlanningPhase().Any() ? 1 : 0,
                MovementAllowed = actor is UnitNode
            }
        };
        
        return actionEconomy;
    }

    public void Restore(TurnPhase phase, bool restoringOnlyAbilityAction = false)
    {
        _isPlanning = phase == TurnPhase.Planning;

        if (restoringOnlyAbilityAction)
        {
            var abilityAction = _actionsDone.FirstOrDefault(a => a is AbilityAction);
            if (abilityAction is not null)
                _actionsDone.Remove(abilityAction);

            Updated();
            return;
        }

        _movementSpent = 0f;
        _actionsDone.Clear();
        Updated();
    }

    public void Moved(float value, bool anyMovementRemaining)
    {
        _movementSpent += value;
        if (anyMovementRemaining is false)
            _actionsDone.Add(new MovementDepletedAction());
        
        Updated();
    }

    public void Attacked(AttackType attackType)
    {
        if (attackType.Equals(AttackType.Melee))
            _actionsDone.Add(new MeleeAttackAction());

        if (attackType.Equals(AttackType.Ranged))
            _actionsDone.Add(new RangedAttackAction());
        
        Updated();
    }

    public void UsedAbilityAction()
    {
        _actionsDone.Add(new AbilityAction());   
        
        Updated();
    }
    
    private int GetMeleeAttackActions()
    {
        var maxActions = _isPlanning 
            ? 0
            : Config.MaxMeleeAttackActions;

        if (maxActions is 0)
            return 0;

        var otherActionMade = _actionsDone.Any(a => a is RangedAttackAction or AbilityAction);
        if (otherActionMade)
            return 0;

        var movementDepleted = _actionsDone.Any(a => a is MovementDepletedAction);
        if (IsMinimumMovementExceeded() && movementDepleted 
                                        && Config.MeleeAttackActionAllowedAfterFullMovement is false)
            return 0;
        
        if (IsMinimumMovementExceeded() && Config.MeleeAttackActionAllowedAfterMinimumMovement is false)
            return 0;
        
        return maxActions - _actionsDone.Count(a => a is MeleeAttackAction);
    }

    private int GetRangedAttackActions()
    {
        var maxActions = _isPlanning 
            ? 0
            : Config.MaxRangedAttackActions;

        if (maxActions is 0)
            return 0;

        var otherActionMade = _actionsDone.Any(a => a is MeleeAttackAction or AbilityAction);
        if (otherActionMade)
            return 0;

        var movementDepleted = _actionsDone.Any(a => a is MovementDepletedAction);
        if (IsMinimumMovementExceeded() && movementDepleted 
                                        && Config.RangedAttackActionAllowedAfterFullMovement is false)
            return 0;
        
        if (IsMinimumMovementExceeded() && Config.RangedAttackActionAllowedAfterMinimumMovement is false)
            return 0;
        
        return maxActions - _actionsDone.Count(a => a is RangedAttackAction);
    }

    private int GetAbilityActions()
    {
        var maxActions = _isPlanning 
            ? Config.MaxAbilitiesInPlanningPhase 
            : Config.MaxAbilitiesInActionPhase;
        
        if (maxActions is 0)
            return 0;

        var otherActionMade = _actionsDone.Any(a => a is MeleeAttackAction or RangedAttackAction);
        if (otherActionMade)
            return 0;

        var movementDepleted = _actionsDone.Any(a => a is MovementDepletedAction);

        if (IsMinimumMovementExceeded() && movementDepleted 
                                        && Config.AbilityActionAllowedAfterFullMovement is false)
            return 0;
        
        if (IsMinimumMovementExceeded() && Config.AbilityActionAllowedAfterMinimumMovement is false)
            return 0;
        
        return maxActions - _actionsDone.Count(a => a is AbilityAction);
    }

    private bool GetCanMove()
    {
        if (Config.MovementAllowed is false)
            return false;

        if (_isPlanning)
            return false;

        var movementDepleted = _actionsDone.Any(a => a is MovementDepletedAction);
        if (movementDepleted)
            return false;

        if (Config.CanMoveAfterAnyAction)
            return true;

        var meleeAttacked = _actionsDone.Any(a => a is MeleeAttackAction);
        if (meleeAttacked && Config.CanMoveAfterMeleeAttackAction is false)
            return false;

        var rangedAttacked = _actionsDone.Any(a => a is RangedAttackAction);
        if (rangedAttacked && Config.CanMoveAfterRangedAttackAction is false)
            return false;
        
        var usedAbility = _actionsDone.Any(a => a is AbilityAction);
        if (usedAbility && Config.CanMoveAfterAbilityAction is false)
            return false;

        return true;
    }

    private bool IsMinimumMovementExceeded()
        => _movementSpent > Config.MinimumAllowedMovement + Constants.Pathfinding.SearchIncrement;
    
    private interface IAction;
    private class MeleeAttackAction : IAction;
    private class RangedAttackAction : IAction;
    private class AbilityAction : IAction;
    private class MovementDepletedAction : IAction;
}