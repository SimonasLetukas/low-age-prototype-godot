using Godot;
using System.Collections.Generic;

public class ServerGame : Game
{
    public const string ScenePath = @"res://app/server/game/ServerGame.tscn";

    private List<int> _notLoadedPlayers;
    private Creator _creator;

    public override async void _Ready()
    {
        GD.Print($"{nameof(ServerGame)}: entering");
        
        _creator = GetNode<Creator>($"{nameof(Creator)}");

        _creator.Connect(nameof(Creator.MapSizeDeclared), this, nameof(OnCreatorMapSizeDeclared));
        _creator.Connect(nameof(Creator.GrassFound), this, nameof(OnCreatorGrassFound));
        _creator.Connect(nameof(Creator.MountainsFound), this, nameof(OnCreatorMountainsFound));
        _creator.Connect(nameof(Creator.MarshFound), this, nameof(OnCreatorMarshFound));
        _creator.Connect(nameof(Creator.ScrapsFound), this, nameof(OnCreatorScrapsFound));
        _creator.Connect(nameof(Creator.CelestiumFound), this, nameof(OnCreatorCelestiumFound));
        _creator.Connect(nameof(Creator.StartingPositionFound), this, nameof(OnCreatorStartingPositionFound));
        _creator.Connect(nameof(Creator.GenerationEnded), this, nameof(OnCreatorGenerationEnded));
        
        Server.Instance.Connect(nameof(Network.PlayerRemoved), this, nameof(OnPlayerRemoved));

        // Wait until the parent scene is fully loaded
        await ToSignal(GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1), "ready");

        _notLoadedPlayers = new List<int>();
        foreach (var player in Data.Instance.Players)
        {
            _notLoadedPlayers.Add(player.Id);
        }
    }

    [Remote]
    public void OnClientLoaded(int playerId)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnClientLoaded)}: '{playerId}' client loaded");
        _notLoadedPlayers.Remove(playerId);
        
        if (_notLoadedPlayers.IsEmpty())
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(OnClientLoaded)}: all clients have loaded, " +
                     "starting map generation");
            _creator.Generate();
            return;
        }
        
        GD.Print($"{nameof(ServerGame)}.{nameof(OnClientLoaded)}: still waiting for " +
                 $"{_notLoadedPlayers.Count} players to load");
    }

    [Remote]
    public void OnNewUnitPosition(int playerId, Vector2 entityPosition, Vector2[] globalPath, Vector2[] path)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnClientLoaded)}: player {playerId} " +
                 $"'{Data.Instance.GetPlayerName(playerId)}' moved unit from {path[0]} to {path[path.Length - 1]}");

        Rpc(nameof(OnUnitPositionUpdated), entityPosition, globalPath, path);
    }

    private void OnPlayerRemoved(int playerId)
    {
        if (Data.Instance.Players.Count < 2)
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(OnPlayerRemoved)}: not enough players to run the " +
                     "game, returning to lobby");
            
            // Tell everyone that the game has ended
            Rpc(nameof(GameEnded));
            GetTree().ChangeScene(ServerLobby.ScenePath);
        }
        
        GD.Print($"{nameof(ServerGame)}.{nameof(OnPlayerRemoved)}: '{Data.Instance.Players.Count}' players remaining");
    }

    private void OnCreatorMapSizeDeclared(Vector2 mapSize)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnCreatorMapSizeDeclared)}: initializing map with size '{mapSize}'");
        Data.Instance.Initialize(mapSize);
    }

    private void OnCreatorGenerationEnded()
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnCreatorGenerationEnded)}: generation ended, requesting " +
                 $"synchronisation with clients");

        Data.Instance.Synchronise();
    }

    private void OnCreatorCelestiumFound(Vector2 coordinates)
    {
        Data.Instance.SetTerrain(coordinates, Constants.Game.Terrain.Celestium);
    }
    
    private void OnCreatorScrapsFound(Vector2 coordinates)
    {
        Data.Instance.SetTerrain(coordinates, Constants.Game.Terrain.Scraps);
    }
    
    private void OnCreatorGrassFound(Vector2 coordinates)
    {
        Data.Instance.SetTerrain(coordinates, Constants.Game.Terrain.Grass);
    }
    
    private void OnCreatorMarshFound(Vector2 coordinates)
    {
        Data.Instance.SetTerrain(coordinates, Constants.Game.Terrain.Marsh);
    }
    
    private void OnCreatorMountainsFound(Vector2 coordinates)
    {
        Data.Instance.SetTerrain(coordinates, Constants.Game.Terrain.Mountains);
    }
    
    private void OnCreatorStartingPositionFound(Vector2 coordinates)
    {
        return; // TODO left for later implementation
    }
}
