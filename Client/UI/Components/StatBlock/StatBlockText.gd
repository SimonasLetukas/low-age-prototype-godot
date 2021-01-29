extends MarginContainer

export var icon: Texture
export var current_value: float = 999
export var max_value: int = 999
export var show_current_value: bool = false
export var text: String = "Biological"

func _ready():
	set_icon(icon)
	set_value(current_value, max_value, show_current_value)
	set_text(text)

func set_icon(icon: Texture) -> void:
	$StatBlock.set_icon(icon)

func set_value(current_value: float, max_value: int, show_current_value: bool = false) -> void:
	$StatBlock.set_value(current_value, max_value, show_current_value)

func set_text(text: String) -> void:
	$MarginContainer/TextType.text = text
	$MarginContainer/TextType/Shadow.text = text
