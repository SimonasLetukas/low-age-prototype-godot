extends "res://shared/game/Game.gd"

onready var map: ClientMap = $Map
onready var camera: Camera2D = $Camera
onready var mouse: Mouse = $Mouse
onready var interface: Interface = $Interface

func _ready():
	print("ClientGame: entering")
	get_tree().paused = true
	
	yield(get_tree().root.get_child(get_tree().root.get_child_count()-1), "ready")
	_handle_client_dependency_injection()
	
	#_initialize_fake_map()
	_mark_as_loaded()

func _handle_client_dependency_injection() -> void:
	mouse.connect("left_released_without_drag", map, "_on_MouseController_left_released_without_drag")
	mouse.connect("right_released_without_examine", map, "_on_MouseController_right_released_without_examine")
	
	mouse.connect("mouse_dragged", camera, "_on_MouseController_mouse_dragged")
	mouse.connect("taking_control", camera, "_on_MouseController_taking_control")
	
	interface.connect("mouse_entered", mouse, "_on_Interface_mouse_entered")
	interface.connect("mouse_exited", mouse, "_on_Interface_mouse_exited")
	
	map.connect("new_tile_hovered", interface, "_on_Map_new_tile_hovered")
	map.connect("unit_movement_issued", self, "_send_new_unit_position")

func _initialize_fake_map() -> void:
	var map_size := Vector2(100, 100)
	map.on_MapCreator_map_size_declared(map_size)
	camera.on_MapCreator_map_size_declared(map_size)
	interface.on_MapCreator_map_size_declared(map_size)

remotesync func _game_ended():
	print("Game ended, returning to main menu.")
	Client.reset_network()
	get_tree().change_scene("res://client/menu/MainMenu.tscn")

remotesync func _on_map_size_declared(map_size: Vector2):
	print("Receiving map size %s." % map_size as String)
	map.on_MapCreator_map_size_declared(map_size)
	camera.on_MapCreator_map_size_declared(map_size)
	interface.on_MapCreator_map_size_declared(map_size)

remotesync func _on_map_generation_ended():
	print("Map generation ended.")
	map.on_MapCreator_generation_ended()
	get_tree().paused = false

remotesync func _on_celestium_found(coordinates: Vector2):
	map.on_MapCreator_celestium_found(coordinates)

remotesync func _on_grass_found(coordinates: Vector2):
	map.on_MapCreator_grass_found(coordinates)

remotesync func _on_marsh_found(coordinates: Vector2):
	map.on_MapCreator_marsh_found(coordinates)

remotesync func _on_mountains_found(coordinates: Vector2):
	map.on_MapCreator_mountains_found(coordinates)

remotesync func _on_scraps_found(coordinates: Vector2):
	map.on_MapCreator_scraps_found(coordinates)

remotesync func _on_starting_position_found(coordinates: Vector2):
	print("Receiving starting coordinates %s." % coordinates as String)
	map.on_MapCreator_starting_position_found(coordinates)

remotesync func _on_unit_position_updated(entity_position: Vector2, global_path: PoolVector2Array, path: PoolVector2Array):
	map.move_unit(entity_position, global_path, path)
