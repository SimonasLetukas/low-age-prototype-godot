using System;
using System.Collections.Generic;
using Godot;
using low_age_data.Domain.Shared;

/// <summary>
/// <para>
/// Map creator encapsulates work done with .png files and any other map generation.
/// </para>
/// <para>
/// Map creator accepts a .png containing a map, list of map properties and a map with all tilesets to be modified.
/// The output of this map generation is an event once it is created.
/// </para>
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
    
    public event Action<MapCreatedEvent> MapCreated = delegate { };
    
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

        var mapSize = new Vector2(image.GetWidth(), image.GetHeight());
        var startingPositions = new List<Vector2>();
        var tiles = new List<(Vector2, Terrain)>();
        
        image.Lock();

        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                var pixel = image.GetPixel(x, y);
                
                // By default all (unknown & starting) tiles are grass
                var terrain = Terrain.Grass;
                
                if (pixel == ColorGrass) terrain = Terrain.Grass;
                if (pixel == ColorMountains) terrain = Terrain.Mountains;
                if (pixel == ColorMarsh) terrain = Terrain.Marsh;
                if (pixel == ColorScraps) terrain = Terrain.Scraps;
                if (pixel == ColorCelestium) terrain = Terrain.Celestium;
                if (pixel == ColorStart) startingPositions.Add(new Vector2(x, y));
                
                tiles.Add((new Vector2(x, y), terrain));
            }
        }
        
        image.Unlock();
        MapCreated(new MapCreatedEvent(mapSize, startingPositions, tiles));
    }
}
