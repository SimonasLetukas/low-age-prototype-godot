extends Node2D

func _ready():
	print("Game: entering")

func _mark_as_loaded() -> void:
	if not get_tree().is_network_server():
		# Report that this client is done loading
		rpc_id(Constants.server_id, "on_client_loaded", get_tree().get_network_unique_id())

remotesync func _on_map_size_declared(map_size: Vector2):
	#map.on_MapCreator_map_size_declared(map_size)
	pass

remotesync func _on_map_generation_ended():
	#map.on_MapCreator_generation_ended()
	pass

remotesync func _on_celestium_found(coordinates: Vector2):
	#map.on_MapCreator_celestium_found(coordinates)
	pass

remotesync func _on_grass_found(coordinates: Vector2):
	#map.on_MapCreator_grass_found(coordinates)
	pass

remotesync func _on_marsh_found(coordinates: Vector2):
	#map.on_MapCreator_marsh_found(coordinates)
	pass

remotesync func _on_mountains_found(coordinates: Vector2):
	#map.on_MapCreator_mountains_found(coordinates)
	pass

remotesync func _on_scraps_found(coordinates: Vector2):
	#map.on_MapCreator_scraps_found(coordinates)
	pass

remotesync func _on_starting_position_found(coordinates: Vector2):
	#map.on_MapCreator_starting_position_found(coordinates)
	pass
