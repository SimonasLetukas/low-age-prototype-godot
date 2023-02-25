using Godot;
using System;

public class ServerLobby : Lobby
{
    public const string ScenePath = @"res://app/server/lobby/ServerLobby.tscn";
    
    public override void _Ready()
    {
        GD.Print($"{nameof(ServerLobby)}: {nameof(_Ready)}");
    }
    
    // TODO
    /*func _ready():
        if not Server.is_hosting():
        if not Server.host_game():
    print("Failed to start server, shutting down.")
    get_tree().quit()
        return
	
    Client.connect("game_started", self, "_on_game_started")

    func _on_game_started():
    get_tree().change_scene("res://server/game/ServerGame.tscn")*/
}
