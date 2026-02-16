using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using LowAgeCommon.Extensions;
using Newtonsoft.Json;

/// <summary>
/// Used for changing and synchronizing game state through <see cref="IGameEvent"/>s.
/// </summary>
public partial class Game : Node2D
{
    protected event Action SaveLoadingFinished = delegate { };
    
    // In case of out-of-sync, this could be used to get the difference: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
    protected List<IGameEvent> Events { get; set; } = [];
    protected Guid GameId { get; set; }
    protected string MapLocation { get; set; } = "";
    protected bool LoadingSavedGame { get; set; } = false;

    protected Turns Turns { get; private set; } = null!;
    protected string LogPrefix => $"{CurrentPlayerStableId}.{GetType().Name}";

    private string CurrentPlayerStableId => Multiplayer.IsServer() 
        ? "S"
        : Players.Instance.Current.StableId.ToString();

    private List<IGameEvent> _eventsToLoad = [];
    private readonly Stopwatch _stopwatch = new();
    
    public override void _Ready()
    {
        Turns = GetNode<Turns>(nameof(Turns));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (LoadingSavedGame is false)
            return;

        IterateLoadingSavedGame(delta);
    }

    protected virtual void ExecuteGameEvent(IGameEvent gameEvent) { }

    protected void LoadGame(Save save)
    {
        _eventsToLoad = save.Events.Select(StringToEvent).ToList();
        LoadingSavedGame = true;
    }

    private void IterateLoadingSavedGame(double deltaTime)
    {
        var deltaMs = (int)(deltaTime * 1000);
        _stopwatch.Reset();
        _stopwatch.Start();

        while (_stopwatch.ElapsedMilliseconds < deltaMs && _eventsToLoad.Count != 0)
        {
            var eventToExecute = _eventsToLoad.First();
            ExecuteGameEvent(eventToExecute);
            _eventsToLoad.Remove(eventToExecute);
        }

        if (_eventsToLoad.Count == 0)
        {
            LoadingSavedGame = false;
            SaveLoadingFinished();
        }
    }

    #region Calls to the server

    /// <summary>
    /// Report that this client has done loading
    /// </summary>
    protected void MarkAsLoaded()
    {
        GD.Print($"{LogPrefix}.{nameof(MarkAsLoaded)}");
        
        if (Multiplayer.IsServer()) 
            return;

        if (LoadingSavedGame)
            return;
        
        GD.Print($"{LogPrefix}: calling {nameof(OnClientLoaded)} as Rpc.");
        RpcId(Constants.ENet.ServerId, nameof(OnClientLoaded), Multiplayer.GetUniqueId());
    }

    /// <summary>
    /// Request server to add a new <see cref="IGameEvent"/> for all clients.
    /// </summary>
    /// <param name="gameEvent"></param>
    protected void RegisterNewGameEvent(IGameEvent gameEvent)
    {
        if (LoadingSavedGame)
            return;

        if (Multiplayer.IsServer())
            return;
        
        GD.Print($"{LogPrefix}.{nameof(RegisterNewGameEvent)}: called with {gameEvent.GetType()} " +
                 $"and properties '{JsonConvert.SerializeObject(gameEvent).TrimForLogs()}'.");
        
        RpcId(Constants.ENet.ServerId, nameof(OnRegisterNewGameEvent), EventToString(gameEvent));
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected virtual void OnClientLoaded(int playerId)
    {
        GD.Print($"{LogPrefix}.{nameof(OnClientLoaded)}: '{playerId}' client loaded");
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    protected virtual void OnRegisterNewGameEvent(string eventBody)
    {
        GD.Print($"{LogPrefix}.{nameof(OnRegisterNewGameEvent)}: registering new game event " +
                 $"'{eventBody.TrimForLogs()}'");
    }

    #endregion

    #region Callbacks from the server

    protected void SaveGameLoadedByAllPeers()
    {
        GD.Print($"{LogPrefix}.{nameof(SaveGameLoadedByAllPeers)}");
        Rpc(nameof(OnSaveGameLoadedByAllPeers));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    protected virtual void OnSaveGameLoadedByAllPeers()
    {
        GD.Print($"{LogPrefix}.{nameof(OnSaveGameLoadedByAllPeers)}");
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected virtual void GameEnded()
    {
        GD.Print($"{LogPrefix}.{nameof(GameEnded)}");
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    protected virtual void OnNewGameEventRegistered(string eventBody)
    {
        GD.Print($"{LogPrefix}.{nameof(OnNewGameEventRegistered)}");
    }

    #endregion

    protected static string EventToString(IGameEvent gameEvent) => JsonConvert.SerializeObject(gameEvent, 
        typeof(IGameEvent), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
    
    protected static IGameEvent StringToEvent(string gameEvent) => JsonConvert.DeserializeObject<IGameEvent>(gameEvent, 
        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
}
