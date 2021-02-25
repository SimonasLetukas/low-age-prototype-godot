extends "res://shared/game/Game.gd"

# ask to generate when everyone is loaded
var not_loaded_players := {}

onready var creator: Creator = $Creator

func _ready():
	print("ServerGame: entering")
	
	Server.connect("player_removed", self, "_on_player_removed")
	
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

remote func on_new_unit_position(player_id: int, entity_position: Vector2, global_path: PoolVector2Array, path: PoolVector2Array) -> void:
	print("Player %s (%s) moved unit from %s to %s" % [
		Data.get_player_name(player_id) as String, 
		player_id as String, 
		path[0] as String, 
		path[path.size() - 1] as String])
	
	rpc("_on_unit_position_updated", entity_position, global_path, path)

func _on_player_removed(player_id: int):
	if Data.players.size() < 2:
		print("Not enough players to run the game, returning to lobby")
		# Tell everyone that game ended
		rpc("_game_ended")
		get_tree().change_scene("res://server/lobby/ServerLobby.tscn")
	
	print("Players remaining: %d" % Data.players.size())

func _on_Creator_map_size_declared(map_size):
	print("Sending map size %s to clients." % map_size as String)
	rpc("_on_map_size_declared", map_size)

func _on_Creator_generation_ended():
	print("Server: generation ended")
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
	print("Sending starting position %s to clients." % coordinates as String)
	rpc("_on_starting_position_found", coordinates)
