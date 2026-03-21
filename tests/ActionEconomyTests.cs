using FluentAssertions;
using LowAgeData.Domain.Common;

namespace LowAgeTests;

public class ActionEconomyTests
{
    private readonly ActionEconomy _sut = new();

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Restore_ShouldAssignConfiguredValues_WhenCalled(bool isPlanning, bool restoringOnlyAbilityAction)
    {
        _sut.Config.MaxMeleeAttackActions = 3;
        _sut.Config.MaxRangedAttackActions = 4;
        _sut.Config.MaxAbilitiesInActionPhase = 5;
        _sut.Config.MaxAbilitiesInPlanningPhase = 6;
        _sut.Moved(100, false);
        var turnPhase = isPlanning ? TurnPhase.Planning : TurnPhase.Action;
        
        _sut.Restore(turnPhase, restoringOnlyAbilityAction);

        _sut.MeleeAttackActions.Should().Be(turnPhase.Equals(TurnPhase.Planning)
            ? 0 
            : 3);
        _sut.RangedAttackActions.Should().Be(restoringOnlyAbilityAction || turnPhase.Equals(TurnPhase.Planning) 
            ? 0
            : 4);
        _sut.AbilityActions.Should().Be(restoringOnlyAbilityAction 
            ? 0 // Because moved 
            : turnPhase.Equals(TurnPhase.Planning) 
                ? 6 
                : 5);
        _sut.CanMove.Should().Be(restoringOnlyAbilityAction is false && turnPhase.Equals(TurnPhase.Action));
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, true, true)]
    public void RestoreOnlyAbilityAction_ShouldAssignConfiguredValuesInActionPhase_WhenSomeActionsWereAlreadyDone(
        bool moved, bool meleeAttacked, bool rangedAttacked)
    {
        _sut.Config.MaxMeleeAttackActions = 1;
        _sut.Config.MaxRangedAttackActions = 2;
        _sut.Config.MaxAbilitiesInActionPhase = 1;
        
        if (moved)
            _sut.Moved(100, false);
        if (meleeAttacked)
            _sut.Attacked(AttackType.Melee);
        if (rangedAttacked)
            _sut.Attacked(AttackType.Ranged);
        
        _sut.UsedAbilityAction();
        
        _sut.Restore(TurnPhase.Action, true);
        
        _sut.MeleeAttackActions.Should().Be(meleeAttacked || rangedAttacked 
            ? 0 
            : 1);
        _sut.RangedAttackActions.Should().Be(moved || meleeAttacked 
            ? 0 
            : rangedAttacked 
                ? 1 
                : 2);
        _sut.AbilityActions.Should().Be(moved || meleeAttacked || rangedAttacked 
            ? 0 
            : 1);
        _sut.CanMove.Should().Be((moved || meleeAttacked || rangedAttacked) is false);
    }

    [Theory]
    [InlineData(
        0, 
        true, 
        true, 
        true, 
        false, 
        false, 
        false, 
        false, 
        true, 
        true,
        true,
        true)]
    [InlineData(
        0.5f, 
        false, 
        true, 
        true, 
        false, 
        false, 
        false, 
        false, 
        true, 
        true,
        true,
        false)]
    [InlineData(
        1.6f, 
        true, 
        true, 
        true, 
        false, 
        false, 
        false, 
        false, 
        true, 
        false,
        false,
        true)]
    [InlineData(
        1.6f, 
        false, 
        true, 
        false, 
        false, 
        true, 
        false, 
        false, 
        false, 
        false,
        false,
        false)]
    [InlineData(
        1.6f, 
        false, 
        true, 
        false, 
        false, 
        true, 
        true, 
        true, 
        false, 
        false,
        true,
        false)]
    public void Moved_ShouldHandleCorrectly_WhenCalled(float movementValue, bool anyMovementRemaining, 
        bool meleeAttackActionAllowedAfterMinimumMovement, bool meleeAttackActionAllowedAfterFullMovement,
        bool rangedAttackActionAllowedAfterMinimumMovement, bool rangedAttackActionAllowedAfterFullMovement,
        bool abilityActionAllowedAfterMinimumMovement, bool abilityActionAllowedAfterFullMovement,
        bool expectedCanMeleeAttack, bool expectedCanRangedAttack, bool expectedCanUseAbilityAction, 
        bool expectedCanMove)
    {
        _sut.Config.MeleeAttackActionAllowedAfterMinimumMovement = meleeAttackActionAllowedAfterMinimumMovement;
        _sut.Config.MeleeAttackActionAllowedAfterFullMovement = meleeAttackActionAllowedAfterFullMovement;
        _sut.Config.RangedAttackActionAllowedAfterMinimumMovement = rangedAttackActionAllowedAfterMinimumMovement;
        _sut.Config.RangedAttackActionAllowedAfterFullMovement = rangedAttackActionAllowedAfterFullMovement;
        _sut.Config.AbilityActionAllowedAfterMinimumMovement = abilityActionAllowedAfterMinimumMovement;
        _sut.Config.AbilityActionAllowedAfterFullMovement = abilityActionAllowedAfterFullMovement;
        _sut.Restore(TurnPhase.Action);
        
        _sut.Moved(movementValue, anyMovementRemaining);

        _sut.CanMeleeAttack.Should().Be(expectedCanMeleeAttack);
        _sut.CanRangedAttack.Should().Be(expectedCanRangedAttack);
        _sut.CanUseAbilityAction.Should().Be(expectedCanUseAbilityAction);
        _sut.CanMove.Should().Be(expectedCanMove);
    }

    [Theory]
    [InlineData(
        true, 
        1, 
        false, 
        false, 
        false, 
        false)]
    [InlineData(
        true, 
        2, 
        false, 
        false, 
        true, 
        false)]
    [InlineData(
        true, 
        1, 
        false, 
        true, 
        false, 
        true)]
    [InlineData(
        true, 
        1, 
        true, 
        false, 
        false, 
        true)]
    [InlineData(
        true, 
        2, 
        true, 
        false, 
        true, 
        true)]
    [InlineData(
        false, 
        1, 
        false, 
        false, 
        false, 
        false)]
    [InlineData(
        false, 
        2, 
        false, 
        false, 
        true, 
        false)]
    [InlineData(
        false, 
        1, 
        false, 
        true, 
        false, 
        true)]
    [InlineData(
        false, 
        1, 
        true, 
        false, 
        false, 
        true)]
    [InlineData(
        false, 
        2, 
        true, 
        false, 
        true, 
        true)]
    public void Attacked_ShouldHandleCorrectly_WhenCalled(bool isMeleeAttack, int startingActions, 
        bool canMoveAfterAnyAction, bool canMoveAfterAction, bool expectedCanAttack, bool expectedCanMove)
    {
        if (isMeleeAttack)
        {
            _sut.Config.MaxMeleeAttackActions = startingActions;
            _sut.Config.CanMoveAfterMeleeAttackAction = canMoveAfterAction;
        }
        else
        {
            _sut.Config.MaxRangedAttackActions = startingActions;
            _sut.Config.CanMoveAfterRangedAttackAction = canMoveAfterAction;
        }
        _sut.Config.CanMoveAfterAnyAction = canMoveAfterAnyAction;
        _sut.Restore(TurnPhase.Action);

        var attackType = isMeleeAttack ? AttackType.Melee : AttackType.Ranged;
        _sut.Attacked(attackType);

        if (isMeleeAttack)
            _sut.CanMeleeAttack.Should().Be(expectedCanAttack);
        else
            _sut.CanRangedAttack.Should().Be(expectedCanAttack);
        _sut.CanMove.Should().Be(expectedCanMove);
    }
}