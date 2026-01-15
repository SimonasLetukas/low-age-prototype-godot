using System;
using Godot;

public partial class ParabolicArrow : Node2D
{
	private const string ScenePath = @"res://app/client/game/map/ParabolicArrow.tscn";
	public static ParabolicArrow Instance() => (ParabolicArrow) GD.Load<PackedScene>(ScenePath).Instantiate();
	public static ParabolicArrow InstantiateAsChild(Node parentNode, Vector2 start, Vector2 end)
	{
		var arrow = Instance();
		arrow.Start = start;
		arrow.End = end;
		parentNode.AddChild(arrow);
		arrow.ZIndex = 4095;
        arrow.Redraw();
		return arrow;
	}
	
	public Vector2 Start;
	public Vector2 End;
	
	[Export] public int Segments = 128;
	[Export] public float ArcHeight = 30f;
	[Export] public Color Color = new("e0d1bf");
	[Export] public float PixelSize = 2f;
	[Export] public float ArrowHeadSize = 4f;
	
	public override void _Draw()
	{
		DrawArrow();
	}

	public void Redraw()
	{
		QueueRedraw();
	}
	
	private void DrawArrow()
	{
		var totalLength = ComputeCurveLength();
		var cutoffLength = totalLength - (ArrowHeadSize / 4);

		var traveled = 0f;
		var prev = GetParabolaPoint(0f);

		for (var i = 1; i <= Segments; i++)
		{
			var t = i / (float)Segments;
			var current = GetParabolaPoint(t);

			var segmentLength = prev.DistanceTo(current);

			if (traveled + segmentLength >= cutoffLength)
				break;

			DrawLine(prev, current, Color, PixelSize, false);

			traveled += segmentLength;
			prev = current;
		}

		DrawArrowHead();
	}

	private void DrawArrowHead()
	{
		var tip = GetParabolaPoint(1f);
		var beforeTip = GetParabolaPoint(0.95f);

		var dir = (tip - beforeTip).Normalized();
		var left = dir.Rotated(Mathf.Pi * 0.75f);
		var right = dir.Rotated(-Mathf.Pi * 0.75f);

		DrawPolygon(
			[
				tip,
				tip + left * ArrowHeadSize,
				tip + right * ArrowHeadSize
			],
			[Color]
		);
	}
	
	private float ComputeCurveLength()
	{
		var length = 0f;
		var prev = GetParabolaPoint(0f);

		for (var i = 1; i <= Segments; i++)
		{
			var t = i / (float)Segments;
			var current = GetParabolaPoint(t);
			length += prev.DistanceTo(current);
			prev = current;
		}

		return length;
	}
	
	private Vector2 GetParabolaPoint(float t)
	{
		var mid = (Start + End) * 0.5f;
		mid.Y -= ArcHeight;

		var a = Start.Lerp(mid, t);
		var b = mid.Lerp(End, t);
		return a.Lerp(b, t);
	}
}
