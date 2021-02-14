extends "res://shared/game/Game.gd"

# ask to generate when everyone is loaded

func _ready():
	yield(get_tree().root, "ready")
	_handle_server_dependency_injection()

func _handle_server_dependency_injection():
	pass
