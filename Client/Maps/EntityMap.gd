extends Node2D

# Parent of all entities (units & structures) on the map.

export(bool) var debug_enabled = true

onready var hovered_entity_position: Vector2 = Vector2.INF

var entities_by_map_positions: Dictionary # <Vector2, EntityBase>

signal new_entity_found(entity)
signal entity_entered(entity)
signal entity_exited(entity)

func initialize():
	for _i in self.get_children():
		if debug_enabled:
			print (_i.name)
			
		var entity_base: EntityBase = _i.get_node(@"UnitBase/EntityBase")
		if entity_base == null:
			continue
		
		entity_base.connect("mouse_entered_visuals", self, "_on_EntityBase_mouse_entered_visuals")
		entity_base.connect("mouse_exited_visuals", self, "_on_EntityBase_mouse_exited_visuals")
		emit_signal("new_entity_found", entity_base)

func register_entity(at: Vector2, entity: EntityBase) -> void:
	entities_by_map_positions[at] = entity

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

func _on_EntityBase_mouse_entered_visuals(entity: EntityBase):
	emit_signal("entity_entered", entity)
	if debug_enabled:
		print(entity.get_global_transform().get_origin())

func _on_EntityBase_mouse_exited_visuals(entity: EntityBase):
	emit_signal("entity_exited", entity)
	if debug_enabled:
		print(entity.get_global_transform().get_origin())
