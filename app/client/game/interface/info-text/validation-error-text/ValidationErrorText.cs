using Godot;
using System;

public partial class ValidationErrorText : Control
{
	public const string ScenePath = @"res://app/client/game/interface/info-text/validation-error-text/ValidationErrorText.tscn";
	public static ValidationErrorText Instance() => (ValidationErrorText) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static ValidationErrorText InstantiateAsChild(string message, Node parentNode)
	{
		var validationErrorText = Instance();
		parentNode.AddChild(validationErrorText);
		validationErrorText.Start(message);
		return validationErrorText;
	}
	
	private const int FloatDistance = 2;
	private const float DurationSeconds = 2;

	public void Start(string message)
	{
		GetNode<RichTextLabel>(nameof(RichTextLabel)).Text = message;
		
		var tween = CreateTween();
		tween.SetParallel();

		tween.TweenProperty(this, "position", Position + Vector2.Up * FloatDistance, DurationSeconds)
			.FromCurrent()
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);

		tween.TweenProperty(this, "modulate:a", 0.0, DurationSeconds)
			.FromCurrent();

		tween.Finished += QueueFree;
	}
}
