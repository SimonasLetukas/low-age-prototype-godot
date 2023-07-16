using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using Newtonsoft.Json;

/// <summary>
/// Handles game data for players, tiles, units & structures.
/// </summary>
public class Data : Node
{
    public static Data Instance = null;
    
    [Signal] public delegate void Synchronised();
    
    public IList<Player> Players { get; private set; } = new List<Player>();
    public Vector2 MapSize { get; private set; } = Vector2.Zero;
    public IList<Unit> Units { get; private set; } = new List<Unit>();
    public IList<Tile> Tiles { get; private set; } = new List<Tile>();

    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }

    public void Initialize(Vector2 mapSize)
    {
        MapSize = mapSize;
        for (var y = 0; y < mapSize.y; y++)
        {
            for (var x = 0; x < mapSize.x; x++)
            {
                Tiles.Add(new Tile
                {
                    Position = new Vector2(x, y),
                    Terrain = Constants.Game.Terrain.Grass,
                    Unit = null
                });
            }
        }
    }

    public void AddPlayer(int playerId, string playerName, Constants.Game.Faction faction)
    {
        Players.Add(new Player
        {
            Id = playerId,
            Name = playerName,
            Faction = faction
        });
    }
    
    public void RemovePlayer(int id)
    {
        var playerToRemove = Players.SingleOrDefault(x => x.Id.Equals(id));
        if (playerToRemove is null)
        {
            return;
        }
        Players.Remove(playerToRemove);
    }

    public string GetPlayerName(int id) => Players.Single(x => x.Id.Equals(id)).Name;

    public Tile GetTile(Vector2 at) => Tiles.SingleOrDefault(x => x.Position.Equals(at));

    public Constants.Game.Terrain GetTerrain(Vector2 at) => at.IsInBoundsOf(MapSize) 
        ? GetTile(at).Terrain 
        : Constants.Game.Terrain.Mountains;

    public void SetTerrain(Vector2 at, Constants.Game.Terrain terrain)
    {
        if (at.IsInBoundsOf(MapSize))
        {
            GetTile(at).Terrain = terrain;
        }
    }

    public Unit GetUnit(Vector2 at) => Units.SingleOrDefault(x => x.Position.Equals(at));

    public Unit NewUnit(Vector2 at)
    {
        var unit = new Unit
        {
            Position = at
        };
        Units.Add(unit);
        
        GetTile(at).Unit = unit;

        return unit;
    }

    public void MoveUnit(Vector2 from, Vector2 to)
    {
        if (from.IsInBoundsOf(MapSize) is false || to.IsInBoundsOf(MapSize) is false)
            return;

        var unit = GetUnit(from);
        var tileFrom = GetTile(from);
        var tileTo = GetTile(to);

        unit.Position = to;
        tileFrom.Unit = null;
        tileTo.Unit = unit;
    }

    public void Reset()
    {
        Players = new List<Player>();
        MapSize = Vector2.Zero;
        Units = new List<Unit>();
        Tiles = new List<Tile>();
    }

    public void Synchronise()
    {
        GD.Print($"{nameof(Data)}.{nameof(Synchronise)}: sending RPC request to synchronise data.");
        Rpc(nameof(OnSynchroniseRequested),
            JsonConvert.SerializeObject(MapSize),
            JsonConvert.SerializeObject(Tiles));
    }

    [Remote]
    public void OnSynchroniseRequested(string mapSize, string tiles)
    {
        GD.Print($"{nameof(Data)}.{nameof(OnSynchroniseRequested)}: synchronisation request received with " +
                 $"{nameof(mapSize)} '{mapSize}', {nameof(tiles)} '{tiles}'.");
        MapSize = JsonConvert.DeserializeObject<Vector2>(mapSize);
        Tiles = JsonConvert.DeserializeObject<List<Tile>>(tiles);
        EmitSignal(nameof(Synchronised));
    }
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Constants.Game.Faction Faction { get; set; }
}

public class Unit
{
    public Vector2 Position { get; set; }
}

public class Tile
{
    public Vector2 Position { get; set; }
    public Constants.Game.Terrain Terrain { get; set; }
    public Unit Unit { get; set; }
}