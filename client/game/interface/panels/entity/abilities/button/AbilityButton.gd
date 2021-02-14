extends NinePatchRect

class_name AbilityButton

export var icon: Texture
export var texture_normal: Texture
export var texture_clicked: Texture

var is_selected: bool = false
var id: String = ""

signal hovering(started, ability)
signal clicked(ability)

func _ready():
	set_icon(icon)

func set_icon(new_icon: Texture) -> void:
	$TextureRect.texture = new_icon
	$TextureRect/Shadow.texture = new_icon

func set_id(new_id: String) -> void:
	id = new_id
	
func set_selected(to: bool) -> void:
	is_selected = to
	highlight(is_selected)

func set_clicked(to: bool) -> void:
	match to:
		true:
			texture = texture_clicked
		false:
			texture = texture_normal

func highlight(to: bool) -> void:
	material.set_shader_param("enabled", to)


func _on_AbilityButton_mouse_entered():
	if is_selected: 
		return
	highlight(true)
	emit_signal("hovering", true, self)


func _on_AbilityButton_mouse_exited():
	set_clicked(false)
	
	if is_selected:
		return
	highlight(false)
	emit_signal("hovering", false, self)


func _on_AbilityButton_gui_input(event):
	if event.is_action_pressed("mouse_left"):
		set_clicked(true)
	if event.is_action_released("mouse_left"):
		set_clicked(false)
		emit_signal("clicked", self)
