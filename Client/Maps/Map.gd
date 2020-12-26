extends Node2D

# Master node for all map-related communications: game data, visuals,
# pathfinding.

export(bool) var debug_enabled = true

var map_size: Vector2
var starting_positions: PoolVector2Array
var tile_hovered: Vector2

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
	get_hovered_entity(mouse_pos, map_pos)

func get_hovered_entity(mouse_pos: Vector2, map_pos: Vector2) -> EntityBase:
	var entity: EntityBase = entities.get_top_entity(mouse_pos)
	
	if entity != null:
		var entity_map_pos: Vector2 = entities.get_entity_map_position(entity)
		if tile_hovered != entity_map_pos:
			tile_hovered = entity_map_pos
			hover_tile()
	elif tile_hovered != map_pos:
		tile_hovered = map_pos
		var entity_hovered: bool = hover_tile()
		if entity_hovered:
			entity = entities.get_hovered_entity()
	else:
		entity = entities.get_hovered_entity()
	
	return entity

func handle_selecting(hovered_entity: EntityBase) -> void:
	if hovered_entity == null:
		handle_deselecting()
		return
	if ExtendedVector2.is_in_bounds(tile_hovered, map_size) == false:
		return

	var temp_range: float = 12.5
	var entity_position: Vector2 = entities.get_entity_map_position(hovered_entity)
	var available_tiles: PoolVector2Array = pathfinder.get_from_point(entity_position, temp_range)
	tile_map.set_available_tiles(available_tiles)
	entities.select_entity(hovered_entity)

func handle_deselecting() -> void:
	tile_map.clear_available_tiles()
	entities.deselect_entity()

func hover_tile() -> bool:
	var hovered_terrain: int = data.get_terrain(tile_hovered)
	emit_signal("new_tile_hovered", tile_hovered, hovered_terrain)
	tile_map.move_selected_tile_to(tile_hovered)
	var entity_hovered: bool = entities.try_hovering_entity(tile_hovered)
	return entity_hovered

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

func _on_EntityMap_new_entity_found(entity: EntityBase):
	var entity_position: Vector2 = entity.get_global_transform().get_origin()
	var map_pos: Vector2 = tile_map.get_map_position_from_global_position(entity_position)
	entities.register_entity(map_pos, entity)

func _on_MouseController_left_released_without_drag():
	var mouse_pos: Vector2 = get_global_mouse_position()
	var map_pos: Vector2 = tile_map.get_map_position_from_global_position(mouse_pos)
	var entity: EntityBase = get_hovered_entity(mouse_pos, map_pos)
	handle_selecting(entity)
