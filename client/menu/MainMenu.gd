extends Control

func _ready():
	get_tree().connect('connected_to_server', self, 'on_connected_to_server')
	
	if OS.has_environment("USERNAME"):
		$Name/Input.text = OS.get_environment("USERNAME")
	else:
		var desktop_path = OS.get_system_dir(0).replace("\\", "/").split("/")
		$Name/Input.text = desktop_path[desktop_path.size() - 2]

func on_connected_to_server():
	get_tree().change_scene("res://client/lobby/ClientLobby.tscn")

func _connect_to_server(player_name: String, player_faction: int):
	_put_connection_message("Connecting to server")
	$Connect.disabled = true
	if not Client.join_game(player_name, player_faction):
		_put_connection_message("Failed to connect")
	$Connect.disabled = false

func _put_connection_message(message: String) -> void:
	$Connect/ErrorMessage.text = message
	$Tween.interpolate_property($Connect/ErrorMessage, "self_modulate", Color(1, 1, 1, 1), Color(1, 1, 1, 0), 2, Tween.TRANS_LINEAR, Tween.EASE_OUT)
	$Tween.start()

func _on_Connect_pressed():
	var player_name := $Name/Input.text as String
	var player_faction := $Faction/Input.selected as int
	_connect_to_server(player_name, player_faction)
