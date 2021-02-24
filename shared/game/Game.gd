extends Node2D

func _ready():
	print("Game: entering")


# Calls to the server

func _mark_as_loaded() -> void:
	if not get_tree().is_network_server():
		# Report that this client is done loading
		rpc_id(Constants.server_id, "on_client_loaded", get_tree().get_network_unique_id())

func _send_new_unit_position(entity_position: Vector2, global_path: PoolVector2Array, path: PoolVector2Array) -> void:
	if not get_tree().is_network_server():
		# Request server to update unit position
		rpc_id(Constants.server_id, "on_new_unit_position", 
			get_tree().get_network_unique_id(), entity_position, global_path, path)

# Callbacks from the server

remotesync func _game_ended():
	pass

remotesync func _on_map_size_declared(map_size: Vector2):
	pass

remotesync func _on_map_generation_ended():
	pass

remotesync func _on_celestium_found(coordinates: Vector2):
	pass

remotesync func _on_grass_found(coordinates: Vector2):
	pass

remotesync func _on_marsh_found(coordinates: Vector2):
	pass

remotesync func _on_mountains_found(coordinates: Vector2):
	pass

remotesync func _on_scraps_found(coordinates: Vector2):
	pass

remotesync func _on_starting_position_found(coordinates: Vector2):
	pass

remotesync func _on_unit_position_updated(entity_position: Vector2, global_path: PoolVector2Array, path: PoolVector2Array):
	pass
