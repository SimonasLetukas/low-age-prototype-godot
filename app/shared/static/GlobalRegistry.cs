using System;
using System.Collections.Generic;
using Godot;
using LowAgeCommon;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Resources;

public partial class GlobalRegistry : Node
{
    public static GlobalRegistry Instance = null!;
    
    public override void _Ready()
    {
        base._Ready();
        
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }

    #region Resolvers

    public Vector2Int MapSize { get; private set; }
    public Func<Guid, EntityNode?> GetEntityById { get; private set; } = null!;
    public Func<TurnPhase> GetCurrentPhase { get; private set; } = null!;
    public Func<ActorNode?> GetActorInAction { get; private set; } = null!;
    public Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> GetHighestTiles { get; private set; } = null!;
    public Func<Vector2Int, bool, Tiles.TileInstance?> GetTile { get; private set; } = null!;
    public Func<IList<Payment>, string> StringifyResources { get; private set; } = null!;
    public Func<IList<Payment>, IList<Payment>> GetNonConsumableResources { get; private set; } = null!;
    public Func<Player, IList<Payment>> GetCurrentPlayerStockpile { get; private set; } = null!;
    public Func<Player, IList<Payment>> GetMaximumPlayerIncome { get; private set; } = null!;
    public Func<Player, float, int, IList<Payment>> GetActualPlayerIncome { get; private set; } = null!;
    public Func<ResourceId, IList<Payment>, IList<Payment>> GetResourcesStoredAs { get; private set; } = null!;
    public Func<IList<Payment>, IList<Payment>, IList<Payment>, float, int> SimulateProductionLength { get; private set; } = null!;
    public Func<IList<Payment>, IList<Payment>, IList<Payment>, float, (IList<Payment> ResourcesSpent, IList<Payment> UpdatedPayment)> SimulatePayment { get; private set; } = null!;
    public Func<IList<Payment>, IList<Payment>, bool> IsPaymentComplete { get; private set; } = null!;
    public Func<IList<Payment>, IList<Payment>, bool> CanSubtractResources { get; private set; } = null!;
    public Func<IList<Payment>, IList<Payment>, bool, IList<Payment>> SubtractResources { get; private set; } = null!;
    
    #endregion Resolvers

    #region Providers

    public void ProvideMapSize(Vector2Int mapSize) => MapSize = mapSize;
    public void ProvideGetEntityById(Func<Guid, EntityNode?> entityById) => GetEntityById = entityById;
    public void ProvideGetCurrentPhase(Func<TurnPhase> getCurrentPhase) => GetCurrentPhase = getCurrentPhase;
    public void ProvideGetActorInAction(Func<ActorNode?> getActorInAction) => GetActorInAction = getActorInAction;
    public void ProvideGetHighestTiles(Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> getHighestTiles) => GetHighestTiles = getHighestTiles;
    public void ProvideGetTile(Func<Vector2Int, bool, Tiles.TileInstance?> getTile) => GetTile = getTile;
    public void ProvideStringifyResources(Func<IList<Payment>, string> stringifyResources) => StringifyResources = stringifyResources;
    public void ProvideGetNonConsumableResources(Func<IList<Payment>, IList<Payment>> getNonConsumableResources) => GetNonConsumableResources = getNonConsumableResources;
    public void ProvideGetCurrentPlayerStockpile(Func<Player, IList<Payment>> getCurrentPlayerStockpile) => GetCurrentPlayerStockpile = getCurrentPlayerStockpile;
    public void ProvideGetMaximumPlayerIncome(Func<Player, IList<Payment>> getCurrentPlayerIncome) => GetMaximumPlayerIncome = getCurrentPlayerIncome;
    public void ProvideGetActualPlayerIncome(Func<Player, float, int, IList<Payment>> getActualPlayerIncome) => GetActualPlayerIncome = getActualPlayerIncome;
    public void ProvideGetResourcesStoredAs(Func<ResourceId, IList<Payment>, IList<Payment>> getResourcesStoredAs) => GetResourcesStoredAs = getResourcesStoredAs;
    public void ProvideSimulateProductionLength(Func<IList<Payment>, IList<Payment>, IList<Payment>, float, int> simulateProductionLength) => SimulateProductionLength = simulateProductionLength;
    public void ProvideSimulatePayment(Func<IList<Payment>, IList<Payment>, IList<Payment>, float, (IList<Payment> ResourcesSpent, IList<Payment> UpdatedPayment)> simulatePayment) => SimulatePayment = simulatePayment;
    public void ProvideIsPaymentComplete(Func<IList<Payment>, IList<Payment>, bool> isPaymentComplete) => IsPaymentComplete = isPaymentComplete;
    public void ProvideCanSubtractResources(Func<IList<Payment>, IList<Payment>, bool> canSubtractResources) => CanSubtractResources = canSubtractResources;
    public void ProvideSubtractResources(Func<IList<Payment>, IList<Payment>, bool, IList<Payment>> subtractResources) => SubtractResources = subtractResources;
    
    #endregion Providers
}