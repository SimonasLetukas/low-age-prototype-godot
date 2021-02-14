extends Node

# Setup server and open a new lobby
func _ready():
	get_tree().change_scene("res://server/lobby/ServerLobby.tscn")
