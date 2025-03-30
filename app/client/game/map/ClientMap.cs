using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Tiles;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using MultipurposePathfinding;
using Area = LowAgeCommon.Area;

public partial class ClientMap : Map
{
	[Export] public bool DebugLinesEnabled { get; set; } = false;

	public event Action FinishedInitializing = delegate { };
	public event Action<EntityNode> EntityIsBeingPlaced = delegate { };
	public event Action<UnitMovedAlongPathEvent> UnitMovementIssued = delegate { };
	public event Action<EntityAttackedEvent> EntityAttacked = delegate { };

	public Entities Entities { get; private set; } = null!;
	private Pathfinding Pathfinding { get; set; } = new();

	private Player _currentPlayer = null!;
	private IList<Area> _startingPositions = new List<Area>();
	private Vector2Int _mapSize = Vector2Int.Max;
	private Tiles _tileMap = null!;
	private FocusedTile _focusedTile = null!;
	private SelectionOverlay _selectionOverlay = SelectionOverlay.None;

	private enum SelectionOverlay
	{
		None,
		Movement,
		Placement,
		Attack
	}

	private AbilityNode? _selectedAbility;
	private (BuildNode?, EntityId?) _previousBuildSelection = (null, null);
	private Node2D _lines = null!;

	private bool _tileMapIsInitialized = false;
	private bool _pathfindingIsInitialized = false;
	private bool _tileMapPointsStartedInitialization = false;
	private bool _tileMapPointsInitialized = false;

	private bool _paused = true;

	public override void _Ready()
	{
		base._Ready();
		ProcessMode = ProcessModeEnum.Always;
		
		_tileMap = GetNode<Tiles>($"{nameof(Tiles)}");
		Entities = GetNode<Entities>($"{nameof(Entities)}");
		_lines = GetNode<Node2D>($"Lines");

		Entities.NewPositionOccupied += OnEntitiesNewPositionOccupied;
		Entities.Destroyed += OnEntitiesDestroyed;
		_tileMap.FinishedInitialInitializing += OnTileMapFinishedInitialInitializing;
		_tileMap.FinishedPointInitialization += OnTileMapFinishedPointInitialization;
		Pathfinding.FinishedInitializing += OnPathfindingFinishedInitializing;
		Pathfinding.PointAdded += OnPathfindingPointAdded;
		Pathfinding.PointRemoved += OnPathfindingPointRemoved;
		EventBus.Instance.EntityPlaced += OnEntityPlaced;
		EventBus.Instance.NewTileFocused += OnNewTileFocused;
		EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
	}

	public override void _ExitTree()
	{
		Entities.NewPositionOccupied -= OnEntitiesNewPositionOccupied;
		Entities.Destroyed += OnEntitiesDestroyed;
		_tileMap.FinishedInitialInitializing -= OnTileMapFinishedInitialInitializing;
		_tileMap.FinishedPointInitialization -= OnTileMapFinishedPointInitialization;
		Pathfinding.FinishedInitializing -= OnPathfindingFinishedInitializing;
		Pathfinding.PointAdded += OnPathfindingPointAdded;
		Pathfinding.PointRemoved += OnPathfindingPointRemoved;
		EventBus.Instance.EntityPlaced -= OnEntityPlaced;
		EventBus.Instance.NewTileFocused -= OnNewTileFocused;
		EventBus.Instance.PathfindingUpdating -= OnPathfindingUpdating;
		
		base._ExitTree();
	}

	#region Initialization

	public void Initialize(MapCreatedEvent @event)
	{
		if (DebugEnabled)
			GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} {nameof(ClientMap)}.{nameof(Initialize)}");

        _currentPlayer = Players.Instance.Current;
		_mapSize = @event.MapSize;
		_startingPositions = @event.StartingPositions[_currentPlayer.Id];

