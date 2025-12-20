using System;
using Godot;
using LowAgeData.Domain.Common;

public partial class TurnPanel : Control
{
	public event Action NextTurnClicked = delegate { };

	private TurnPhase _currentPhase = TurnPhase.Planning;
	
	private RichTextLabel _turnCounterHundred = null!;
	private RichTextLabel _turnCounterTen = null!;
	private RichTextLabel _turnCounterSingle = null!;
	private RichTextLabel _phaseLabel = null!;
	private BaseButton _nextTurnButton = null!;
	
	public override void _Ready()
	{
		base._Ready();
		
		_turnCounterHundred = GetNode<RichTextLabel>("TurnCounter/HundredDigits");
		_turnCounterTen = GetNode<RichTextLabel>("TurnCounter/TenDigits");
		_turnCounterSingle = GetNode<RichTextLabel>("TurnCounter/SingleDigits");
		_phaseLabel = GetNode<RichTextLabel>("PhaseLabel");
		_nextTurnButton = GetNode<BaseButton>("NextTurnButton");
		
		_nextTurnButton.Clicked += OnNextTurnButtonClicked;
		EventBus.Instance.PhaseStarted += OnPhaseStarted;
		EventBus.Instance.ActionStarted += OnActionStarted;
	}

	private void OnActionStarted(ActorNode actor)
	{
		var currentPlayerCanPressNextTurn = Players.Instance.IsActionAllowedForCurrentPlayerOn(actor);
		_nextTurnButton.SetDisabled(currentPlayerCanPressNextTurn is false);
	}

	public override void _ExitTree()
	{
		_nextTurnButton.Clicked -= OnNextTurnButtonClicked;
		
		base._ExitTree();
	}

	public void OnPhaseStarted(int turn, TurnPhase phase)
	{
		_currentPhase = phase;
		_nextTurnButton.SetDisabled(false);
		
		SetTurnCounter(turn);
		SetPhaseLabel(phase);
	}

	private void SetTurnCounter(int turn)
	{
		var lastThreeDigits = turn % 1000;
		
		var hundreds = (lastThreeDigits / 100) % 10;
		var tens = (lastThreeDigits / 10) % 10;
		var ones = lastThreeDigits % 10;

		_turnCounterHundred.Text = $"[center]{hundreds.ToString()}";
		_turnCounterTen.Text = $"[center]{tens.ToString()}";
		_turnCounterSingle.Text = $"[center]{ones.ToString()}";
	}

	private void SetPhaseLabel(TurnPhase phase) => _phaseLabel.Text = $"{phase.ToDisplayValue().Capitalize()} Phase";

	private void OnNextTurnButtonClicked()
	{
		if (_currentPhase.Equals(TurnPhase.Planning))
			_nextTurnButton.SetDisabled(true);
		
		NextTurnClicked();
	}
}
