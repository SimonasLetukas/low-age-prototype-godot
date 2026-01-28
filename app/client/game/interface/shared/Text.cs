using Godot;

public partial class Text : RichTextLabel
{
	public const string ScenePath = @"res://app/client/game/interface/shared/Text.tscn";
	public static Text Instance() => (Text) GD.Load<PackedScene>(ScenePath).Instantiate();
	
	[Export]
	public bool IsBrighter { get; set; }
	
	[Export]
	public bool HasOutline { get; set; }
	
	[Export]
	public bool IsBlue { get; set; }

	private Color _defaultFontColor = new("e0d1bf");
	private Color _brighterFontColor = new("f0e9e0");
	private Color _blueFontColor = new("c1d7df");
	private int _outlineSize = 3;
	
	private const int DefaultSize = 15;

	public override void _Ready()
	{
		base._Ready();
		
		AddThemeColorOverride("default_color", _defaultFontColor);
		AddThemeConstantOverride("outline_size", 0);

		if (IsBrighter) 
			AddThemeColorOverride("default_color", _brighterFontColor);
		
		if (IsBlue)
			AddThemeColorOverride("default_color", _blueFontColor);
		
		if (HasOutline)
			AddThemeConstantOverride("outline_size", _outlineSize);
	}

	public void SetFontSize(int size = DefaultSize)
	{
		AddThemeFontSizeOverride("normal_font_size", size);
	}
}
