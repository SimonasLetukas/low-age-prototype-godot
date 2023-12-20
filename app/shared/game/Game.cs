using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

/// <summary>
/// Used for changing and synchronizing game state through <see cref="IGameEvent"/>s.
/// </summary>
public class Game : Node2D
{
    // In case of out-of-sync, this could be used to get the difference: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
    protected List<IGameEvent> Events { get; set; } = new List<IGameEvent>();
    
    public override void _Ready()
    {
        GD.Print("Game: entering");
    }

    #region Calls to the server

    /// <summary>
    /// Report that this client has done loading
    /// </summary>
    protected void MarkAsLoaded()
    {
        GD.Print($"{nameof(Game)}.{nameof(MarkAsLoaded)}");
        
        if (GetTree().IsNetworkServer()) 
            return;
        
        GD.Print($"{nameof(Game)}: calling {nameof(ServerGame.OnClientLoaded)} as RPC.");
        RpcId(Constants.ServerId, nameof(ServerGame.OnClientLoaded), GetTree().GetNetworkUniqueId());
    }

    /// <summary>
    /// Request server to add a new <see cref="IGameEvent"/> for all clients.
    /// </summary>
    /// <param name="gameEvent"></param>
    protected void RegisterNewGameEvent(IGameEvent gameEvent)
    {
        GD.Print($"{nameof(Game)}.{nameof(RegisterNewGameEvent)}: called with {gameEvent.GetType()} " +
                 $"and properties '{JsonConvert.SerializeObject(gameEvent)}'.");

        if (GetTree().IsNetworkServer())
            return;
        
        RpcId(Constants.ServerId, nameof(ServerGame.OnRegisterNewGameEvent), GetTree().GetNetworkUniqueId(), 
            EventToString(gameEvent));
    }

    #endregion

    #region Callbacks from the server

    [RemoteSync]
    protected virtual void GameEnded()
    {
    }
    
    [RemoteSync]
    protected virtual void OnNewGameEventRegistered(string eventBody)
    {
        GD.Print($"{nameof(Game)}.{nameof(OnNewGameEventRegistered)}");
    }

    #endregion

    protected static string EventToString(IGameEvent gameEvent) => JsonConvert.SerializeObject(gameEvent, 
        typeof(IGameEvent), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
    
    protected static IGameEvent StringToEvent(string gameEvent) => JsonConvert.DeserializeObject<IGameEvent>(gameEvent, 
        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
}
