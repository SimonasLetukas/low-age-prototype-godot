using Godot;
using System;

/// <summary>
/// Map creator encapsulates work done with .png files and any
/// other map generation.
/// 
/// Map creator accepts a .png containing a map, list of map
/// properties and a map with all tilesets to be modified.
/// The output of this generation should be various signals:
/// Map size declared, starting positions, copies of tilesets
/// with tiles generated.
/// </summary>
public class Creator : Node2D
{
    [Export] public bool DebugEnabled { get; set; } = true;
    [Export(PropertyHint.File, "*.png")] public string MapFileLocation { get; set; }
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorGrass { get; set; } = new Color(Colors.White);
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorMountains { get; set; } = new Color(Colors.Black);
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorMarsh { get; set; } = new Color(Colors.Magenta);
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorScraps { get; set; } = new Color(Colors.Red);
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorCelestium { get; set; } = new Color(Colors.Green);
    [Export(PropertyHint.ColorNoAlpha)] public Color ColorStart { get; set; } = new Color(Colors.Blue);
    
    [Signal] public delegate void MapSizeDeclared(Vector2 mapSize);
    [Signal] public delegate void GrassFound(Vector2 coordinates);
    [Signal] public delegate void MountainsFound(Vector2 coordinates);
    [Signal] public delegate void MarshFound(Vector2 coordinates);
    [Signal] public delegate void ScrapsFound(Vector2 coordinates);
    [Signal] public delegate void CelestiumFound(Vector2 coordinates);
    [Signal] public delegate void StartingPositionFound(Vector2 coordinates);
    [Signal] public delegate void GenerationEnded();

    private Vector2 _mapSize;

    public override void _Ready()
    {
        if (DebugEnabled)
        {
            GD.Print($"{nameof(Creator)}.{nameof(MapFileLocation)}: {MapFileLocation}");
            GD.Print($"{nameof(Creator)}.{nameof(ColorGrass)}: {ColorGrass}");
            GD.Print($"{nameof(Creator)}.{nameof(ColorMountains)}: {ColorMountains}");
            GD.Print($"{nameof(Creator)}.{nameof(ColorMarsh)}: {ColorMarsh}");
            GD.Print($"{nameof(Creator)}.{nameof(ColorScraps)}: {ColorScraps}");
            GD.Print($"{nameof(Creator)}.{nameof(ColorCelestium)}: {ColorCelestium}");
            GD.Print($"{nameof(Creator)}.{nameof(ColorStart)}: {ColorStart}");
        }
    }

    public void Generate()
    {
        var image = GD.Load<Image>(MapFileLocation);
        if (image.IsInvisible() || image.IsEmpty())
        {
            if (DebugEnabled) GD.Print($"{nameof(Creator)}.{nameof(Generate)}: loaded image is not available");
            return;
        }

        _mapSize = new Vector2(image.GetWidth(), image.GetHeight());
        EmitSignal(nameof(MapSizeDeclared), _mapSize);
        if (DebugEnabled) GD.Print($"{nameof(Creator)}.{nameof(Generate)}: signal " +
                                   $"{nameof(MapSizeDeclared)} emitted with {_mapSize}");
        
        image.Lock();

        for (var y = 0; y < _mapSize.y; y++)
        {
            for (var x = 0; x < _mapSize.x; x++)
            {
                var pixel = image.GetPixel(x, y);
                
                // By default all tiles are grass
                EmitSignal(nameof(GrassFound), new Vector2(x, y));
                
                if (pixel == ColorGrass) EmitSignal(nameof(GrassFound), new Vector2(x, y));
                if (pixel == ColorMountains) EmitSignal(nameof(MountainsFound), new Vector2(x, y));
                if (pixel == ColorMarsh) EmitSignal(nameof(MarshFound), new Vector2(x, y));
                if (pixel == ColorScraps) EmitSignal(nameof(ScrapsFound), new Vector2(x, y));
                if (pixel == ColorCelestium) EmitSignal(nameof(CelestiumFound), new Vector2(x, y));
                if (pixel == ColorStart) EmitSignal(nameof(StartingPositionFound), new Vector2(x, y));
            }
        }
        
        image.Unlock();
        EmitSignal(nameof(GenerationEnded));
    }
}
