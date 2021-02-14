extends "res://shared/lobby/Lobby.gd"

func _ready():
	if not Server.is_hosting():
		if not Server.host_game():
			print("Failed to start server, shutting down.")
			get_tree().quit()
			return
	
	Client.connect("game_started", self, "_on_game_started")

func _on_game_started():
	get_tree().change_scene("res://server/game/ServerGame.tscn")
