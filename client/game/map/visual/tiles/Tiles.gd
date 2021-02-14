extends Node2D

# Responsible for handling all tile-map related visuals and
# conversions between global and map positions.

var map_size: Vector2
var tilemap_offset: Vector2
var tile_offset: Vector2
var mountains_fill_offset: int 

onready var tile_width: int = Constants.tile_width
onready var tile_height: int = Constants.tile_height

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
onready var tilemap_path_tile_index: int = 9

onready var focused_tile: AnimatedSprite = $FocusedTile

onready var available_tiles: TileMap = $Alpha/Available

onready var path: TileMap = $Path

func _ready() -> void:
	focused_tile.disable()

func initialize(_map_size: Vector2) -> void:
	self.map_size = _map_size
	tilemap_offset = Vector2(map_size.x / 2, (map_size.y / 2) * -1)
	mountains_fill_offset = max(map_size.x, map_size.y)
	tile_offset = Vector2(0, tile_height / 2)
	clear_tilemaps()

func get_map_position_from_global_position(global_position: Vector2) -> Vector2:
	return grass.world_to_map(global_position) - tilemap_offset

func get_global_position_from_map_position(map_position: Vector2) -> Vector2:
	return grass.map_to_world(map_position + tilemap_offset, true) + tile_offset

func get_global_positions_from_map_positions(map_positions: PoolVector2Array) -> PoolVector2Array:
	var global_positions: PoolVector2Array
	for map_position in map_positions:
		global_positions.append(get_global_position_from_map_position(map_position))
	return global_positions

func get_tilemap_index_from_terrain_index(terrain_index: int) -> int:
	match terrain_index:
		Constants.Terrain.GRASS:
			return tilemap_grass_index
		Constants.Terrain.MOUNTAINS:
			return tilemap_mountains_index
		Constants.Terrain.MARSH:
			return tilemap_marsh_index
		Constants.Terrain.SCRAPS:
			return tilemap_scraps_index
		Constants.Terrain.CELESTIUM:
			return tilemap_celestium_index
		_:
			return tilemap_grass_index

func set_cell(at: Vector2, terrain_index: int) -> void:
	match terrain_index:
		Constants.Terrain.MOUNTAINS:
			mountains.set_cellv(at, tilemap_mountains_index)
		Constants.Terrain.MARSH:
			marsh.set_cellv(at, tilemap_marsh_index)
		Constants.Terrain.SCRAPS:
			scraps.set_cellv(at, tilemap_scraps_index)
		Constants.Terrain.CELESTIUM:
			grass.set_cellv(at, tilemap_celestium_index)
		Constants.Terrain.GRASS:
			continue
		_:
			grass.set_cellv(at, tilemap_grass_index)

func set_celestium(at: Vector2) -> void:
	grass.set_cellv(at, tilemap_celestium_index)

func move_focused_tile_to(position: Vector2) -> void:
	focused_tile.enable()
	focused_tile.move_to(get_global_position_from_map_position(position))

func set_available_tiles(available_positions: PoolVector2Array) -> void:
	clear_available_tiles()
	
	for available_position in available_positions:
		available_tiles.set_cellv(available_position, tilemap_available_tile_index)
		available_tiles.update_bitmask_region(available_position)

func is_currently_available(map_position: Vector2) -> bool:
	return available_tiles.get_cellv(map_position) == tilemap_available_tile_index

func set_path_tiles(path_positions: PoolVector2Array) -> void:
	clear_path()
	
	for path_position in path_positions:
		path.set_cellv(path_position, tilemap_path_tile_index)

func fill_outside_mountains():
	for y in range(mountains_fill_offset * -1, map_size.y + mountains_fill_offset):
		for x in range(mountains_fill_offset * -1, map_size.x + mountains_fill_offset): 
			if x < 0 or x >= map_size.x or y < 0 or y >= map_size.y:
				mountains.set_cell(x, y, tilemap_mountains_index)

func update_all_bitmasks():
	grass.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	scraps.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	marsh.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	mountains.update_bitmask_region(
		Vector2(mountains_fill_offset * -1, mountains_fill_offset * -1), 
		Vector2(map_size.x + mountains_fill_offset, map_size.y + mountains_fill_offset))

func clear_available_tiles():
	available_tiles.clear()

func clear_path():
	path.clear()

func clear_tilemaps():
	grass.clear()
	scraps.clear()
	marsh.clear()
	mountains.clear()
	available_tiles.clear()
	path.clear()