		Position = new Vector2(((float)Mathf.Max(_mapSize.X, _mapSize.Y) * Constants.TileWidth) / 2, Position.Y);
		_tileMap.Initialize(_mapSize.ToGodotVector2(), @event.Tiles);

		InitializePathfinding(@event.Tiles);

		_focusedTile = _tileMap.Elevatable.Focused;

		_lines.Visible = DebugEnabled && DebugLinesEnabled;
		ResetLines();
	}

	private void InitializePathfinding(ICollection<(Vector2Int, TileId)> tiles)
	{
		var blueprint = Data.Instance.Blueprint;
		var initialPositionsAndTerrainIndexes = tiles
			.Select(x => (x.Item1, new DijkstraMap.Terrain(blueprint.Tiles
				.First(y => y.Id.Equals(x.Item2)).Terrain.ToIndex())))
			.ToList();
		Pathfinding.Initialize(initialPositionsAndTerrainIndexes, new Configuration
		{
			MapSize = _mapSize,
			TerrainWeights = blueprint.Tiles.ToDictionary(
				tile => tile.Terrain.ToIndex(),
				tile => tile.MovementCost),
			MaxSizeForPathfinding = blueprint.Entities.Units.Max(x => x.Size),
			MaxNumberOfTeams = 2, // TODO
			DiagonalCost = Mathf.Sqrt2,
			DebugEnabled = false
		});
	}

	private void OnTileMapFinishedInitialInitializing()
	{
		if (DebugEnabled)
			GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
					 $"{nameof(ClientMap)}.{nameof(OnTileMapFinishedInitialInitializing)}");
		_tileMapIsInitialized = true;
		FinishInitialization();
	}

	private void OnPathfindingFinishedInitializing()
	{
		if (DebugEnabled)
			GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
					 $"{nameof(ClientMap)}.{nameof(OnPathfindingFinishedInitializing)}");
		_pathfindingIsInitialized = true;
		FinishInitialization();
	}

	private void OnTileMapFinishedPointInitialization()
	{
		if (DebugEnabled)
			GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
					 $"{nameof(ClientMap)}.{nameof(OnTileMapFinishedPointInitialization)}");
		_tileMapPointsInitialized = true;
		FinishInitialization();
	}

	private void FinishInitialization()
	{
		if (_tileMapIsInitialized is false || _pathfindingIsInitialized is false)
			return;

		if (_tileMapPointsStartedInitialization is false)
		{
			_tileMap.AddPoints(Pathfinding.GetTerrainPoints());
			_tileMapPointsStartedInitialization = true;
			return;
		}

		if (_tileMapPointsInitialized is false)
			return;

		Entities.Initialize(_tileMap.GetHighestTiles, _tileMap.GetTile);
		
		FinishedInitializing();
	}

	#endregion Initialization
	
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_pathfindingIsInitialized is false)
		{
			Pathfinding.IterateInitialization(delta);
			return;
		}

		if (_paused)
			return;
		
		var mousePosition = GetGlobalMousePosition();

		if (_selectionOverlay is SelectionOverlay.None)
		{
			UpdateHoveredEntity(mousePosition);
		}

		if (_selectionOverlay is SelectionOverlay.Movement)
		{
			UpdateHoveredEntity(mousePosition);

			// TODO optimization: only if focused tile changed from above, display path
			if (_focusedTile.IsWithinTheMap)
			{
				var size = Entities.SelectedEntity!.EntitySize.X;
				var path = Pathfinding.FindPath(
					_focusedTile.CurrentTile!.Point,
					Entities.SelectedEntity.Player.Team,
					size);
				_tileMap.Elevatable.SetPathTiles(path, size);
			}
		}

		if (_selectionOverlay is SelectionOverlay.Attack)
		{
			var hoveredEntity = UpdateHoveredEntity(mousePosition);
			UpdateTargetedEntity(hoveredEntity, Entities.SelectedEntity);
		}

		if (_selectionOverlay is SelectionOverlay.Placement)
		{
			var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(mousePosition);
			var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(mapPosition);
			Entities.UpdateEntityInPlacement(mapPosition, globalPosition);
		}

		HandleFlattenInput();
		HandleMovementAttackToggle();
	}

	public void SetupFactionStart()
	{
		Entities.SetupStartingEntities(_startingPositions.First().ToList(), _currentPlayer.Faction);
	}

	public void SetPaused(bool to)
	{
		_paused = to;
	}
	
	public void HandleEvent(UnitMovedAlongPathEvent @event)
	{
		var selectedEntity = Entities.GetEntityByInstanceId(@event.EntityInstanceId);
		if (selectedEntity is null)
			return;
		
		if (Entities.IsEntitySelected(selectedEntity))
			HandleDeselecting();
		
		RemoveOccupation(selectedEntity);
		Entities.MoveEntity(selectedEntity, @event.GlobalPath, @event.Path.ToList());
	}

	public void HandleDeselecting()
	{
		_tileMap.Elevatable.ClearAvailableTiles(false);
		_tileMap.Elevatable.ClearTargetTiles(false);
		_tileMap.Elevatable.ClearPath();
		Entities.DeselectEntity();
		_selectionOverlay = SelectionOverlay.None;
	}

	private void HandleFlattenInput()
	{
		if (Input.IsActionJustReleased(Constants.Input.Flatten) is false)
			return;

		ClientState.Instance.ToggleFlattened();
	}

	private void HandleMovementAttackToggle()
	{
		if (Input.IsActionJustReleased(Constants.Input.MovementAttackToggle) is false)
			return;
		
		if (_selectionOverlay is not (SelectionOverlay.Movement or SelectionOverlay.Attack)) 
			return;
		
		var entity = Entities.SelectedEntity!;

		var actor = entity as ActorNode;
		var actorHasAnyAttacks = actor != null && (actor.HasMeleeAttack || actor.HasRangedAttack);
		if (actorHasAnyAttacks is false)
			return;
		
		var clientState = ClientState.Instance;
		clientState.ToggleMovementAttackOverlay(entity);
		
		if (clientState.MovementToggled)
		{
			_tileMap.Elevatable.ClearTargetTiles(false);
			ShowMovementOverlay(entity);
			return;
		}

		if (clientState.AttackToggled)
		{
			_tileMap.Elevatable.ClearAvailableTiles(false);
			_tileMap.Elevatable.ClearPath();
			ShowAttackOverlay(actor!);
		}
	}

	private void HandleLeftClick()
	{
		if (_focusedTile.IsWithinTheMap is false)
			return;

		if (Entities.EntityMoving)
			return;

		_focusedTile.UpdateTile();

		if (_selectionOverlay is SelectionOverlay.None or SelectionOverlay.Movement or SelectionOverlay.Attack)
		{
			ExecuteEntitySelection();
			return;
		}

		if (_selectionOverlay is SelectionOverlay.Placement 
		    && IsActionAuthorizedForCurrentPlayerOnSelectedEntity()
		    && TryActivateAbilityInCurrentPhase())
		{
			ExecutePlacement();
			return;
		}
	}
	
	private void HandleRightClick()
	{
		if (_focusedTile.IsWithinTheMap is false)
			return;

		if (Entities.EntityMoving)
			return;

		if (_selectionOverlay is SelectionOverlay.None)
			return;

		if (_selectionOverlay is SelectionOverlay.Movement 
		    && IsActionAuthorizedForCurrentPlayerOnSelectedEntity()
		    && TryActivateAbilityInCurrentPhase())
		{
			_focusedTile.UpdateTile();
			ExecuteMovement();
			return;
		}

		if (_selectionOverlay is SelectionOverlay.Attack 
		    && IsActionAuthorizedForCurrentPlayerOnSelectedEntity()
		    && TryActivateAbilityInCurrentPhase())
		{
			_focusedTile.UpdateTile();
			ExecuteAttack();
			return;
		}

		if (_selectionOverlay is SelectionOverlay.Placement)
		{
			ExecuteCancellation();
			return;
		}
	}

	private void ExecuteEntitySelection(bool maintainSelection = false)
	{
		var mousePosition = GetGlobalMousePosition();
		var entity = maintainSelection
			? Entities.SelectedEntity
			: UpdateHoveredEntity(mousePosition);

		if (maintainSelection is false)
			_tileMap.Elevatable.ClearTargetTiles(false);
		
		HandleDeselecting();

		if (entity is null)
			return;

		Entities.SelectEntity(entity);
		
		ClientState.Instance.ResetMovementAttackOverlayToggle(entity);
		ShowMovementOverlay(entity);
	}

	private void ShowMovementOverlay(EntityNode entity)
	{
		if (entity is not UnitNode unit) 
			return;
		
		ShowMovementOverlay(unit);
	}
	
	private void ShowMovementOverlay(UnitNode unit)
	{
		var size = unit.EntitySize.X;
		var availablePoints = Pathfinding.GetAvailablePoints(
			unit.EntityPrimaryPosition,
			unit.Movement,
			unit.IsOnHighGround,
			unit.Player.Team,
			size);
		_tileMap.Elevatable.ClearAvailableTiles(false);
		_tileMap.Elevatable.SetAvailableTiles(unit, availablePoints, size, false);
		_selectionOverlay = SelectionOverlay.Movement;
	}

	private void ShowAttackOverlay(EntityNode entity)
	{
		if (entity is not ActorNode actor)
			return;

		ShowAttackOverlay(actor);
	}

	private void ShowAttackOverlay(ActorNode actor, bool? showMelee = null)
	{
		_tileMap.Elevatable.ClearTargetTiles(false);
		
		var availablePoints = actor is UnitNode unit 
			? Pathfinding.GetAvailablePoints(
				unit.EntityPrimaryPosition,
				unit.MeleeAttack?.MaximumDistance + Constants.Pathfinding.SearchIncrement ?? 0,
				unit.IsOnHighGround,
				unit.Player.Team,
			unit.EntitySize.X) 
			: [];
		
		var targetMeleeTiles = showMelee is null or true 
			? actor.GetMeleeAttackTargetTiles(_mapSize, availablePoints) 
			: [];
		var targetRangedTiles = showMelee is null or false 
			? actor.GetRangedAttackTargetTiles(_mapSize) 
			: [];
		
		if (targetMeleeTiles.Count > 0)
			_tileMap.Elevatable.SetTargetTiles(targetMeleeTiles, false, false, true);
		
		if (targetRangedTiles.Count > 0)
			_tileMap.Elevatable.SetTargetTiles(targetRangedTiles, false, false, false);
		
		_selectionOverlay = SelectionOverlay.Attack;
	}

	private void ExecutePlacement()
	{
		Entities.PlaceEntity();

		if (Input.IsActionPressed(Constants.Input.RepeatPlacement) && _previousBuildSelection.Item1 != null)
		{
			OnInterfaceSelectedToBuild(_previousBuildSelection.Item1, _previousBuildSelection.Item2!);
			return;
		}

		ExecuteCancellation();
	}

	private void ExecuteMovement()
	{
		if (_tileMap.Elevatable.IsCurrentlyAvailable(_focusedTile.CurrentTile) is false
			|| Entities.SelectedEntity!.EntityPrimaryPosition.Equals(_focusedTile.CurrentTile!.Position))
			return;

		// TODO automatically move and melee attack enemy unit; ranged attacks are more tricky

		var selectedEntity = Entities.SelectedEntity;
		var path = Pathfinding.FindPath(
			_focusedTile.CurrentTile.Point,
			selectedEntity.Player.Team,
			selectedEntity.EntitySize.X).ToList();
		var globalPath = _tileMap.GetGlobalPositionsFromMapPoints(path);
		UnitMovementIssued(new UnitMovedAlongPathEvent(selectedEntity.InstanceId, globalPath, path));
		HandleDeselecting();
	}

	private void ExecuteAttack()
	{
		if (_focusedTile.CurrentTile is not { } focusedTile
			|| focusedTile.TargetType is TargetType.None
		    || Entities.HoveredEntity is not { } targetEntity
		    || Entities.SelectedEntity is not { } selectedEntity)
			return;

		if (targetEntity.CanBeTargetedBy(selectedEntity) is false)
			return;
		
		var attackType = focusedTile.TargetType is TargetType.Melee ? AttackType.Melee : AttackType.Ranged;
		EntityAttacked(new EntityAttackedEvent
		{
			SourceId = selectedEntity.InstanceId,
			TargetId = targetEntity.InstanceId,
			AttackType = attackType
		});
		HandleDeselecting();
	}

	private void ExecuteCancellation()
	{
		Pathfinding.ClearCache();
		_tileMap.Elevatable.ClearCache();
		_tileMap.Elevatable.ClearTargetTiles(false);
		Entities.CancelPlacement();
		_previousBuildSelection = (null, null);
		_selectedAbility = null;
		_focusedTile.Enable();
		ExecuteEntitySelection(true);
	}

	private EntityNode? UpdateHoveredEntity(Vector2 mousePosition)
	{
		if (_focusedTile.IsWithinTheMap is false)
			return null;

		var topEntity = Entities.GetTopEntity(mousePosition);

		if (topEntity != null && Input.IsActionPressed(Constants.Input.FocusSelection) is false)
		{
			_focusedTile.FocusEntity(topEntity);
			_focusedTile.UpdateTile();

			if (Entities.IsEntityHovered(topEntity))
				return topEntity;
		}

		var entityWasHovered = Entities.TryHoveringEntityOn(_focusedTile.CurrentTile!);
		_focusedTile.StopEntityFocus(); // TODO remove flashing when focusing from one entity to another

		return entityWasHovered ? Entities.HoveredEntity : null;
	}

	private void UpdateTargetedEntity(EntityNode? target, EntityNode? source)
	{
		if (target is null || source is null || target.InstanceId.Equals(source.InstanceId) 
		    || _focusedTile.CurrentTile is null)
			return;

		var attackType = _focusedTile.CurrentTile.TargetType is TargetType.Melee ? AttackType.Melee : AttackType.Ranged; 
		EventBus.Instance.RaiseEntityTargeted(target, source, attackType);
	}

	private Vector2Int GetMapPositionFromMousePosition()
		=> _tileMap.GetMapPositionFromGlobalPosition(GetGlobalMousePosition());

	private void AddOccupation(EntityNode entity)
	{
		Pathfinding.ClearCache();
		_tileMap.Elevatable.ClearCache();
		
		_tileMap.AddOccupation(entity);

		var pathfindingEntity = new PathfindingEntity(entity.InstanceId, entity.EntityPrimaryPosition,
			entity.EntitySize, entity.Player.Team, entity is UnitNode { IsOnHighGround: true },
			entity.CanBeMovedThroughAt, entity.AllowsConnectionBetweenPoints);
		Pathfinding.AddOrUpdateEntity(pathfindingEntity);
		Pathfinding.UpdateAround(entity.InstanceId);
	}

	private void RemoveOccupation(EntityNode? entity)
	{
		if (entity is null)
			return;
		
		_tileMap.RemoveOccupation(entity);

		Pathfinding.RemoveEntity(entity.InstanceId);
		
		Pathfinding.ClearCache();
		_tileMap.Elevatable.ClearCache();
	}

	private void DropAllEntitiesDownToLowGround(IList<Vector2Int> positions)
	{
		var entities = new HashSet<EntityNode>();
		foreach (var position in positions)
		{
			var highGroundTile = _tileMap.GetTile(position, true);
			if (highGroundTile is null)
				continue;

			foreach (var occupant in highGroundTile.GetOccupants())
			{
				entities.Add(occupant);
			}
		}

		foreach (var entity in entities)
		{
			RemoveOccupation(entity);
		}
		
		Entities.DropDownToLowGround(entities);
	}
	
	private bool IsActionAuthorizedForCurrentPlayerOnSelectedEntity() 
		=> Players.Instance.IsActionAllowedForCurrentPlayerOn(Entities.SelectedEntity);

	private bool TryActivateAbilityInCurrentPhase() => _selectionOverlay switch
	{
		SelectionOverlay.Placement // or SelectionOverlay.Ability TODO
			when _selectedAbility is not null && _selectedAbility.TryActivate(CurrentPhase, ActorInAction) => true,

		SelectionOverlay.Movement or SelectionOverlay.Attack
			when Entities.SelectedEntity!.Equals(ActorInAction) => true,

		_ => false
	};

	private void ResetLines()
	{
		foreach (var node in _lines.GetChildren())
			node.QueueFree();
	}

	private void UpdateLines()
	{
		if (DebugEnabled is false || DebugLinesEnabled is false)
			return;

		ResetLines();

		const int size = 2;
		const int team = 1;

		for (var x = 0; x < 10; x++)
		{
			for (var y = 0; y < 10; y++)
			{
				var position = new Vector2Int(x, y);
				var tile = _tileMap.GetHighestTile(position);
				if (tile is null || tile.Point.IsImpassable)
					continue;

				for (var offsetX = -1; offsetX < 2; offsetX++)
				{
					for (var offsetY = -1; offsetY < 2; offsetY++)
					{
						var adjacentPosition = position + new Vector2Int(offsetX, offsetY);
						var adjacentTile = _tileMap.GetHighestTile(adjacentPosition);
						if (adjacentTile is null)
							continue;

						if (Pathfinding.HasConnection(tile.Point, adjacentTile.Point, team, size) is false
							|| tile.Point.IsImpassable)
							continue;

						var line = new Line2D();
						line.Width = 1;
						line.ZIndex = 4000;
						line.Points =
						[
							_tileMap.GetGlobalPositionFromMapPosition(position) - Position,
							_tileMap.GetGlobalPositionFromMapPosition(adjacentPosition) - Position
						];
						_lines.AddChild(line);
					}
				}
			}
		}
	}

	private void OnPathfindingUpdating(IPathfindingUpdatable data, bool isAdded)
	{
		Pathfinding.ClearCache();
		var entityId = data.GetParentEntity().InstanceId;
		switch (data)
		{
			case AscendableNode _:
				if (isAdded)
					Pathfinding.AddAscendableHighGround(entityId,
						data.LeveledPositionsWithoutSpriteOffset);
				else
					Pathfinding.RemoveAscendableHighGround(entityId);
				break;
			case HighGroundNode _:
				if (isAdded)
					Pathfinding.AddHighGround(entityId,
						data.FlattenedPositionsWithoutSpriteOffset);
				else
					Pathfinding.RemoveHighGround(entityId);
				break;
		}
		
		Pathfinding.UpdateAround(entityId);
	}

	private static void OnPathfindingPointAdded(Point point)
	{
		if (point.IsLowGround)
			return;
		
		EventBus.Instance.RaiseHighGroundPointCreated(point);
	}

	private static void OnPathfindingPointRemoved(Point point)
	{
		if (point.IsLowGround)
			return;
		
		EventBus.Instance.RaiseHighGroundPointRemoved(point);
	}

	private void OnNewTileFocused(Vector2Int mapPosition, Terrain terrain, IList<EntityNode>? occupants)
	{
		if (_focusedTile.IsWithinTheMap is false)
			return;

		if (occupants is null || occupants.IsEmpty())
		{
			_tileMap.Elevatable.ClearAvailableTiles(true);
			return;
		}

		if (occupants.Last() is UnitNode unit)
		{
			if (Entities.IsEntitySelected(unit))
				return;
			
			// TODO also show target hovering tiles
			
			var temporaryAvailablePoints = Pathfinding.GetAvailablePoints(
				unit.EntityPrimaryPosition,
				unit.GetReach(),
				unit.IsOnHighGround,
				unit.Player.Team,
				unit.EntitySize.X,
				true);

			_tileMap.Elevatable.SetAvailableTiles(unit, temporaryAvailablePoints, unit.EntitySize.X, true);
		}
		else
			_tileMap.Elevatable.ClearAvailableTiles(true);
	}

	private void OnEntityPlaced(EntityNode entity) => AddOccupation(entity);

	private void OnEntitiesNewPositionOccupied(EntityNode entity)
	{
		var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(entity.EntityPrimaryPosition);
		Entities.AdjustGlobalPosition(entity, globalPosition);
		Entities.RegisterRenderer(entity);

		AddOccupation(entity);
		UpdateLines();
	}
	
	private void OnEntitiesDestroyed(EntityNode entity)
	{
		if (Entities.IsEntitySelected(entity))
			HandleDeselecting();

		if (entity.ProvidesHighGround)
			DropAllEntitiesDownToLowGround(entity.EntityOccupyingPositions);
			
		RemoveOccupation(entity);
	}

	internal void OnMouseLeftReleasedWithoutDrag()
	{
		HandleLeftClick();
	}

	internal void OnMouseRightReleasedWithoutExamine()
	{
		HandleRightClick();
	}

	internal void OnInterfaceSelectedToBuild(BuildNode buildAbility, EntityId entityId)
	{
		_previousBuildSelection = (buildAbility, entityId);

		Entities.CancelPlacement();
		_tileMap.Elevatable.ClearAvailableTiles(false);
		_tileMap.Elevatable.ClearPath();
		_focusedTile.Disable();

		var canBePlacedOnTheWholeMap = buildAbility.CanBePlacedOnTheWholeMap();
		var placementTiles = buildAbility.GetPlacementPositions(Entities.SelectedEntity!, _mapSize)
			.Select(position => _tileMap.GetTile(position, false))
			.WhereNotNull();
		_tileMap.Elevatable.SetTargetTiles(placementTiles, canBePlacedOnTheWholeMap, false);

		var entity = Entities.SetEntityForPlacement(entityId, canBePlacedOnTheWholeMap);
		// TODO pass in the cost to be tracked inside the buildableNode

		_selectedAbility = buildAbility;
		_selectionOverlay = SelectionOverlay.Placement;
		EntityIsBeingPlaced(entity);
	}

	internal void OnEntityPlaced()
	{
		if (_selectionOverlay is SelectionOverlay.Placement)
			return;

		ExecuteCancellation();
	}

	public void OnInterfaceAttackSelected(bool started, AttackType? attackType)
	{
		if (_selectionOverlay is SelectionOverlay.None or SelectionOverlay.Placement 
		    || Entities.SelectedEntity is not ActorNode actor)
			return;

		var clientState = ClientState.Instance;
		
		if (started is false)
		{
			if (clientState.AttackToggled)
				clientState.ToggleMovementAttackOverlay(actor);
			
			_tileMap.Elevatable.ClearTargetTiles(false);
			ShowMovementOverlay(actor);
			return;
		}
		
		if (clientState.MovementToggled)
			clientState.ToggleMovementAttackOverlay(actor);
		
		_tileMap.Elevatable.ClearAvailableTiles(false);
		_tileMap.Elevatable.ClearPath();
		bool? showMelee = attackType switch
		{
			null => null,
			_ when attackType.Equals(AttackType.Ranged) => false,
			_ when attackType.Equals(AttackType.Melee) => true,
			_ => null,
		};
		ShowAttackOverlay(actor, showMelee);
	}
}
