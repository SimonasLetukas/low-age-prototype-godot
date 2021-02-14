extends "res://shared/game/Game.gd"

onready var map: ClientMap = $Map
onready var camera: Camera2D = $Camera
onready var mouse: Mouse = $Mouse
onready var interface: Interface = $Interface

func _ready():
	yield(get_tree().root, "ready")
	_handle_client_dependency_injection()
	
	_initialize_fake_map()

func _handle_client_dependency_injection() -> void:
	mouse.connect("left_released_without_drag", map, "_on_MouseController_left_released_without_drag")
	mouse.connect("right_released_without_examine", map, "_on_MouseController_right_released_without_examine")
	
	mouse.connect("mouse_dragged", camera, "_on_MouseController_mouse_dragged")
	mouse.connect("taking_control", camera, "_on_MouseController_taking_control")
	
	interface.connect("mouse_entered", mouse, "_on_Interface_mouse_entered")
	interface.connect("mouse_exited", mouse, "_on_Interface_mouse_exited")
	
	map.connect("new_tile_hovered", interface, "_on_Map_new_tile_hovered")

func _initialize_fake_map() -> void:
	var map_size := Vector2(100, 100)
	map.on_MapCreator_map_size_declared(map_size)
	camera.on_MapCreator_map_size_declared(map_size)
	interface.on_MapCreator_map_size_declared(map_size)

remotesync func _on_Creator_map_size_declared(map_size: Vector2):
	map.on_MapCreator_map_size_declared(map_size)
	camera.on_MapCreator_map_size_declared(map_size)
	interface.on_MapCreator_map_size_declared(map_size)

remotesync func _on_Creator_generation_ended():
	map.on_MapCreator_generation_ended()

remotesync func _on_Creator_celestium_found(coordinates: Vector2):
	map.on_MapCreator_celestium_found(coordinates)

remotesync func _on_Creator_grass_found(coordinates: Vector2):
	map.on_MapCreator_grass_found(coordinates)

remotesync func _on_Creator_marsh_found(coordinates: Vector2):
	map.on_MapCreator_marsh_found(coordinates)

remotesync func _on_Creator_mountains_found(coordinates: Vector2):
	map.on_MapCreator_mountains_found(coordinates)

remotesync func _on_Creator_scraps_found(coordinates: Vector2):
	map.on_MapCreator_scraps_found(coordinates)

remotesync func _on_Creator_starting_position_found(coordinates: Vector2):
	map.on_MapCreator_starting_position_found(coordinates)
