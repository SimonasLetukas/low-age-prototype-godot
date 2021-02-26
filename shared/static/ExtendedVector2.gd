extends Node

class_name ExtendedVector2

static func is_in_bounds(point: Vector2, size: Vector2) -> bool:
	if point.x < 0 or point.y < 0 or point.x >= size.x or point.y >= size.y:
		return false
	return true

static func encode_to_string(from: Vector2) -> String:
	return "%s,%s" % [from.x, from.y]

static func decode_from_string(from: String) -> Vector2:
	var values = from.split(",", false, 1)
	return Vector2(int(values[0]), int(values[1]))
