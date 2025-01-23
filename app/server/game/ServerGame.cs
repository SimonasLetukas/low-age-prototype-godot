using Godot;
using System.Collections.Generic;
using System.Linq;
using low_age_prototype_common.Extensions;

public partial class ServerGame : Game
{
    public const string ScenePath = @"res://app/server/game/ServerGame.tscn";

    private List<int> _notLoadedPlayers;
    private List<int> _notInitializedPlayers;
    private Creator _creator;

    public override async void _Ready()
    {
        GD.Print($"{nameof(ServerGame)}: entering");
        
        _creator = GetNode<Creator>($"{nameof(Creator)}");
        
        _creator.MapCreated += OnRegisterServerEvent;
        
        Server.Instance.Connect(nameof(Network.PlayerRemoved), new Callable(this, nameof(OnPlayerRemoved)));

        // Wait until the parent scene is fully loaded
        await ToSignal(GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1), "ready");

        _notLoadedPlayers = new List<int>();
        _notInitializedPlayers = new List<int>();
        foreach (var player in Data.Instance.Players)
        {
            _notLoadedPlayers.Add(player.Id);
            _notInitializedPlayers.Add(player.Id);
        }
    }

    public override void _ExitTree()
    {
        _creator.MapCreated -= OnRegisterServerEvent;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
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
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void OnRegisterNewGameEvent(string eventBody)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterNewGameEvent)}: registering new game event " +
                 $"'{eventBody.TrimForLogs()}'");

        var gameEvent = StringToEvent(eventBody);
        if (Events.Any(x => x.Id.Equals(gameEvent.Id)))
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterNewGameEvent)}: event '{gameEvent.Id}' " +
                     "already exists for server");
            return;
        }
        
        Events.Add(gameEvent);

        Rpc(nameof(OnNewGameEventRegistered), eventBody);
        
        ExecuteGameEvent(gameEvent);
    }

    private void OnRegisterServerEvent(IGameEvent gameEvent)
    {
        GD.Print($"{nameof(ServerGame)}.{nameof(OnRegisterServerEvent)}: called with {gameEvent.GetType()}");
        OnRegisterNewGameEvent(EventToString(gameEvent));
    }
    
    private void ExecuteGameEvent(IGameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case ClientFinishedInitializingEvent clientFinishedInitializingEvent:
                HandleEvent(clientFinishedInitializingEvent);
                break;
            default:
                GD.PrintErr($"{nameof(ServerGame)}.{nameof(ExecuteGameEvent)}: could not execute event " +
                            $"'{EventToString(gameEvent)}'. Type not implemented or not relevant for server.");
                break;
        }
    }

    private void HandleEvent(ClientFinishedInitializingEvent clientFinishedInitializingEvent)
    {
        var playerId = clientFinishedInitializingEvent.PlayerId;
        GD.Print($"{nameof(ServerGame)}.{nameof(ClientFinishedInitializingEvent)}: '{playerId}' client initialized");
        _notInitializedPlayers.Remove(playerId);
        
        if (_notInitializedPlayers.IsEmpty())
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(ClientFinishedInitializingEvent)}: all clients have loaded, " +
                     "notifying all players");
            OnRegisterServerEvent(new InitializationCompletedEvent());
            return;
        }
        
        GD.Print($"{nameof(ServerGame)}.{nameof(ClientFinishedInitializingEvent)}: still waiting for " +
                 $"{_notInitializedPlayers.Count} players to initialize");
    }
    
    private void OnPlayerRemoved(int playerId)
    {
        if (Data.Instance.Players.Count < 2)
        {
            GD.Print($"{nameof(ServerGame)}.{nameof(OnPlayerRemoved)}: not enough players to run the " +
                     "game, returning to lobby");
            
            // Tell everyone that the game has ended
            Rpc(nameof(GameEnded));
            Callable.From(() => GetTree().ChangeSceneToFile(ServerLobby.ScenePath)).CallDeferred();
        }
        
        GD.Print($"{nameof(ServerGame)}.{nameof(OnPlayerRemoved)}: '{Data.Instance.Players.Count}' players remaining");
    }
}
