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

onready var tile_hovered: bool = false
onready var sprite_hovered: bool = false

signal mouse_entered_visuals(entity)
signal mouse_exited_visuals(entity)

func set_tile_hovered(to: bool) -> void:
	tile_hovered = to
	match tile_hovered:
		true:
			set_outline(true)
		false:
			if (sprite_hovered == false):
				set_outline(false)

func set_outline(to: bool) -> void:
	$Visuals.material.set_shader_param("enabled", to)

func _on_Visuals_mouse_entered():
	sprite_hovered = true
	set_outline(true)
	emit_signal("mouse_entered_visuals", self)

func _on_Visuals_mouse_exited():
	sprite_hovered = false
	if (tile_hovered == false):
		set_outline(false)
	emit_signal("mouse_exited_visuals", self)
