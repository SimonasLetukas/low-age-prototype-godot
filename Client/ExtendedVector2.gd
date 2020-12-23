extends Node

class_name ExtendedVector2

static func is_in_bounds(point: Vector2, size: Vector2) -> bool:
	if point.x < 0 or point.y < 0 or point.x >= size.x or point.y >= size.y:
		return false
	return true
