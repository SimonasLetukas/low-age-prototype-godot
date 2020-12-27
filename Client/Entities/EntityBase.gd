extends Node2D
class_name EntityBase

# Properties that define all entities are:
#	Stats:
#		Type, 
#		Cost,
#		Melee Armour,
#		Range Armour,
#		Health
#	Abilities:
#		Passive,
#		Planning

# Only structures can do research (might not be true in the future)
# Only units can do active abilities, thus have initiative. Move and 
# attack are also active abilities so all stats connected to them are 
# unique to units.

export(String, "Fast", "Medium", "Slow") var movement_speed = "Medium"

var move_path: PoolVector2Array

onready var selected: bool = false
onready var movement_duration: float = get_duration_from_movement_speed()

signal finished_moving(entity)

func _ready():
	var visuals_size: Vector2 = $Sprite.texture.get_size()
	var area: Area2D = $Area2D
	
	var shape: RectangleShape2D = RectangleShape2D.new()
	shape.set_extents(Vector2(visuals_size.x / 2, visuals_size.y / 2))

	var collision: CollisionShape2D = CollisionShape2D.new()
	collision.set_shape(shape)

	area.add_child(collision)
	area.position.y -= visuals_size.y / 2
	
	$Health.rect_position.y = (visuals_size.y * -1.0) - 2
	$Health.visible = false

func set_tile_hovered(to: bool) -> void:
	if selected:
		return
	set_outline(to)

func set_selected(to: bool) -> void:
	selected = to
	set_outline(to)

func set_outline(to: bool) -> void:
	$Sprite.material.set_shader_param("enabled", to)
	$Health.visible = to

func move_until_finished(path: PoolVector2Array) -> void:
	move_path = path
	move_to_next_target()

func move_to_next_target() -> void:
	var next_move_target: Vector2 = move_path[0]
	move_path.remove(0)
	move_path.resize(move_path.size())
	var actual_unit: Node2D = self.get_parent().get_parent()
	$Movement.interpolate_property(actual_unit, "global_position", actual_unit.global_position, 
		next_move_target, movement_duration, Tween.TRANS_QUAD, Tween.EASE_IN_OUT)
	$Movement.start()

func get_duration_from_movement_speed() -> float:
	match movement_speed:
		"Slow":
			return 0.5
		"Medium":
			return 0.25
		"Fast":
			return 0.1
		_:
			return 0.25

func _on_Movement_tween_all_completed():
	if move_path.size() == 0:
		emit_signal("finished_moving", self)
		return
	
	move_to_next_target()
