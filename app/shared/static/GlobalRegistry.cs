using System;
using System.Collections.Generic;
using Godot;
using LowAgeCommon;
using LowAgeData.Domain.Common;

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

    #endregion Resolvers

    #region Providers

    public void ProvideMapSize(Vector2Int mapSize) => MapSize = mapSize;
    public void ProvideGetEntityById(Func<Guid, EntityNode?> entityById) => GetEntityById = entityById;
    public void ProvideGetCurrentPhase(Func<TurnPhase> getCurrentPhase) => GetCurrentPhase = getCurrentPhase;
    public void ProvideGetActorInAction(Func<ActorNode?> getActorInAction) => GetActorInAction = getActorInAction;
    public void ProvideGetHighestTiles(Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> getHighestTiles) => GetHighestTiles = getHighestTiles;
    public void ProvideGetTile(Func<Vector2Int, bool, Tiles.TileInstance?> getTile) => GetTile = getTile;

    #endregion Providers
}