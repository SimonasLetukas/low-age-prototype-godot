using LowAgeData.Domain.Common;

public class ActionEconomy
{
    public int MeleeAttackActions { get; private set; }
    public int RangedAttackActions { get; private set; }
    public int AbilityActions { get; private set; }
    public bool CanMeleeAttack => MeleeAttackActions > 0;
    public bool CanRangedAttack => RangedAttackActions > 0;
    public bool CanUseAbilityAction => AbilityActions > 0;
    public bool CanMove { get; private set; }
    public int NumberOfPossibleActions => MeleeAttackActions + RangedAttackActions + AbilityActions + (CanMove ? 1 : 0);

    // Configuration properties
    public int MaxMeleeAttackActions { get; set; } = 1;
    public int MaxRangedAttackActions { get; set; } = 1;
    public int MaxAbilityActions { get; set; } = 1;
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

    private float _movementSpent = 0f;

    public void Restore()
    {
        MeleeAttackActions = MaxMeleeAttackActions;
        RangedAttackActions = MaxRangedAttackActions;
        AbilityActions = MaxAbilityActions;
        CanMove = true;
        _movementSpent = 0f;
    }

    public void Moved(float value, bool anyMovementRemaining)
    {
        _movementSpent += value;
        if (_movementSpent > MinimumAllowedMovement)
            HandleActionsAfterMovement(anyMovementRemaining);
        
        if (anyMovementRemaining is false)
            CanMove = false;
    }

    public void Attacked(AttackType attackType)
    {
        if (attackType.Equals(AttackType.Melee))
            HandleMeleeAttack();

        if (attackType.Equals(AttackType.Ranged))
            HandleRangedAttack();
    }

    public void UsedAbilityAction() => HandleAbility();

    private void HandleActionsAfterMovement(bool anyMovementRemaining)
    {
        if (MeleeAttackActionAllowedAfterMinimumMovement is false 
            || (anyMovementRemaining is false && MeleeAttackActionAllowedAfterFullMovement is false))
            MeleeAttackActions = 0;
        
        if (RangedAttackActionAllowedAfterMinimumMovement is false 
            || (anyMovementRemaining is false && RangedAttackActionAllowedAfterFullMovement is false))
            RangedAttackActions = 0;
        
        if (AbilityActionAllowedAfterMinimumMovement is false 
            || (anyMovementRemaining is false && AbilityActionAllowedAfterFullMovement is false))
            AbilityActions = 0;
    }

    private void HandleMeleeAttack()
    {
        MeleeAttackActions--;

        if (CanMoveAfterAnyAction)
            return;

        if (CanMoveAfterMeleeAttackAction is false)
            CanMove = false;
    }

    private void HandleRangedAttack()
    {
        RangedAttackActions--;

        if (CanMoveAfterAnyAction)
            return;

        if (CanMoveAfterRangedAttackAction is false)
            CanMove = false;
    }

    private void HandleAbility()
    {
        AbilityActions--;

        if (CanMoveAfterAnyAction)
            return;

        if (CanMoveAfterAbilityAction is false)
            CanMove = false;
    }
}