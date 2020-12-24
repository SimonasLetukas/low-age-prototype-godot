extends Node

# Handles game data for tiles, units & structures. Might be moved out of 
# Map in the future multiplayer implementation.

class Tile:
	var position: Vector2
	var terrain: int
	
	func _init(_position: Vector2, _terrain: int):
		self.position = _position
		self.terrain = _terrain
	
	func set_terrain(_terrain: int):
		self.terrain = _terrain

var map_size: Vector2
var tiles: Array

func initialize(_map_size: Vector2) -> void:
	self.map_size = _map_size
	
	for x in range(map_size.x):
		tiles.append([])
		for y in range(map_size.y):
			tiles[x].append(Tile.new(Vector2(x, y), Constants.Terrain.GRASS))

func get_tile(at: Vector2) -> Tile:
	var tile: Tile = tiles[at.x][at.y]
	return tile

func get_terrain(at: Vector2) -> int:
	var tile_terrain: int = Constants.Terrain.MOUNTAINS
	if ExtendedVector2.is_in_bounds(at, map_size):
		tile_terrain = get_tile(at).terrain
	return tile_terrain

func set_terrain(at: Vector2, index: int) -> void:
	if ExtendedVector2.is_in_bounds(at, map_size):
		get_tile(at).set_terrain(index)
