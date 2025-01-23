using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Tiles;
using low_age_prototype_common;
using Area = low_age_prototype_common.Area;

/// <summary>
/// <para>
/// Map creator encapsulates work done with .png files and any other map generation.
/// </para>
/// <para>
/// Map creator accepts a .png containing a map, list of map properties and a map with all tilesets to be modified.
/// The output of this map generation is an event once it is created.
/// </para>
/// </summary>
public partial class Creator : Node2D
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

        var mapSize = new Vector2<int>(image.GetWidth(), image.GetHeight());
        var startingPositions = new List<Vector2<int>>();
        var tiles = new List<(Vector2<int>, TileId)>();
        
        for (var y = 0; y < mapSize.Y; y++)
        {
            for (var x = 0; x < mapSize.X; x++)
            {
                var pixel = image.GetPixel(x, y);
                
                // By default, all (unknown & starting) tiles are grass
                var tile = TileId.Grass;
                
                if (pixel == ColorGrass) tile = TileId.Grass;
                if (pixel == ColorMountains) tile = TileId.Mountains;
                if (pixel == ColorMarsh) tile = TileId.Marsh;
                if (pixel == ColorScraps) tile = TileId.Scraps;
                if (pixel == ColorCelestium) tile = TileId.Celestium;
                if (pixel == ColorStart) startingPositions.Add(new Vector2<int>(x, y));
                
                tiles.Add((new Vector2<int>(x, y), tile));
            }
        }
        
        MapCreated(new MapCreatedEvent(mapSize, AssignStartingPositions(startingPositions.ToSquareRects()), tiles));
    }

    private static Dictionary<int, IList<Area>> AssignStartingPositions(IList<Area> positions)
    {
        var assigned = new Dictionary<int, IList<Area>>();
        foreach (var player in Data.Instance.Players)
        {
            assigned[player.Id] = new List<Area> { positions.First() };
            positions.RemoveAt(0);
        }

        return assigned;
    }
}
