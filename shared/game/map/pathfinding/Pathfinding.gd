extends Node
class_name Pathfinding

var map_size: Vector2
var point_ids_by_positions: Dictionary # <Vector2, int>
var positions_by_point_ids: Dictionary # <int, Vector2>

# Documentation: https://github.com/MatejSloboda/Dijkstra_map_for_Godot/blob/master/DOCUMENTATION.md
onready var pathfinding: DijkstraMap = DijkstraMap.new()
onready var terrain_weights: Dictionary = {
	Constants.Terrain.GRASS: 1.0,
	Constants.Terrain.MOUNTAINS: INF,
	Constants.Terrain.MARSH: 2.0
}

onready var previous_position: Vector2 = Vector2.INF
onready var previous_range: float = -1.0

func is_cached(_position: Vector2, _range: float) -> bool:
	if _position == previous_position and _range <= previous_range:
		return true
	return false

func cache(_position: Vector2, _range: float, _size: int) -> void:
	previous_position = _position
	previous_range = _range

func initialize(_map_size: Vector2) -> void:
	self.map_size = _map_size
	point_ids_by_positions = pathfinding.add_square_grid(
		0,										# Point offset
		Rect2(0, 0, map_size.x, map_size.y),	# Bounds
		Constants.Terrain.GRASS,				# Terrain
		1.0,									# Orth-cost
		sqrt(2))								# Diag-cost
	for position in point_ids_by_positions.keys():
		var id: int = point_ids_by_positions[position]
		positions_by_point_ids[id] = position

func set_terrain_for_point(at: Vector2, terrain_index: int) -> void:
	if ExtendedVector2.is_in_bounds(at, map_size):
		pathfinding.set_terrain_for_point(
			point_ids_by_positions[at], 
			terrain_index)

func get_from_point(_position: Vector2, _range: float, _size: int = 1) -> PoolVector2Array:
	if is_cached(_position, _range) == false:
		pathfinding.recalculate(point_ids_by_positions[_position], {"maximum_cost": _range, "terrain weights": terrain_weights})
		cache(_position, _range, _size)
	
	var available_point_ids: PoolIntArray = pathfinding.get_all_points_with_cost_between(0.0, _range)
	
	var available_positions: PoolVector2Array
	for point_id in available_point_ids:
		var position: Vector2 = positions_by_point_ids[point_id]
		available_positions.append(position)
	
	return available_positions

func find_path(to: Vector2) -> PoolVector2Array:
	var path: PoolVector2Array
	path.append(to)
	
	if ExtendedVector2.is_in_bounds(to, map_size) == false:
		return path
	
	var target_point_id: int = point_ids_by_positions[to]
	
	var path_point_ids: PoolIntArray = pathfinding.get_shortest_path_from_point(target_point_id)
	
	for point_id in path_point_ids:
		var position: Vector2 = positions_by_point_ids[point_id]
		path.append(position)
	
	path.invert()
	return path
