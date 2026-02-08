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
	private Area _startingPositions;
	private Vector2Int _mapSize = Vector2Int.Max;
	private Tiles _tileMap = null!;
	private FocusedTile _focusedTile = null!;
	private ActorNode? _hoveredInitiativePanelActor;
	private SelectionOverlay _selectionOverlay = SelectionOverlay.None;

	private enum SelectionOverlay
	{
		None,
		Movement,
		Attack,
		Target,
		Placement,
	}

	private IAbilityNode? _selectedAbility;
	private (BuildNode?, EntityId?) _previousBuildSelection = (null, null);
	private Node2D _lines = null!;
	private Node2D _arrows = null!;

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
		_arrows = GetNode<Node2D>($"Arrows");
		
		ResetLines();
		ClearArrows();
		
		Entities.NewPositionOccupied += OnEntitiesNewPositionOccupied;
		_tileMap.FinishedInitialInitializing += OnTileMapFinishedInitialInitializing;
		_tileMap.FinishedPointInitialization += OnTileMapFinishedPointInitialization;
		Pathfinding.FinishedInitializing += OnPathfindingFinishedInitializing;
		Pathfinding.PointAdded += OnPathfindingPointAdded;
		Pathfinding.PointRemoved += OnPathfindingPointRemoved;
		EventBus.Instance.EntityPlaced += OnEntityPlaced;
		EventBus.Instance.EntityDestroyed += OnEntityDestroyed;
		EventBus.Instance.NewTileFocused += OnNewTileFocused;
		EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
	}

	public override void _ExitTree()
	{
		Entities.NewPositionOccupied -= OnEntitiesNewPositionOccupied;
		_tileMap.FinishedInitialInitializing -= OnTileMapFinishedInitialInitializing;
		_tileMap.FinishedPointInitialization -= OnTileMapFinishedPointInitialization;
		Pathfinding.FinishedInitializing -= OnPathfindingFinishedInitializing;
		Pathfinding.PointAdded -= OnPathfindingPointAdded;
		Pathfinding.PointRemoved -= OnPathfindingPointRemoved;
		EventBus.Instance.EntityPlaced -= OnEntityPlaced;
		EventBus.Instance.EntityDestroyed -= OnEntityDestroyed;
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
		
		GlobalRegistry.Instance.ProvideGetCurrentPhase(() => CurrentPhase);
		GlobalRegistry.Instance.ProvideGetActorInAction(() => ActorInAction);
		GlobalRegistry.Instance.ProvideGetHighestTiles(_tileMap.GetHighestTiles);
		GlobalRegistry.Instance.ProvideGetTile(_tileMap.GetTile);
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

		Entities.Initialize();
		
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

			if (Entities.SelectedEntity is not UnitNode selectedUnit || selectedUnit.ActionEconomy.CanMove is false)
				return;

			// TODO optimization: only if focused tile changed from above, display path
			if (_focusedTile.IsWithinTheMap || _hoveredInitiativePanelActor != null)
			{
				var to = _hoveredInitiativePanelActor?.EntityPrimaryTile.Point 
				         ?? _focusedTile.CurrentTile!.Point;
				var team = Entities.SelectedEntity!.Player.Team;
				var size = Entities.SelectedEntity!.EntitySize.X;
				var path = Pathfinding.FindPath(to, team, size);
				_tileMap.Elevatable.SetPathTiles(path, size);
			}
		}

		if (_selectionOverlay is SelectionOverlay.Attack or SelectionOverlay.Target)
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
		Entities.SetupStartingEntities(_startingPositions, _currentPlayer.Faction);
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
		Entities.MoveEntity(selectedEntity, @event.GlobalPath.ToList(), @event.Path.ToList());
	}

	public void HandleDeselecting()
	{
		_tileMap.Elevatable.ClearAvailableTiles(false);
		_tileMap.Elevatable.ClearTargetTiles(false);
		_tileMap.Elevatable.ClearPath();
		ClearArrows();
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
		
		if (_selectionOverlay is not (SelectionOverlay.Movement or SelectionOverlay.Attack or SelectionOverlay.None)) 
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

		if (_selectionOverlay is SelectionOverlay.Target or SelectionOverlay.Placement)
		{
			ExecuteCancellation();
			return;
		}
	}
	
	private ValidationResult HandleRightClick()
	{
		if (_focusedTile.IsWithinTheMap is false)
			return ValidationResult.Invalid("This action is outside of the map boundaries!");

		if (Entities.EntityMoving)
			return ValidationResult.Invalid("Cannot execute while another unit is moving.");

		if (_selectionOverlay is SelectionOverlay.None)
			return ValidationResult.Invalid("Nothing to execute.");
		
		_focusedTile.UpdateTile();

		var isActionAuthorized = IsActionAuthorizedForCurrentPlayerOnSelectedEntity();
		if (isActionAuthorized.IsValid is false)
			return isActionAuthorized;

		return _selectionOverlay switch
		{
			SelectionOverlay.Movement => ExecuteMovement(),
			SelectionOverlay.Attack => ExecuteAttack(),
			SelectionOverlay.Target => ExecuteTarget(),
			SelectionOverlay.Placement => ExecutePlacement(),
			_ => ValidationResult.Invalid("Nothing to execute.")
		};
	}

	private void ExecuteEntitySelection(bool maintainCurrentlySelectedEntity = false)
	{
		var mousePosition = GetGlobalMousePosition();
		var entity = maintainCurrentlySelectedEntity
			? Entities.SelectedEntity
			: UpdateHoveredEntity(mousePosition);

		if (maintainCurrentlySelectedEntity is false)
			_tileMap.Elevatable.ClearTargetTiles(false);
		
		ExecuteEntitySelection(entity);
	}
	
	private void ExecuteEntitySelection(EntityNode? entity)
	{
		HandleDeselecting();

		if (entity is null)
			return;

		var initialOverlay = entity is ActorNode actor
			? actor.ActionEconomy.CanMove
				? SelectionOverlay.Movement
				: actor.ActionEconomy.CanMeleeAttack || actor.ActionEconomy.CanRangedAttack
					? SelectionOverlay.Attack
					: SelectionOverlay.None
			: SelectionOverlay.None;

		switch (initialOverlay)
		{
			case SelectionOverlay.Movement:
				ClientState.Instance.ResetMovementAttackOverlayToggle(true, entity);
				ShowMovementOverlay(entity);
				break;
			case SelectionOverlay.Attack:
				ClientState.Instance.ResetMovementAttackOverlayToggle(false, entity);
				ShowAttackOverlay(entity);
				break;
			default:
				ClientState.Instance.ResetMovementAttackOverlayToggle(true, entity);
				break;
		}

		ShowArrowsForEntity(entity);
		
		Entities.SelectEntity(entity);
	}

	private void ShowMovementOverlay(EntityNode entity)
	{
		if (entity is not UnitNode unit) 
			return;
		
		ShowMovementOverlay(unit);
	}
	
	private void ShowMovementOverlay(UnitNode unit)
	{
		if (unit.ActionEconomy.CanMove is false)
			return;
		
		var size = unit.EntitySize.X;
		var availablePoints = Pathfinding.GetAvailablePoints(
			unit.EntityPrimaryPosition,
			unit.Movement.CurrentAmount,
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
		if (actor.CanAttack(showMelee) is false)
			return;
		
		_tileMap.Elevatable.ClearTargetTiles(false);
		
		var availablePoints = actor is UnitNode unit 
			? Pathfinding.GetAvailablePoints(
				unit.EntityPrimaryPosition,
				unit.MeleeAttack?.MaximumDistance + Constants.Pathfinding.SearchIncrement ?? 0,
				unit.IsOnHighGround,
				unit.Player.Team,
			unit.EntitySize.X) 
			: [];
		
		var targetRangedTiles = showMelee is null or false && actor.ActionEconomy.CanRangedAttack
			? actor.GetRangedAttackTargetTiles(_mapSize) 
			: [];
		var targetMeleeTiles = showMelee is null or true && actor.ActionEconomy.CanMeleeAttack
			? actor.GetMeleeAttackTargetTiles(_mapSize, availablePoints) 
			: [];
		
		// The order is important here, because melee target tiles should always replace ranged ones when both overlap 
		if (targetRangedTiles.Count > 0)
			_tileMap.Elevatable.SetTargetTiles(targetRangedTiles, false, false, 
				ElevatableTiles.TargetPurpose.AttackRanged);
		if (targetMeleeTiles.Count > 0)
			_tileMap.Elevatable.SetTargetTiles(targetMeleeTiles, false, false, 
				ElevatableTiles.TargetPurpose.AttackMelee);
		
		_selectionOverlay = SelectionOverlay.Attack;
	}
	
	private void ShowArrowsForEntity(EntityNode entity)
	{
		if (entity.CreationProgress is not null)
		{
			foreach (var helper in entity.CreationProgress.Helpers.Keys)
			{
				ShowArrow(helper.OwnerActor.GlobalPosition, entity.GlobalPosition, helper.OwnerActor.Renderer.ZIndex);
			}
		}

		if (entity is not ActorNode actor) 
			return;
		
		foreach (var workingOnAbility in actor.WorkingOn)
		{
			if (workingOnAbility.Ability is not IAbilityHasTargetArea abilityWithTargetArea)
				continue;
				
			foreach (var targetGlobalPosition in abilityWithTargetArea.GetGlobalPositionsOfFocusedTargets())
			{
				ShowArrow(entity.GlobalPosition, targetGlobalPosition, entity.Renderer.ZIndex);
			}
		}
	}

	private void ShowArrow(Vector2 fromGlobalPosition, Vector2 toGlobalPosition, int zIndex)
	{
		var arrow = ParabolicArrow.InstantiateAsChild(_arrows, Vector2.Zero, Vector2.Zero);

		arrow.Start = ToLocal(fromGlobalPosition);
		arrow.End = ToLocal(toGlobalPosition);
		arrow.ZIndex = zIndex;
		arrow.Redraw();
	}

	private void ClearArrows()
	{
		foreach (var arrow in _arrows.GetChildren())
		{
			arrow.QueueFree();
		}
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

	private ValidationResult ExecuteMovement()
	{
		if (Entities.SelectedEntity!.Equals(ActorInAction) is false)
			return ValidationResult.Invalid("It is not your turn!");
		
		if (_tileMap.Elevatable.IsCurrentlyAvailable(_focusedTile.CurrentTile) is false
			|| Entities.SelectedEntity!.EntityPrimaryPosition.Equals(_focusedTile.CurrentTile!.Position))
			return ValidationResult.Invalid("The target tile is outside of the available movement range.");
		
		if (Entities.SelectedEntity is not UnitNode selectedUnit)
			return ValidationResult.Invalid("Movement is impossible!");

		if (selectedUnit.ActionEconomy.CanMove is false)
			return ValidationResult.Invalid("Can't move!");
		
		// TODO automatically move and melee attack enemy unit; ranged attacks are more tricky

		var path = Pathfinding.FindPath(
			_focusedTile.CurrentTile.Point,
			selectedUnit.Player.Team,
			selectedUnit.EntitySize.X).ToList();
		var globalPath = _tileMap.GetGlobalPositionsFromMapPoints(path);
		UnitMovementIssued(new UnitMovedAlongPathEvent(selectedUnit.InstanceId, globalPath, path));
		HandleDeselecting();
		
		return ValidationResult.Valid;
	}

	private ValidationResult ExecuteAttack()
	{
		if (_focusedTile.CurrentTile is not { } focusedTile
		    || focusedTile.TargetType is TargetType.None
		    || Entities.HoveredEntity is not { } targetEntity
		    || Entities.SelectedEntity is not { } selectedEntity)
			return ValidationResult.Invalid("Invalid attack target.");
		
		if (selectedEntity.Equals(ActorInAction) is false)
			return ValidationResult.Invalid("It is not your turn!");

		var canBeTargeted = targetEntity.CanBeTargetedBy(selectedEntity);
		if (canBeTargeted.IsValid is false)
			return canBeTargeted;
		
		var attackType = focusedTile.TargetType is TargetType.Melee ? AttackType.Melee : AttackType.Ranged;
		
		if (selectedEntity is not ActorNode selectedActor) // TODO how would doodads be able to execute attack?
			return ValidationResult.Invalid("This unit cannot attack!");

		if (selectedActor.CanAttack(attackType) is false)
			return ValidationResult.Invalid($"This unit has no action for a {attackType} attack.");
		
		EntityAttacked(new EntityAttackedEvent
		{
			SourceId = selectedEntity.InstanceId,
			TargetId = targetEntity.InstanceId,
			AttackType = attackType
		});
		HandleDeselecting();
		
		return ValidationResult.Valid;
	}

	private ValidationResult ExecuteTarget()
	{
		IAbilityActivationRequest? request = _selectedAbility switch
		{
			TargetNode => new TargetNode.ActivationRequest
			{
				UseConsumableResources = true,
				TileToTarget = _focusedTile.CurrentTile,
				EntityToTarget = Entities.HoveredEntity
			},
			BuildNode => new BuildNode.ActivationRequest
			{
				UseConsumableResources = false, // Should have been consumed when placing
				EntityAlreadyPlaced = true,
				EntityToBuild = Entities.HoveredEntity
			},
			_ => null
		};

		if (request is null || _selectedAbility is null)
		{
			ExecuteCancellation();
			return ValidationResult.Invalid("This ability cannot target an area.");
		}
		
		var result = _selectedAbility.Activate(request);
		if (result.IsValid is false)
			return result;
		
		ExecuteCancellation();
		return ValidationResult.Valid;
	}
	
	private ValidationResult ExecutePlacement()
	{
		if (_selectedAbility is not BuildNode buildAbility)
		{
			ExecuteCancellation();
			return ValidationResult.Invalid("This ability cannot execute placement action.");
		}

		var request = new BuildNode.ActivationRequest
		{
			UseConsumableResources = true,
			EntityAlreadyPlaced = false, 
			EntityToBuild = Entities.EntityInPlacement,
		};
		
		var result = buildAbility.Activate(request);
		if (result.IsValid is false)
			return result;
		
		var placedEntity = Entities.PlaceEntity();
		if (placedEntity is null)
		{
			ExecuteCancellation();
			return ValidationResult.Invalid("Placement was not successful.");
		}

		if (Input.IsActionPressed(Constants.Input.RepeatPlacement) && _previousBuildSelection.Item1 != null)
		{
			OnInterfaceSelectedToBuild(_previousBuildSelection.Item1, _previousBuildSelection.Item2!);
			return ValidationResult.Valid;
		}

		ExecuteCancellation();
		return ValidationResult.Valid;
	}

	private EntityNode? UpdateHoveredEntity(Vector2 mousePosition)
	{
		if (_hoveredInitiativePanelActor != null)
		{
			_focusedTile.FocusEntity(_hoveredInitiativePanelActor);
			_focusedTile.UpdateTile();
			return Entities.TryHoveringEntity(_hoveredInitiativePanelActor) 
				? Entities.HoveredEntity 
				: null;
		}
		
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
	
	private ValidationResult IsActionAuthorizedForCurrentPlayerOnSelectedEntity() 
		=> Players.Instance.IsActionAllowedForCurrentPlayerOn(Entities.SelectedEntity) 
			? ValidationResult.Valid 
			: ValidationResult.Invalid("Cannot execute actions for another player.");

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

	protected override void OnActionStarted(ActorNode actor)
	{
		base.OnActionStarted(actor);

		if (Players.Instance.IsActionAllowedForCurrentPlayerOn(actor) is false)
			return;
		
		ExecuteEntitySelection(actor);
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
		if (_focusedTile.IsWithinTheMap is false && _hoveredInitiativePanelActor is null)
			return;

		var occupant = _hoveredInitiativePanelActor ?? occupants?.LastOrDefault();

		if (occupant is null)
		{
			_tileMap.Elevatable.ClearAvailableTiles(true);
			return;
		}

		if (occupant is UnitNode unit)
		{
			if (Entities.IsEntitySelected(unit))
				return;
			
			var temporaryAvailablePoints = Pathfinding.GetAvailablePoints(
				unit.EntityPrimaryPosition,
				unit.GetReach(),
				unit.IsOnHighGround,
				unit.Player.Team,
				unit.EntitySize.X,
				true);

			// TODO also show target hovering tiles
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
	
	private void OnEntityDestroyed(EntityNode entity)
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
		var result = HandleRightClick();
		
		if (result.IsValid is false)
			EventBus.Instance.RaiseValidationError(result.Message);
	}

	internal void OnInterfaceSelectedToBuild(BuildNode buildAbility, EntityId entityId)
	{
		_previousBuildSelection = (buildAbility, entityId);

		Entities.CancelPlacement();
		_tileMap.Elevatable.ClearAvailableTiles(false);
		_tileMap.Elevatable.ClearTargetTiles(false);
		_tileMap.Elevatable.ClearPath();
		_focusedTile.Disable();

		var targetTiles = buildAbility.GetTargetPositions(Entities.SelectedEntity!)
			.Select(position => _tileMap.GetTile(position, false))
			.WhereNotNull();
		var wholeMapIsTargeted = buildAbility.WholeMapIsTargeted();
		_tileMap.Elevatable.SetTargetTiles(targetTiles, wholeMapIsTargeted, false, 
			ElevatableTiles.TargetPurpose.Placement);
		var cost = buildAbility.GetSelectableItemCost(entityId);

		var entity = Entities.SetEntityForPlacement(entityId, wholeMapIsTargeted, cost);

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

	internal void OnInterfaceAttackSelected(bool started, AttackType? attackType)
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

	internal void OnInitiativePanelActorHovered(ActorNode? actor) => _hoveredInitiativePanelActor = actor;

	internal void OnInitiativePanelActorSelected(ActorNode? actor) => ExecuteEntitySelection(actor);

	internal void OnInterfaceAbilitySelected(IAbilityNode ability)
	{
		if (ability is IAbilityHasTargetArea abilityWithTargetArea)
		{
			_tileMap.Elevatable.ClearAvailableTiles(false);
			_tileMap.Elevatable.ClearTargetTiles(false);
			_tileMap.Elevatable.ClearPath();
			
			var tiles = abilityWithTargetArea.GetTargetPositions(Entities.SelectedEntity!)
				.Select(position => _tileMap.GetTile(position, false))
				.WhereNotNull();
			var wholeMapIsTargeted = abilityWithTargetArea.WholeMapIsTargeted();
			var targetPurpose = ability is BuildNode
				? ElevatableTiles.TargetPurpose.Placement
				: ElevatableTiles.TargetPurpose.Ability;
			
			_tileMap.Elevatable.SetTargetTiles(tiles, wholeMapIsTargeted, false, targetPurpose);
			_selectionOverlay = SelectionOverlay.Target;
		}

		_selectedAbility = ability;
	}

	internal void OnInterfaceAbilityDeselected() => _selectedAbility = null;
}
