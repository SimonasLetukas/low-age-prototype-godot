using System;
using System.Linq;
using LowAgeData.Domain.Common;

public class ActionEconomy
{
    public event Action Updated = delegate { };
    
    public int MeleeAttackActions { get; private set; }
    public int RangedAttackActions { get; private set; }
    public int AbilityActions { get; private set; }
    public bool CanMeleeAttack => MeleeAttackActions > 0;
    public bool CanRangedAttack => RangedAttackActions > 0;
    public bool CanUseAbilityAction => AbilityActions > 0;
    public bool CanMove { get; private set; }
    public int NumberOfPossibleActions => MeleeAttackActions + RangedAttackActions + AbilityActions + (CanMove ? 1 : 0);

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

    public void Restore(TurnPhase phase)
    {
        _isPlanning = phase == TurnPhase.Planning;

        if (_isPlanning)
        {
            MeleeAttackActions = 0;
            RangedAttackActions = 0;
            AbilityActions = Config.MaxAbilitiesInPlanningPhase;
            CanMove = false;
        }
        else
        {
            MeleeAttackActions = Config.MaxMeleeAttackActions;
            RangedAttackActions = Config.MaxRangedAttackActions;
            AbilityActions = Config.MaxAbilitiesInActionPhase;
            CanMove = Config.MovementAllowed;
            _movementSpent = 0f;
        }
        
        Updated();
    }

    public void Moved(float value, bool anyMovementRemaining)
    {
        _movementSpent += value;
        if (_movementSpent > Config.MinimumAllowedMovement + Constants.Pathfinding.SearchIncrement)
            HandleActionsAfterMovement(anyMovementRemaining);
        
        if (anyMovementRemaining is false)
            CanMove = false;
        
        Updated();
    }

    public void Attacked(AttackType attackType)
    {
        if (attackType.Equals(AttackType.Melee))
            HandleMeleeAttack();

        if (attackType.Equals(AttackType.Ranged))
            HandleRangedAttack();
        
        Updated();
    }

    public void UsedAbilityAction()
    {
        HandleAbility();
        
        Updated();
    }

    private void HandleActionsAfterMovement(bool anyMovementRemaining)
    {
        if (Config.MeleeAttackActionAllowedAfterMinimumMovement is false 
            || (anyMovementRemaining is false && Config.MeleeAttackActionAllowedAfterFullMovement is false))
            MeleeAttackActions = 0;
        
        if (Config.RangedAttackActionAllowedAfterMinimumMovement is false 
            || (anyMovementRemaining is false && Config.RangedAttackActionAllowedAfterFullMovement is false))
            RangedAttackActions = 0;
        
        if (Config.AbilityActionAllowedAfterMinimumMovement is false 
            || (anyMovementRemaining is false && Config.AbilityActionAllowedAfterFullMovement is false))
            AbilityActions = 0;
    }

    private void HandleMeleeAttack()
    {
        MeleeAttackActions--;
        RangedAttackActions = 0;
        AbilityActions = 0;

        if (Config.CanMoveAfterAnyAction)
            return;

        if (Config.CanMoveAfterMeleeAttackAction is false)
            CanMove = false;
    }

    private void HandleRangedAttack()
    {
        MeleeAttackActions = 0;
        RangedAttackActions--;
        AbilityActions = 0;

        if (Config.CanMoveAfterAnyAction)
            return;

        if (Config.CanMoveAfterRangedAttackAction is false)
            CanMove = false;
    }

    private void HandleAbility()
    {
        MeleeAttackActions = 0;
        RangedAttackActions = 0;
        AbilityActions--;

        if (Config.CanMoveAfterAnyAction)
            return;

        if (Config.CanMoveAfterAbilityAction is false)
            CanMove = false;
    }
}