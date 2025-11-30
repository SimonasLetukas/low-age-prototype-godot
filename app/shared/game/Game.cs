using System.Collections.Generic;
using Godot;
using LowAgeCommon.Extensions;
using Newtonsoft.Json;

/// <summary>
/// Used for changing and synchronizing game state through <see cref="IGameEvent"/>s.
/// </summary>
public partial class Game : Node2D
{
    // In case of out-of-sync, this could be used to get the difference: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
    protected List<IGameEvent> Events { get; set; } = [];

    protected Turns Turns { get; private set; } = null!;
    protected string LogPrefix => $"{CurrentPlayerId}.{GetType().Name}";

    private string CurrentPlayerId => Multiplayer.IsServer() 
        ? "0"
        : Players.Instance.Current.Id.ToString();
    
    public override void _Ready()
    {
        Turns = GetNode<Turns>(nameof(Turns));
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
        
        GD.Print($"{LogPrefix}: calling {nameof(OnClientLoaded)} as Rpc.");
        RpcId(Constants.ENet.ServerId, nameof(OnClientLoaded), Multiplayer.GetUniqueId());
    }

    /// <summary>
    /// Request server to add a new <see cref="IGameEvent"/> for all clients.
    /// </summary>
    /// <param name="gameEvent"></param>
    protected void RegisterNewGameEvent(IGameEvent gameEvent)
    {
        GD.Print($"{LogPrefix}.{nameof(RegisterNewGameEvent)}: called with {gameEvent.GetType()} " +
                 $"and properties '{JsonConvert.SerializeObject(gameEvent).TrimForLogs()}'.");

        if (Multiplayer.IsServer())
            return;
        
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
