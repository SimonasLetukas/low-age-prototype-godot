using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Factions;
using LowAgeCommon;
using LowAgeData.Domain.Common;
using MultipurposePathfinding;
using Newtonsoft.Json;

/// <summary>
/// Parent of all entities (units & structures) and their rendering on the map.
/// </summary>
public partial class Entities : Node2D
{
    [Export] public bool DebugEnabled { get; set; } = false;
    
    public event Action<EntityPlacedRequestEvent> EntityPlaced = delegate { };
    public event Action<EntityCandidatePlacementCancelledEvent> CandidatePlacementCancelled = delegate { };
    public event Action<AbilityExecutionRequestedEvent> AbilityExecutionRequested = delegate { };
    public event Action<AbilityExecutionCompletedEvent> AbilityExecutionCompleted = delegate { };
    public event Action<EntityNode> NewPositionOccupied = delegate { };
    public event Action<EntityNode> EntitySelected = delegate { };
    public event Action<EntityNode> EntityDeselected = delegate { };

    public bool EntitiesBeingDestroyed => _entitiesBeingDestroyed.Count != 0;
    public bool EntityMoving { get; private set; } = false;
    public EntityNode? SelectedEntity { get; private set; } = null;
    public EntityNode? EntityInPlacement { get; private set; } = null;
    public EntityNode? HoveredEntity { get; private set; } = null;
    
    private EntityRenderers _renderers = null!;
    private Node2D _units = null!;
    private Node2D _structures = null!;
    private PlayerPriority _playerPriority = null!;

    private readonly Dictionary<Guid, EntityNode> _entitiesByIds = new();
    private readonly List<EntityNode> _entitiesBeingDestroyed = [];

    public override void _Ready()
    {
        _renderers = GetNode<EntityRenderers>(nameof(EntityRenderers));
        _units = GetNode<Node2D>("Units");
        _structures = GetNode<Node2D>("Structures");

        NewPositionOccupied += _renderers.UpdateSorting;
    }
    
    public void Initialize()
    {
        _playerPriority = new PlayerPriority
        {
            Queue = Players.Instance.GetAll().OrderBy(x => x.Id).ToList(),
        };
        
        GlobalRegistry.Instance.ProvideGetEntityById(GetEntityByInstanceId);
    }

    public void SetupStartingEntities(IList<Vector2Int> startingPositions, FactionId factionId)
    {
        var startingEntities = Data.Instance.Blueprint.Factions.First(x => x.Id.Equals(factionId))
            .StartingEntities;

        for (var i = 0; i < startingPositions.Count; i++)
        {
            if (i >= startingEntities.Count)
                continue;

            var entityBlueprint = Data.Instance.GetEntityBlueprintById(startingEntities[i]);
            PlaceEntity(entityBlueprint, startingPositions[i]);
        }
    }
    
    public override void _ExitTree()
    {
        NewPositionOccupied -= _renderers.UpdateSorting;
        
        foreach (var unit in _units.GetChildren().OfType<UnitNode>())
        {
            unit.FinishedMoving -= OnEntityFinishedMoving;
            unit.Destroyed -= OnEntityDestroyed;
            unit.Abilities.ExecutionRequested -= OnAbilityExecutionRequested;
        }
        foreach (var structure in _structures.GetChildren().OfType<StructureNode>())
        {
            structure.Destroyed -= OnEntityDestroyed;
            structure.Abilities.ExecutionRequested -= OnAbilityExecutionRequested;
        }
        
        base._ExitTree();
    }

    public override void _Process(double delta)
    {
        if (EntitiesBeingDestroyed)
            return;
        
        if (Input.IsActionJustPressed(Constants.Input.Rotate) && EntityInPlacement is ActorNode actor) 
            actor.Rotate();
        
        if (EntityMoving || EntityInPlacement != null)
            _renderers.UpdateSorting();
    }

    public EntityNode? GetEntityByInstanceId(Guid instanceId) => _entitiesByIds.ContainsKey(instanceId)
        ? _entitiesByIds[instanceId]
        : null;

