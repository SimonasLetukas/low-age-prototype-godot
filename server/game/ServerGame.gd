extends "res://shared/game/Game.gd"

# ask to generate when everyone is loaded
var not_loaded_players := {}

onready var creator: Creator = $Creator

func _ready():
	print("ServerGame: entering")
	
	yield(get_tree().root.get_child(get_tree().root.get_child_count()-1), "ready")
	_handle_server_dependency_injection()
	
	for player_id in Data.players:
		not_loaded_players[player_id] = player_id

func _handle_server_dependency_injection():
	pass

remote func on_client_loaded(player_id: int) -> void:
	print("Client loaded: %s" % player_id as String)
	not_loaded_players.erase(player_id)
	if not_loaded_players.empty():
		print("Starting the map generation")
		creator.generate()
		return
	
	print("Still waiting for %d players" % not_loaded_players.size())


func _on_Creator_map_size_declared(map_size):
	rpc("_on_map_size_declared", map_size)

func _on_Creator_generation_ended():
	rpc("_on_map_generation_ended")

func _on_Creator_celestium_found(coordinates):
	rpc("_on_celestium_found", coordinates)

func _on_Creator_grass_found(coordinates):
	rpc("_on_grass_found", coordinates)

func _on_Creator_marsh_found(coordinates):
	rpc("_on_marsh_found", coordinates)

func _on_Creator_mountains_found(coordinates):
	rpc("_on_mountains_found", coordinates)

func _on_Creator_scraps_found(coordinates):
	rpc("_on_scraps_found", coordinates)

func _on_Creator_starting_position_found(coordinates):
	rpc("_on_starting_position_found", coordinates)
