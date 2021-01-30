extends MarginContainer

export var icon: Texture

signal clicked()

func _ready() -> void:
	set_icon(icon)

func set_icon(texture: Texture) -> void:
	$AttackTypePanel/AttackTypeIcon.texture = icon
	$AttackTypePanel/AttackTypeIcon/Shadow.texture = icon


func _on_AttackTypePanel_mouse_entered():
	$AttackTypePanel.material.set_shader_param("enabled", true)


func _on_AttackTypePanel_mouse_exited():
	$AttackTypePanel.material.set_shader_param("enabled", false)
	$AttackTypePanel.set_clicked(false)


func _on_AttackTypePanel_gui_input(event):
	if event.is_action_pressed("mouse_left"):
		$AttackTypePanel.set_clicked(true)
	if event.is_action_released("mouse_left"):
		$AttackTypePanel.set_clicked(false)
		emit_signal("clicked")
