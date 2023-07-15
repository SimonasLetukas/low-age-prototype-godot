using Godot;

/// <summary>
/// Used for changing and synchronizing game state 
/// </summary>
public class Game : Node2D
{
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
        if (GetTree().IsNetworkServer()) 
            return;
        
        RpcId(Constants.ServerId, nameof(ServerGame.OnClientLoaded), GetTree().GetNetworkUniqueId());
    }

    /// <summary>
    /// Request server to update unit position
    /// </summary>
    /// <param name="entityPosition"></param>
    /// <param name="globalPath"></param>
    /// <param name="path"></param>
    public void SendNewUnitPosition(Vector2 entityPosition, Vector2[] globalPath, Vector2[] path)
    {
        if (GetTree().IsNetworkServer()) 
            return;
        
        RpcId(Constants.ServerId, nameof(ServerGame.OnNewUnitPosition), GetTree().GetNetworkUniqueId(),
            entityPosition, globalPath, path);
    }

    #endregion

    #region Callbacks from the server

    [RemoteSync]
    protected virtual void GameEnded()
    {
    }

    [RemoteSync]
    protected virtual void OnUnitPositionUpdated(Vector2 entityPosition, Vector2[] globalPath, Vector2[] path)
    {
    }

    #endregion
}
