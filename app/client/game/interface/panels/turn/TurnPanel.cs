using System;
using Godot;

public partial class TurnPanel : Control
{
	public event Action NextTurnClicked = delegate { };

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
		
		OnNewTurn(1234, Phase.Planning);
	}

	public override void _ExitTree()
	{
		_nextTurnButton.Clicked -= OnNextTurnButtonClicked;
		
		base._ExitTree();
	}

	public void OnNewTurn(int turn, Phase phase)
	{
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

	private void SetPhaseLabel(Phase phase) => _phaseLabel.Text = $"{phase.ToString()} Phase";

	private void OnNextTurnButtonClicked() => NextTurnClicked();
}