    public IList<ActorNode> GetActorsSortedByInitiative()
    {
        var deterministicInitiative = Config.Instance.DeterministicInitiative;
        var actorsGroupedBySortedInitiative = GetActorInitiativeMap()
            .OrderByDescending(pair => deterministicInitiative ? (int)pair.Value : pair.Value)
            .ThenBy(pair => pair.Key.CreationToken)
            .GroupBy(pair => deterministicInitiative ? (int)pair.Value : pair.Value)
            .Select(group => group.Select(pair => pair.Key));

        var finalOrder = ResolveIdenticalInitiatives(actorsGroupedBySortedInitiative);
        
        if (DebugEnabled)
            GD.Print($"{nameof(Entities)}.{nameof(GetActorsSortedByInitiative)}: Final order of actors sorted " +
                     $"by initiative: {JsonConvert.SerializeObject(finalOrder.Select(x => new
                     {
                         Id = x.InstanceId, 
                         Name = x.DisplayName, 
                         Position = x.EntityPrimaryPosition 
                     }).ToList())}");
        
        return finalOrder;
    }

    public IList<EntityNode> GetCandidateEntities() => _entitiesByIds.Values
        .Where(e => e.IsCandidate())
        .ToList();

    public void AdjustGlobalPosition(EntityNode entity, Vector2 globalPosition) => entity.SnapTo(globalPosition);

    public void SelectEntity(EntityNode entity)
    {
        if (EntityMoving) 
            return;
        
        if (IsEntitySelected())
            SelectedEntity!.SetSelected(false);

        SelectedEntity = entity;
        SelectedEntity.SetSelected(true);
        EntitySelected(entity);
    }

    public void DeselectEntity()
    {
        if (IsEntitySelected() is false) 
            return;

        var unselectedEntity = SelectedEntity;
        unselectedEntity!.SetSelected(false);
        SelectedEntity = null;
        
        EntityDeselected(unselectedEntity);
    }

    public bool IsEntitySelected() => SelectedEntity != null;

    public bool IsEntitySelected(EntityNode entity) => IsEntitySelected() && SelectedEntity!.Equals(entity);

    public bool IsEntityHovered()
    {
        if (HoveredEntity is null)
            return false;

        if (HoveredEntity.IsBeingDestroyed)
        {
            HoveredEntity = null;
            return false;
        }

        return true;
    }

    public bool IsEntityHovered(EntityNode entity) => IsEntityHovered() && HoveredEntity!.Equals(entity);

    public bool TryHoveringEntity(EntityNode entity)
    {
        HoveredEntity?.SetTileHovered(false);
        entity.SetTileHovered(true);
        HoveredEntity = entity;
        return true;
    }
    
    public bool TryHoveringEntityOn(Tiles.TileInstance tile)
    {
        if (EntityMoving)
            return false;
        
        var occupationExists = tile.IsOccupied();
        var occupantEntity = tile.GetLastOccupantOrNull();
        
        if (occupationExists && IsEntityHovered(occupantEntity!))
            return true;

        HoveredEntity?.SetTileHovered(false);

        if (occupationExists)
        {
            occupantEntity?.SetTileHovered(true);
            HoveredEntity = occupantEntity;
            return true;
        }
        
        HoveredEntity = null;
        return false;
    }
    
    public EntityNode? GetTopEntity(Vector2 globalPosition)
    {
        var topZ = float.NegativeInfinity;
        EntityNode? topEntity = null;

        var colliders = Colliders.GetAt(globalPosition, GetWorld2D());
        
        foreach (var collider in colliders)
        {
            if ((collider.GetParent().GetParent() is EntityNode entity) is false)
                continue;

            if (entity.Renderer.ContainsSpriteAt(globalPosition) is false)
                continue;

            if (entity.Renderer.ZIndex <= topZ) 
                continue;
            
            topZ = entity.Renderer.ZIndex;
            topEntity = entity;
        }
        
        //if (DebugEnabled && topEntity != null)
            //GD.Print($"{nameof(Entities)}.{nameof(GetTopEntity)}: entity found '{topEntity.DisplayName}'");

        return topEntity;
    }
    
    public void MoveEntity(EntityNode entity, IList<Vector2> globalPath, IList<Point> path)
    {
        EntityMoving = true;
        entity.MoveUntilFinished(globalPath, path);
    }
    
    public void RegisterRenderer(EntityNode entity)
    {
        _renderers.RegisterRenderer(entity.Renderer);
        _renderers.UpdateSorting();
    }

    public EntityNode SetEntityForPlacement(EntityId entityId, bool canBePlacedOnTheWholeMap, int? cost)
    {
        var playerId = Players.Instance.Current.Id;
        var newEntityBlueprint = Data.Instance.GetEntityBlueprintById(entityId);
        var newEntity = InstantiateEntity(newEntityBlueprint, playerId, cost);
        
        newEntity.SetForPlacement(canBePlacedOnTheWholeMap);
        EntityInPlacement = newEntity;
        
        _renderers.RegisterRenderer(EntityInPlacement.Renderer);
        
        return newEntity;
    }

    public void UpdateEntityInPlacement(Vector2Int mapPosition, Vector2 globalPosition)
    {
        if (EntityInPlacement is null)
            return;
        
        EntityInPlacement.EntityPrimaryPosition = mapPosition;
        EntityInPlacement.SnapTo(globalPosition);
        EntityInPlacement.DeterminePlacementValidity(true);
    }

    public void CancelPlacement()
    {
        EntityInPlacement?.Destroy();
        EntityInPlacement = null;
    }

    public void DropDownToLowGround(IEnumerable<EntityNode> entities)
    {
        foreach (var entity in entities)
        {
            entity.DropDownToLowGround();
            if (entity.IsBeingDestroyed)
            {
                _renderers.UnregisterRenderer(entity.Renderer);
                continue;
            }
            
            NewPositionOccupied(entity);
        }
    }

    public void CancelCandidateEntities(IEnumerable<Guid> candidateEntities)
    {
        foreach (var cancelledCandidate in candidateEntities)
        {
            if (_entitiesByIds.TryGetValue(cancelledCandidate, out var entity) is false)
                continue;

            if (entity.IsCandidate() is false)
                continue;
            
            entity.Destroy();
        }
    }

    public void OnInterfaceCandidatePlacementCancelled(EntityNode entity)
    {
        if (Players.Instance.IsActionAllowedForCurrentPlayerOn(entity) is false)
            return;
        
        CancelCandidateEntities([entity.InstanceId]);
        CandidatePlacementCancelled(new EntityCandidatePlacementCancelledEvent
        {
            InstanceId = entity.InstanceId,
            PlayerId = entity.Player.Id
        });
    }

    public void HandleEvent(AbilityExecutionRequestedEvent @event)
    {
        var sourceActor = GetEntityByInstanceId(@event.SourceActorId) as ActorNode;
        if (sourceActor is null)
        {
            GD.Print($"{nameof(Entities)} could not apply {nameof(AbilityExecutionRequestedEvent)} because " +
                     $"{nameof(sourceActor)} '{@event.SourceActorId}' entity was null.");
            return;
        }

        var ability = sourceActor.Abilities.GetById(@event.AbilityId);
        if (ability is null)
        {
            GD.Print($"{nameof(Entities)} could not apply {nameof(AbilityExecutionRequestedEvent)} because " +
                     $"{nameof(ability)} '{@event.AbilityId}' was not found for {nameof(sourceActor)} " +
                     $"'{@event.SourceActorId}'.");
            return;
        }
        
        ability.OnExecutionRequested(@event.Focus);
        AbilityExecutionCompleted(new AbilityExecutionCompletedEvent
        {
            PlayerId = Players.Instance.Current.Id,
            AbilityExecutionRequestedEventId = @event.Id
        });
    }

    public void HandleEvent(EntityAttackedEvent @event)
    {
        var source = GetEntityByInstanceId(@event.SourceId) as ActorNode; // TODO how would doodads be able to execute attack?
        var target = GetEntityByInstanceId(@event.TargetId);

        if (source is null)
        {
            GD.Print($"{nameof(Entities)} could not apply {nameof(EntityAttackedEvent)} because " +
                     $"{nameof(source)} '{@event.SourceId}' entity was null.");
            return;
        }
        
        if (target is null)
        {
            GD.Print($"{nameof(Entities)} could not apply {nameof(EntityAttackedEvent)} because " +
                     $"{nameof(target)} '{@event.TargetId}' entity was null.");
            return;
        }

        source.ActionEconomy.Attacked(@event.AttackType);
        target.ReceiveAttack(source, @event.AttackType, false);
    }

    public void HandleEvent(EntityPlacedResponseEvent @event)
    {
        var entity = GetEntityByInstanceId(@event.InstanceId);
        if (entity != null)
        {
            entity.CreationToken = @event.CreationToken;
            PlaceEntity(entity, false);
            return;
        }
        
        var entityBlueprint = Data.Instance.GetEntityBlueprintById(@event.BlueprintId);
        entity = InstantiateEntity(entityBlueprint, @event.PlayerId, @event.Cost, @event.InstanceId);
        entity.ForcePlace(@event);
        
        NewPositionOccupied(entity);
    }

    public EntityNode? PlaceEntity()
    {
        if (EntityInPlacement is null)
            return null;
        
        var entity = EntityInPlacement;
        EntityInPlacement = null;
        
        _renderers.UnregisterRenderer(entity.Renderer);
        if (entity is StructureNode)
            entity.Renderer.MakeStatic();
        
        entity.DeterminePlacementValidity(true);
        return PlaceEntity(entity, true);
    }
    
    private EntityNode? PlaceEntity(Entity entityBlueprint, Vector2Int mapPosition)
    {
        var playerId = Players.Instance.Current.Id;
        var entity = InstantiateEntity(entityBlueprint, playerId, null);
        entity.EntityPrimaryPosition = mapPosition;
        entity.DeterminePlacementValidity(false);
        return PlaceEntity(entity, true);
    }

    private EntityNode? PlaceEntity(EntityNode entity, bool placeAsCandidate)
    {
        var instanceId = entity.InstanceId;
        
        if (DebugEnabled) GD.Print($"{nameof(Entities)}: placing {entity.DisplayName} '{instanceId}' at " +
                                   $"{entity.EntityPrimaryPosition}.");

        if (TryPlaceEntity(entity, placeAsCandidate) is false)
            return null;
        
        if (placeAsCandidate)
            EntityPlaced(new EntityPlacedRequestEvent
            {
                BlueprintId = entity.BlueprintId,
                MapPosition = entity.EntityPrimaryPosition,
                Cost = entity.HasCost ? entity.CreationProgress?.TotalCost : null,
                InstanceId = instanceId,
                ActorRotation = entity is ActorNode actor ? actor.ActorRotation : ActorRotation.BottomRight,
                PlayerId = entity.Player.Id
            });
        
        NewPositionOccupied(entity);
        
        return entity;
    }

    private static bool TryPlaceEntity(EntityNode entity, bool placeAsCandidate)
    {
        var placedSuccessfully = placeAsCandidate 
            ? entity.SetAsCandidate() 
            : entity.Place();
        
        return placedSuccessfully;
    }

    private EntityNode InstantiateEntity(Entity entityBlueprint, int playerId, int? cost, Guid? instanceId = null)
    {
        var player = Players.Instance.Get(playerId);

        EntityNode entity = entityBlueprint switch
        {
            Structure structure => InstantiateStructure(structure, player),
            Unit unit => InstantiateUnit(unit, player),
            _ => throw new ArgumentOutOfRangeException($"No possible cast for entity '{entityBlueprint.Id}'")
        };

        entity.Destroyed += OnEntityDestroyed;

        if (instanceId != null)
            entity.InstanceId = (Guid)instanceId;
        _entitiesByIds[entity.InstanceId] = entity;

        entity.SetCost(cost);
        
        return entity;
    }

    private StructureNode InstantiateStructure(Structure structureBlueprint, Player player)
    {
        var structure = StructureNode.InstantiateAsChild(structureBlueprint, _structures, player);

        structure.Abilities.ExecutionRequested += OnAbilityExecutionRequested;

        return structure;
    }

    private UnitNode InstantiateUnit(Unit unitBlueprint, Player player)
    {
        var unit = UnitNode.InstantiateAsChild(unitBlueprint, _units, player);

        unit.FinishedMoving += OnEntityFinishedMoving;
        unit.Abilities.ExecutionRequested += OnAbilityExecutionRequested;

        return unit;
    }
    
    private Dictionary<ActorNode, float> GetActorInitiativeMap()
    {
        var deterministicInitiative = Config.Instance.DeterministicInitiative;
        var entityInitiativeMap = new Dictionary<ActorNode, float>();
        
        foreach (var entity in _entitiesByIds.Values.OrderBy(e => e.CreationToken))
        {
            if (entity is not ActorNode actor 
                || actor.HasInitiative is false 
                || actor.IsCompleted() is false
                || actor.WorkingOn.Any(ability => ability.ConsumesAction 
                                                  && ability.Timing.Equals(TurnPhase.Planning)))
                continue;

            var initiativeBonus = (actor.Initiative!.MaxAmount - actor.Initiative!.CurrentAmount) / 1.5f;

            var initiative = deterministicInitiative 
                ? actor.Initiative!.MaxAmount 
                : Mathf.Max((float)Dice.RollMultiple(19, actor.Initiative!.MaxAmount).Sum() / 10 
                            + initiativeBonus, 0);

            actor.Initiative.CurrentAmount = initiative;
            
            if (DebugEnabled)
                GD.Print($"{nameof(Entities)}.{nameof(GetActorInitiativeMap)}: {actor.DisplayName} at " + 
                         $"{actor.EntityPrimaryPosition} {nameof(initiative)} {initiative}");
            
            entityInitiativeMap[actor] = initiative;
        }
        
        return entityInitiativeMap;
    }

    private List<ActorNode> ResolveIdenticalInitiatives(
        IEnumerable<IEnumerable<ActorNode>> entitiesGroupedBySortedInitiative)
    {
        var finalOrder = new List<ActorNode>();

        foreach (var group in entitiesGroupedBySortedInitiative)
        {
            var entitiesByPlayer = new Dictionary<Player, List<ActorNode>>();
            foreach (var entity in group)
            {
                if (entitiesByPlayer.ContainsKey(entity.Player) is false)
                    entitiesByPlayer[entity.Player] = [];
                
                entitiesByPlayer[entity.Player].Add(entity);
            }

            while (entitiesByPlayer.Values.Any(entities => entities.Count > 0))
            {
                var uniquePlayerEntityGroup = new List<ActorNode>();
                foreach (var player in entitiesByPlayer.Keys
                             .Where(player => entitiesByPlayer[player].Count > 0)
                             .OrderBy(player => player.Id))
                {
                    uniquePlayerEntityGroup.Add(entitiesByPlayer[player].First());
                    entitiesByPlayer[player].RemoveAt(0);
                }

                uniquePlayerEntityGroup.Sort((entityA, entityB) => 
                    _playerPriority.Compare(entityA.Player, entityB.Player));
                _playerPriority.Rotate();
                
                finalOrder.AddRange(uniquePlayerEntityGroup);
            }
        }

        return finalOrder;
    }

    private void OnEntityFinishedMoving(EntityNode entity)
    {
        EntityMoving = false;
        NewPositionOccupied(entity);
    }
    
    private void OnAbilityExecutionRequested(ActorNode sourceActor, IAbilityNode ability, IAbilityFocus focus)
    {
        AbilityExecutionRequested(new AbilityExecutionRequestedEvent
        {
            SourceActorId = sourceActor.InstanceId,
            AbilityId = ability.Id,
            Focus = focus
        });
    }

    private void OnEntityDestroyed(EntityNode entity)
    {
        _entitiesBeingDestroyed.Add(entity);
        
        if (IsEntityHovered(entity))
            HoveredEntity = null;
        
        EventBus.Instance.RaiseEntityDestroyed(entity);

        _renderers.UnregisterRenderer(entity.Renderer);
        
        entity.FinishedMoving -= OnEntityFinishedMoving;
        entity.Destroyed -= OnEntityDestroyed;
        if (entity is ActorNode actor)
            actor.Abilities.ExecutionRequested -= OnAbilityExecutionRequested;

        _entitiesByIds.Remove(entity.InstanceId);
        _entitiesBeingDestroyed.Remove(entity);
    }
    
    private class PlayerPriority
    {
        public required IList<Player> Queue { private get; init; }

        public int Compare(Player p1, Player p2) => Queue.IndexOf(p1).CompareTo(Queue.IndexOf(p2));

        public void Rotate()
        {
            if (Queue.Count < 1)
                return;
            
            var firstPlayer = Queue.First();
            Queue.RemoveAt(0);
            Queue.Add(firstPlayer);
        }
    }
}
