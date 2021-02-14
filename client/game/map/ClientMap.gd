extends Map
class_name ClientMap

# Master node for all map-related communications: game data, visuals,
# pathfinding.

var map_size: Vector2
var starting_positions: PoolVector2Array
var tile_hovered: Vector2

# Tiles used for visual components and converting between global and map positions
onready var tile_map: Node2D = $Visual/Tiles

# Entities of all units and structures
onready var entities: Node2D = $Visual/Entities

# Node used for game data
onready var data: Node = Data

signal new_tile_hovered(tile_hovered, terrain_index)

func _ready() -> void:
	entities.connect("new_entity_found", self, "_on_EntityMap_new_entity_found")

func _process(_delta) -> void:
	var mouse_pos: Vector2 = get_global_mouse_position()
	var map_pos: Vector2 = tile_map.get_map_position_from_global_position(mouse_pos)
	get_hovered_entity(mouse_pos, map_pos)
	if entities.is_entity_selected():
		# TODO only if hovered tile changed from above, display path
		var path: PoolVector2Array = pathfinding.find_path(tile_hovered)
		tile_map.set_path_tiles(path)
	else:
		tile_map.clear_path()

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

func handle_execute(map_pos: Vector2) -> void:
	if entities.is_entity_selected():
		if tile_map.is_currently_available(tile_hovered):
			var path: PoolVector2Array = pathfinding.find_path(tile_hovered)
			var global_path: PoolVector2Array = tile_map.get_global_positions_from_map_positions(path)
			entities.move_selected_entity(global_path, path)
			handle_deselecting()

func handle_selecting(hovered_entity: EntityBase) -> void:
	if ExtendedVector2.is_in_bounds(tile_hovered, map_size) == false:
		return
	
	if hovered_entity == null:
		handle_deselecting()
		return

	var temp_range: float = 12.5
	var entity_position: Vector2 = entities.get_entity_map_position(hovered_entity)
	var available_tiles: PoolVector2Array = pathfinding.get_from_point(entity_position, temp_range)
	tile_map.set_available_tiles(available_tiles)
	entities.select_entity(hovered_entity)

func handle_deselecting() -> void:
	tile_map.clear_available_tiles()
	entities.deselect_entity()

func hover_tile() -> bool:
	var hovered_terrain: int = data.get_terrain(tile_hovered)
	emit_signal("new_tile_hovered", tile_hovered, hovered_terrain)
	tile_map.move_focused_tile_to(tile_hovered)
	var entity_hovered: bool = entities.try_hovering_entity(tile_hovered)
	return entity_hovered

func on_MapCreator_map_size_declared(_map_size: Vector2):
	map_size = _map_size
	self.position.x = (max(map_size.x, map_size.y) * Constants.tile_width) / 2
	tile_map.initialize(map_size)
	pathfinding.initialize(map_size)
	data.initialize(map_size)
	entities.initialize()

func on_MapCreator_generation_ended():
	tile_map.fill_outside_mountains()
	tile_map.update_all_bitmasks()
	emit_signal("starting_positions_declared") # TODO

func on_MapCreator_celestium_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.CELESTIUM)
	data.set_terrain(coordinates, Constants.Terrain.CELESTIUM)

func on_MapCreator_grass_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.GRASS)
	data.set_terrain(coordinates, Constants.Terrain.GRASS)

func on_MapCreator_marsh_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.MARSH)
	data.set_terrain(coordinates, Constants.Terrain.MARSH)
	pathfinding.set_terrain_for_point(coordinates, Constants.Terrain.MARSH)

func on_MapCreator_mountains_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.MOUNTAINS)
	data.set_terrain(coordinates, Constants.Terrain.MOUNTAINS)
	pathfinding.set_terrain_for_point(coordinates, Constants.Terrain.MOUNTAINS)

func on_MapCreator_scraps_found(coordinates: Vector2):
	tile_map.set_cell(coordinates, Constants.Terrain.SCRAPS)
	data.set_terrain(coordinates, Constants.Terrain.SCRAPS)

func on_MapCreator_starting_position_found(coordinates: Vector2):
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

func _on_MouseController_right_released_without_examine():
	var mouse_pos: Vector2 = get_global_mouse_position()
	var map_pos: Vector2 = tile_map.get_map_position_from_global_position(mouse_pos)
	handle_execute(map_pos)
