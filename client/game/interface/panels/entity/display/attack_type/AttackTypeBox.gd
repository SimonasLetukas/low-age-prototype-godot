extends MarginContainer

export var icon: Texture

var is_selected: bool = false

signal clicked()
signal hovering(started)

func _ready() -> void:
	set_icon(icon)

func set_icon(texture: Texture) -> void:
	$AttackTypePanel/AttackTypeIcon.texture = icon
	$AttackTypePanel/AttackTypeIcon/Shadow.texture = icon

func set_selected(to: bool) -> void:
	is_selected = to
	$AttackTypePanel.material.set_shader_param("enabled", is_selected)

func _on_AttackTypePanel_mouse_entered():
	if is_selected: 
		return
	$AttackTypePanel.material.set_shader_param("enabled", true)
	emit_signal("hovering", true)


func _on_AttackTypePanel_mouse_exited():
	$AttackTypePanel.set_clicked(false)
	
	if is_selected:
		return
	$AttackTypePanel.material.set_shader_param("enabled", false)
	emit_signal("hovering", false)

func _on_AttackTypePanel_gui_input(event):
	if event.is_action_pressed("mouse_left"):
		$AttackTypePanel.set_clicked(true)
	if event.is_action_released("mouse_left"):
		$AttackTypePanel.set_clicked(false)
		emit_signal("clicked")
		set_selected(!is_selected)
