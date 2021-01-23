extends MarginContainer

export var icon: Texture
export var current_value: float = 999
export var max_value: int = 999
export var show_current_value: bool = false

func _ready() -> void:
	set_icon(icon)
	set_value(current_value, max_value, show_current_value)

func set_icon(texture: Texture) -> void:
	$HBoxContainer/Icon.texture = texture
	$HBoxContainer/Icon/Shadow.texture = texture

func set_value(_current_value: float, _max_value: int, _show_current_value: bool = false) -> void:
	show_current_value = _show_current_value
	var new_label_value: String = ""
	match show_current_value:
		true:
			var current_value_string: String
			_current_value = stepify(_current_value, 0.1)
			if _current_value as int == _current_value:
				current_value_string = _current_value as String
			else:
				current_value_string = "%.1f" % (_current_value)
			new_label_value = current_value_string + "/" + _max_value as String
		false:
			new_label_value = _max_value as String
	$HBoxContainer/MarginContainer/Label.text = new_label_value
	$HBoxContainer/MarginContainer/Label/Shadow.text = new_label_value
