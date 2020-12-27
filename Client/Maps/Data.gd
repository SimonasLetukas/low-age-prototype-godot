extends Node

# Handles game data for tiles, units & structures. Might be moved out of 
# Map in the future multiplayer implementation.

class Tile:
	var position: Vector2
	var terrain: int
	var unit: Unit
	
	func _init(_position: Vector2, _terrain: int) -> void:
		self.position = _position
		self.terrain = _terrain
	
	func set_terrain(_terrain: int) -> void:
		self.terrain = _terrain
	
	func set_unit(_unit: Unit) -> void:
		self.unit = _unit

class Unit:
	var position: Vector2
	
	func _init(_position: Vector2) -> void:
		self.position = _position
	
	func set_position(_position: Vector2) -> void:
		self.position = _position

var map_size: Vector2
var units: Array
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

func new_unit(at: Vector2) -> Unit:
	var unit: Unit = Unit.new(at)
	units[at.x][at.y] = unit
	
	var tile: Tile = tiles[at.x][at.y]
	tile.set_unit(unit)
	
	return unit

func move_unit(from: Vector2, to: Vector2) -> void:
	if ExtendedVector2.is_in_bounds(from, map_size) == false or ExtendedVector2.is_in_bounds(to, map_size) == false:
		return
	var unit: Unit = units[from.x][from.y]
	var tile_from: Tile = tiles[from.x][from.y]
	var tile_to: Tile = tiles[to.x][to.y]
	
	unit.set_position(to)
	tile_from.set_unit(null)
	tile_to.set_unit(unit)

func get_terrain(at: Vector2) -> int:
	var tile_terrain: int = Constants.Terrain.MOUNTAINS
	if ExtendedVector2.is_in_bounds(at, map_size):
		tile_terrain = get_tile(at).terrain
	return tile_terrain

func set_terrain(at: Vector2, index: int) -> void:
	if ExtendedVector2.is_in_bounds(at, map_size):
		get_tile(at).set_terrain(index)
