extends Node2D

var tile_width: int
var tile_height: int
var map_size: Vector2
var starting_positions: PoolVector2Array

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

signal starting_positions_declared(starting_positions)

# world_to_map

func update_bitmasks(mountains_fill_offset: int):
	grass.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	scraps.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	marsh.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	mountains.update_bitmask_region(Vector2(mountains_fill_offset * -1, mountains_fill_offset * -1), Vector2(map_size.x + mountains_fill_offset, map_size.y + mountains_fill_offset))

func clear_tilemaps():
	grass.clear()
	scraps.clear()
	marsh.clear()
	mountains.clear()

func fill_outside_mountains(fill_offset: int):
	for y in range(fill_offset * -1, map_size.y + fill_offset):
		for x in range(fill_offset * -1, map_size.x + fill_offset): 
			if x < 0 or x >= map_size.x or y < 0 or y >= map_size.y:
				mountains.set_cell(x, y, tilemap_mountains_index)


func _on_MapCreator_map_size_declared(_map_size: Vector2):
	map_size = _map_size
	tile_width = Constants.tile_width
	tile_height = Constants.tile_height
	self.position.x = (map_size.x * tile_width) / 2
	clear_tilemaps()

func _on_MapCreator_generation_ended():
	var mountains_fill_offset: int = max(map_size.x, map_size.y)
	fill_outside_mountains(mountains_fill_offset)
	update_bitmasks(mountains_fill_offset)
	emit_signal("starting_positions_declared")

func _on_MapCreator_celestium_found(coordinates: Vector2):
	grass.set_cellv(coordinates, tilemap_celestium_index)

func _on_MapCreator_grass_found(coordinates: Vector2):
	grass.set_cellv(coordinates, tilemap_grass_index)

func _on_MapCreator_marsh_found(coordinates: Vector2):
	marsh.set_cellv(coordinates, tilemap_marsh_index)

func _on_MapCreator_mountains_found(coordinates: Vector2):
	mountains.set_cellv(coordinates, tilemap_mountains_index)

func _on_MapCreator_scraps_found(coordinates: Vector2):
	scraps.set_cellv(coordinates, tilemap_scraps_index)

func _on_MapCreator_starting_position_found(coordinates: Vector2):
	starting_positions.append(coordinates)
