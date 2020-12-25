extends Node2D

# Parent of all entities (units & structures) on the map.

export(bool) var debug_enabled = true

onready var hovered_entity_position: Vector2 = Vector2.INF

var entities_by_map_positions: Dictionary # <Vector2, EntityBase>
var map_positions_by_entities: Dictionary # <EntityBase, Vector2>

signal new_entity_found(entity)

func initialize():
	for _i in self.get_children():
		if debug_enabled:
			print (_i.name)
			
		var entity_base: EntityBase = _i.get_node(@"UnitBase/EntityBase")
		if entity_base == null:
			continue
		
		emit_signal("new_entity_found", entity_base)

func register_entity(at: Vector2, entity: EntityBase) -> void:
	entities_by_map_positions[at] = entity
	map_positions_by_entities[entity] = at

func try_hovering_entity(at: Vector2) -> bool:
	if hovered_entity_position != Vector2.INF:
		var entity: EntityBase = entities_by_map_positions[hovered_entity_position]
		entity.set_tile_hovered(false)
	
	if entities_by_map_positions.has(at):
		var entity: EntityBase = entities_by_map_positions[at]
		entity.set_tile_hovered(true)
		hovered_entity_position = at
		return true
	
	hovered_entity_position = Vector2.INF
	return false

func get_entity_map_position(entity: EntityBase) -> Vector2:
	if map_positions_by_entities.has(entity):
		return map_positions_by_entities[entity]
	return Vector2.INF

func get_top_entity(global_position: Vector2) -> EntityBase:
	var intersections: Array
	var top_entity_area: Area2D
	var top_z: = 1
	var top_entity: EntityBase = null
	
	intersections = get_world_2d().get_direct_space_state().intersect_point(
		global_position, 32, [], 0x7FFFFFFF, true, true)
		
	for node in intersections:
		var area: Area2D = node.collider
		
		if (area.get_parent() is EntityBase) == false:
			continue
			
		if area.z_index < top_z:
			top_entity_area = area
			top_z = area.z_index
			top_entity = area.get_parent()
	
	return top_entity
