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

onready var selected: bool = false

signal mouse_entered_visuals(entity)
signal mouse_exited_visuals(entity)

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
