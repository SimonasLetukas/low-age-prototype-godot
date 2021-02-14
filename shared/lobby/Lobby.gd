extends Control

func _ready():
	Client.connect("player_added", self, "_on_player_added")
	Client.connect("player_removed", self, "_on_player_removed")

func _on_player_added(player_id: int):
	print("Creating player in lobby")
	var player_info_scene = preload("res://shared/lobby/players/Player.tscn")
	
	var player_info_node = player_info_scene.instance()
	player_info_node.set_network_master(player_id)
	player_info_node.set_name(str(player_id))
	
	var player = Data.players[player_id]
	player_info_node.get_node("Name").text = player.name
	
	$Players.add_child(player_info_node)

func _on_player_removed(player_id: int):
	var name = str(player_id)
	for child in $Players.get_children():
		if child.name == name:
			print("Player removed")
			$Players.remove_child(child)
			break
