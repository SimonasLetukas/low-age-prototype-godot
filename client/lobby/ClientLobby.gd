extends "res://shared/lobby/Lobby.gd"

func _ready():
	Client.connect("game_started", self, "_on_game_started")
	
	# Tell the server about you
	Server.register_self(
		get_tree().get_network_unique_id(), 
		Client.local_player_name, 
		Client.local_player_faction)

func _on_game_started():
	get_tree().change_scene("res://client/game/ClientGame.tscn")

func _on_StartGame_pressed():
	Client.start_game()
