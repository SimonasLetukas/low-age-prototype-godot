extends YSort

# Parent of all entities (units & structures) on the map.

export(bool) var debug_enabled = true

onready var hovered_entity_position: Vector2 = Vector2.INF
onready var selected_entity: EntityBase = null

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

func select_entity(entity: EntityBase) -> void:
	if is_entity_selected():
		selected_entity.set_selected(false)
	selected_entity = entity
	selected_entity.set_selected(true)

func deselect_entity() -> void:
	if is_entity_selected():
		selected_entity.set_selected(false)
		selected_entity = null

func is_entity_selected() -> bool:
	return selected_entity != null

func try_hovering_entity(at: Vector2) -> bool:
	if hovered_entity_position == at:
		return true
	
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

func get_entity_from_map_position(map_position: Vector2) -> EntityBase:
	if entities_by_map_positions.has(map_position):
		return entities_by_map_positions[map_position]
	return null

func get_hovered_entity() -> EntityBase:
	if entities_by_map_positions.has(hovered_entity_position):
		return entities_by_map_positions[hovered_entity_position]
	return null

# TODO: seems like each entity has z_index of 0 and so this func doesn't work
func get_top_entity(global_position: Vector2) -> EntityBase:
	var intersections: Array
	var top_z: = -INF
	var top_entity: EntityBase = null
	
	intersections = get_world_2d().get_direct_space_state().intersect_point(
		global_position, 32, [], 0x7FFFFFFF, true, true)
		
	for node in intersections:
		var area: Area2D = node.collider
		
		if (area.get_parent() is EntityBase) == false:
			continue
		
		var entity: EntityBase = area.get_parent()
		
		if entity == null: 
			continue
		
		if debug_enabled:
			#print(entity.get_parent().get_parent().name as String + " " + get_absolute_z_index(entity) as String)
			pass
		
		if entity.z_index > top_z:
			top_z = entity.z_index
			top_entity = entity
	
	return top_entity

static func get_absolute_z_index(target: Node2D) -> int:
	var node: Node2D = target
	var z_index: int = 0
	while node and node.is_class("Node2D"):
		z_index += node.z_index
		if !node.z_as_relative:
			break
		node = node.get_parent()
	return z_index
