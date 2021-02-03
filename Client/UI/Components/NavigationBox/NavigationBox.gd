extends MarginContainer

export var icon: Texture

signal clicked()

func _ready() -> void:
	set_icon(icon)

func set_icon(texture: Texture) -> void:
	$NavigationBoxPanel/NavigationBoxIcon.texture = icon
	$NavigationBoxPanel/NavigationBoxIcon/Shadow.texture = icon

func _on_NavigationBoxPanel_mouse_entered():
	$NavigationBoxPanel.material.set_shader_param("enabled", true)


func _on_NavigationBoxPanel_mouse_exited():
	$NavigationBoxPanel.set_clicked(false)
	$NavigationBoxPanel.material.set_shader_param("enabled", false)


func _on_NavigationBoxPanel_gui_input(event: InputEvent):
	if event.is_action_pressed("mouse_left"):
		$NavigationBoxPanel.set_clicked(true)
	if event.is_action_released("mouse_left"):
		$NavigationBoxPanel.set_clicked(false)
		$NavigationBoxPanel.material.set_shader_param("enabled", false)
		emit_signal("clicked")
