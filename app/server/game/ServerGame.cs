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
        
        _creator.MapCreated += OnRegisterServerEvent;
        
        Server.Instance.Connect(nameof(Network.PlayerRemoved), this, nameof(OnPlayerRemoved));

        // Wait until the parent scene is fully loaded
        await ToSignal(GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1), "ready");

        _notLoadedPlayers = new List<int>();
        foreach (var player in Data.Instance.Players)
        {
            _notLoadedPlayers.Add(player.Id);
        }
    }

    public override void _ExitTree()
    {
        _creator.MapCreated -= OnRegisterServerEvent;
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
    public void OnRegisterNewGameEvent(string eventBody)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterNewGameEvent)}: registering new game event " +
                 $"'{eventBody.TrimForLogs()}'");

        var gameEvent = StringToEvent(eventBody);
        Events.Add(gameEvent);

        Rpc(nameof(OnNewGameEventRegistered), eventBody);
    }

    private void OnRegisterServerEvent(IGameEvent gameEvent)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterServerEvent)}: called with {gameEvent.GetType()}");
        OnRegisterNewGameEvent(EventToString(gameEvent));
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
}
