extends HBoxContainer

class_name AbilityButtons

export var ability_icon_paths: Dictionary # <String, String>

var ability_icons: Dictionary # <String, Texture>
var ability_scene: PackedScene = preload("res://client/game/interface/panels/entity/abilities/button/AbilityButton.tscn")

signal abilities_populated()

func _ready() -> void:
	reset()
	for key in ability_icon_paths:
		ability_icons[key] = load(ability_icon_paths[key])

func reset() -> void:
	for i in get_children():
		i.queue_free()

func populate(ids: PoolStringArray) -> void:
	for id in ids:
		var ability: AbilityButton = ability_scene.instance()
		var icon: Texture = ability_icons[id]
		ability.icon = icon
		ability.set_icon(icon)
		ability.set_id(id)
		add_child(ability)
	emit_signal("abilities_populated")

func deselect_all() -> void:
	for ability_button in get_children():
		ability_button.set_selected(false)

func is_any_selected() -> bool:
	for ability_button in get_children():
		if ability_button.is_selected:
			return true
	return false
