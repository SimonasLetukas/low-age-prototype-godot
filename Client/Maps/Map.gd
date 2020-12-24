extends Node2D

# Master node for all map-related communications: game data, visuals,
# pathfinding.

export(bool) var debug_enabled = true

var map_size: Vector2
var starting_positions: PoolVector2Array
var tile_hovered: Vector2

onready var selection_blocked: bool = false
onready var ignore_mouse_hovering: bool = false

# TileMap used for visual components
onready var tile_map: Node2D = $TileMap

# Entities of all units and structures
onready var entities: Node2D = $EntityMap

# Node used for game data
onready var data: Node = $Data

# Pathfinder used as a gateway to Dijkstra library
onready var pathfinder: Node = $Pathfinder

signal starting_positions_declared(starting_positions)
signal new_tile_hovered(tile_hovered, terrain_index)

func _process(_delta) -> void:
	var mouse_pos: Vector2 = get_global_mouse_position()
	var map_pos: Vector2 = tile_map.get_map_position_from_global_position(mouse_pos)
	
	if tile_hovered != map_pos && ignore_mouse_hovering == false:
		tile_hovered = map_pos
		hover_tile()
		
	if Input.is_action_just_pressed("mouse_left"):
		if selection_blocked:
			return
		if ExtendedVector2.is_in_bounds(tile_hovered, map_size) == false:
			return
		
		var temp_range: float = 12.5
		var available_tiles: PoolVector2Array = pathfinder.get_from_point(map_pos, temp_range)
		tile_map.set_available_tiles(available_tiles)

func hover_tile() -> void:
	var hovered_terrain: int = data.get_terrain(tile_hovered)
	emit_signal("new_tile_hovered", tile_hovered, hovered_terrain)
	tile_map.move_selected_tile_to(tile_hovered)
	entities.try_hovering_entity(tile_hovered)

func _on_MapCreator_map_size_declared(_map_size: Vector2):
	map_size = _map_size
	self.position.x = (max(map_size.x, map_size.y) * Constants.tile_width) / 2
	tile_map.initialize(map_size)
	pathfinder.initialize(map_size)
	data.initialize(map_size)
	entities.initialize()

func _on_MapCreator_generation_ended():
	tile_map.fill_outside_mountains()
	tile_map.update_all_bitmasks()
	emit_signal("starting_positions_declared") # TODO

func _on_MapCreator_celestium_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.CELESTIUM)
	data.set_terrain(coordinates, Constants.Terrain.CELESTIUM)

func _on_MapCreator_grass_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.GRASS)
	data.set_terrain(coordinates, Constants.Terrain.GRASS)

func _on_MapCreator_marsh_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.MARSH)
	data.set_terrain(coordinates, Constants.Terrain.MARSH)
	pathfinder.set_terrain_for_point(coordinates, Constants.Terrain.MARSH)

func _on_MapCreator_mountains_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.MOUNTAINS)
	data.set_terrain(coordinates, Constants.Terrain.MOUNTAINS)
	pathfinder.set_terrain_for_point(coordinates, Constants.Terrain.MOUNTAINS)

func _on_MapCreator_scraps_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.SCRAPS)
	data.set_terrain(coordinates, Constants.Terrain.SCRAPS)

func _on_MapCreator_starting_position_found(coordinates: Vector2):
	starting_positions.append(coordinates)

func _on_Camera_dragging_started():
	selection_blocked = true

func _on_Camera_dragging_ended():
	selection_blocked = false

func _on_EntityMap_entity_entered(entity: EntityBase):
	ignore_mouse_hovering = true
	var entity_position: Vector2 = entity.get_global_transform().get_origin()
	var map_pos: Vector2 = tile_map.get_map_position_from_global_position(entity_position)
	tile_hovered = map_pos
	hover_tile()

func _on_EntityMap_entity_exited(entity: EntityBase):
	ignore_mouse_hovering = false

func _on_EntityMap_new_entity_found(entity: EntityBase):
	var entity_position: Vector2 = entity.get_global_transform().get_origin()
	var map_pos: Vector2 = tile_map.get_map_position_from_global_position(entity_position)
	entities.register_entity(map_pos, entity)
