using Godot;
using System;

public class ServerLobby : Lobby
{
    public const string ScenePath = @"res://app/server/lobby/ServerLobby.tscn";
    
    public override void _Ready()  // TODO not tested
    {
        if (Server.Instance.IsHosting()) 
            return;
        
        if (Server.Instance.HostGame() is false)
        {
            GD.Print("Failed to start server, shutting down.");
            GetTree().Quit();
        }

        Client.Instance.Connect(nameof(Client.GameStarted), this, nameof(OnGameStarted));
    }

    private void OnGameStarted() // TODO not tested
    {
        GD.Print("Game starting...");
        //GetTree().ChangeScene(ServerGame.ScenePath);
    }
}
