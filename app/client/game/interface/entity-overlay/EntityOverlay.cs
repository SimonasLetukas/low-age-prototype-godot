using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeData.Domain.Common;

public partial class EntityOverlay : Control
{
	private Guid _targetInstanceId = Guid.Empty;  
	private Guid _sourceInstanceId = Guid.Empty;
	
	private Text _damageCalculationDisplay = null!;

	public override void _Ready()
	{
		base._Ready();
		
		_damageCalculationDisplay = GetNode<Text>("DamageCalculationDisplay");
		_damageCalculationDisplay.SetFontSize(16);

		EventBus.Instance.EntityTargeted += OnEntityTargeted;
		EventBus.Instance.NewTileFocused += OnNewTileFocused;
	}

	public override void _ExitTree()
	{
		EventBus.Instance.EntityTargeted -= OnEntityTargeted;
		EventBus.Instance.NewTileFocused -= OnNewTileFocused;
		
		base._ExitTree();
	}

	private void SetDamage(int amount, bool isLethal)
	{
		_damageCalculationDisplay.Text = $"[center]-{amount}";
		_damageCalculationDisplay.Modulate = isLethal ? Colors.Red : Colors.White;
	}

	private void SetPosition(EntityNode target)
	{
		var targetPosition = target.GlobalPosition + target.GetTopCenterOffset();
		GlobalPosition = GetCanvasTransform().AffineInverse() * target.GetCanvasTransform() * targetPosition;
	}

	private void OnEntityTargeted(EntityNode target, EntityNode source, AttackType attackType)
	{
		if (target.InstanceId.Equals(_targetInstanceId) && source.InstanceId.Equals(_sourceInstanceId))
			return;

		var canBeTargeted = target.CanBeTargetedBy(source);
		if (canBeTargeted.IsValid is false)
			return;
		
		_targetInstanceId = target.InstanceId;
		_sourceInstanceId = source.InstanceId;

		var (damage, isLethal) = target.ReceiveAttack(source, attackType, true);
		SetDamage(damage, isLethal);
		SetPosition(target);
		Visible = true;
	}
	
	private void OnNewTileFocused(Vector2Int mapPosition, Terrain terrain, IList<EntityNode>? occupants)
	{
		if (occupants is not null && occupants.Any(x => x.InstanceId.Equals(_targetInstanceId))) 
			return;
		
		Visible = false;
		_targetInstanceId = Guid.Empty;
		_sourceInstanceId = Guid.Empty;
	}
}
