extends Node2D

export(bool) var debug_enabled = true

class Tile:
	var position: Vector2
	var terrain: int
	
	func _init(_position: Vector2, _terrain: int):
		self.position = _position
		self.terrain = _terrain
	
	func set_terrain(_terrain: int):
		self.terrain = _terrain

var tiles: Array
var tile_width: int
var tile_height: int
var map_size: Vector2
var tilemap_offset: Vector2
var starting_positions: PoolVector2Array
var tile_hovered: Vector2
var point_ids_by_positions: Dictionary # <Vector2, int>
var positions_by_point_ids: Dictionary # <int, Vector2>

# TileMaps used for visual component
onready var grass: TileMap = $Grass
onready var scraps: TileMap = $Scraps
onready var marsh: TileMap = $Marsh
onready var mountains: TileMap = $Stone
onready var tilemap_grass_index: int = 6
onready var tilemap_celestium_index: int = 7
onready var tilemap_scraps_index: int = 5
onready var tilemap_marsh_index: int = 3
onready var tilemap_mountains_index: int = 4
onready var tilemap_available_tile_index: int = 8

onready var selected_tile: AnimatedSprite = $SelectedTile
onready var selection_blocked: bool = false
onready var available_tiles: TileMap = $Alpha/AvailableTiles

onready var terrain_weights: Dictionary = {
	Constants.Terrain.GRASS: 1.0,
	Constants.Terrain.MOUNTAINS: INF,
	Constants.Terrain.MARSH: 2.0
}
# Documentation: https://github.com/MatejSloboda/Dijkstra_map_for_Godot/blob/master/DOCUMENTATION.md
onready var pathfinding: DijkstraMap = DijkstraMap.new()

signal starting_positions_declared(starting_positions)
signal new_tile_hovered(tile_hovered, terrain_index)

func _ready() -> void:
	selected_tile.disable()

func _process(delta) -> void:
	var mouse_pos: Vector2 = get_global_mouse_position()
	var map_pos: Vector2 = get_map_position_from_global_position(mouse_pos)
	
	if (tile_hovered != map_pos):
		tile_hovered = map_pos
		emit_signal("new_tile_hovered", tile_hovered, get_tile(tile_hovered).terrain)
		selected_tile.enable()
		selected_tile.move_to(get_global_position_from_map_position(tile_hovered))
		
	if Input.is_action_just_pressed("mouse_left"):
		if selection_blocked:
			return
		if tile_hovered.x < 0 or tile_hovered.x >= map_size.x or tile_hovered.y < 0 or tile_hovered.y >= map_size.y:
			return
		available_tiles.clear()
		
		var temp_size: float = 12.5
		pathfinding.recalculate(point_ids_by_positions[map_pos], {"maximum_cost": temp_size, "terrain weights": terrain_weights})
		var available_point_ids: PoolIntArray = pathfinding.get_all_points_with_cost_between(0.0, temp_size)
		for point_id in available_point_ids:
			var position: Vector2 = positions_by_point_ids[point_id]
			available_tiles.set_cellv(position, tilemap_available_tile_index)
			available_tiles.update_bitmask_region( # TODO: optimize
				Vector2(position.x - temp_size, position.y - temp_size), 
				Vector2(temp_size * 2, temp_size * 2))

func get_map_position_from_global_position(global_position: Vector2) -> Vector2:
	return grass.world_to_map(global_position) - tilemap_offset

func get_global_position_from_map_position(map_position: Vector2) -> Vector2:
	return grass.map_to_world(map_position + tilemap_offset, true)

func get_tile(position: Vector2) -> Tile:
	var tile: Tile = tiles[position.x][position.y]
	return tile

func update_bitmasks(mountains_fill_offset: int):
	grass.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	scraps.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	marsh.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	mountains.update_bitmask_region(
		Vector2(mountains_fill_offset * -1, mountains_fill_offset * -1), 
		Vector2(map_size.x + mountains_fill_offset, map_size.y + mountains_fill_offset))

func clear_tilemaps():
	grass.clear()
	scraps.clear()
	marsh.clear()
	mountains.clear()
	available_tiles.clear()

func fill_outside_mountains(fill_offset: int):
	for y in range(fill_offset * -1, map_size.y + fill_offset):
		for x in range(fill_offset * -1, map_size.x + fill_offset): 
			if x < 0 or x >= map_size.x or y < 0 or y >= map_size.y:
				mountains.set_cell(x, y, tilemap_mountains_index)

func initialize_pathfinding_graph():
	point_ids_by_positions = pathfinding.add_square_grid(
		0,										# Point offset
		Rect2(0, 0, map_size.x, map_size.y),	# Bounds
		Constants.Terrain.GRASS,				# Terrain
		1.0,									# Orth-cost
		sqrt(2))								# Diag-cost
	for position in point_ids_by_positions.keys():
		var id: int = point_ids_by_positions[position]
		positions_by_point_ids[id] = position

func initialize_tiles():
	for x in range(map_size.x):
		tiles.append([])
		for y in range(map_size.y):
			tiles[x].append(Tile.new(Vector2(x, y), Constants.Terrain.GRASS))

func _on_MapCreator_map_size_declared(_map_size: Vector2):
	map_size = _map_size
	tile_width = Constants.tile_width
	tile_height = Constants.tile_height
	self.position.x = (max(map_size.x, map_size.y) * tile_width) / 2
	tilemap_offset = Vector2(map_size.x / 2, (map_size.y / 2) * -1)
	initialize_pathfinding_graph()
	initialize_tiles()
	clear_tilemaps()

func _on_MapCreator_generation_ended():
	var mountains_fill_offset: int = max(map_size.x, map_size.y)
	fill_outside_mountains(mountains_fill_offset)
	update_bitmasks(mountains_fill_offset)
	emit_signal("starting_positions_declared")

func _on_MapCreator_celestium_found(coordinates: Vector2):
	grass.set_cellv(coordinates, tilemap_celestium_index)
	get_tile(coordinates).set_terrain(Constants.Terrain.CELESTIUM)

func _on_MapCreator_grass_found(coordinates: Vector2):
	grass.set_cellv(coordinates, tilemap_grass_index)
	get_tile(coordinates).set_terrain(Constants.Terrain.GRASS)

func _on_MapCreator_marsh_found(coordinates: Vector2):
	marsh.set_cellv(coordinates, tilemap_marsh_index)
	get_tile(coordinates).set_terrain(Constants.Terrain.MARSH)
	pathfinding.set_terrain_for_point(
		point_ids_by_positions[coordinates], 
		Constants.Terrain.MARSH)

func _on_MapCreator_mountains_found(coordinates: Vector2):
	mountains.set_cellv(coordinates, tilemap_mountains_index)
	get_tile(coordinates).set_terrain(Constants.Terrain.MOUNTAINS)
	pathfinding.set_terrain_for_point(
		point_ids_by_positions[coordinates], 
		Constants.Terrain.MOUNTAINS)

func _on_MapCreator_scraps_found(coordinates: Vector2):
	scraps.set_cellv(coordinates, tilemap_scraps_index)
	get_tile(coordinates).set_terrain(Constants.Terrain.SCRAPS)

func _on_MapCreator_starting_position_found(coordinates: Vector2):
	starting_positions.append(coordinates)

func _on_Camera_dragging_started():
	selection_blocked = true

func _on_Camera_dragging_ended():
	selection_blocked = false
