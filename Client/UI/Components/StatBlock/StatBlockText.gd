extends MarginContainer

export var icon: Texture
export var value: int = 999
export var text: String = "Biological"

func _ready():
	set_icon(icon)
	set_value(value)
	set_text(text)

func set_icon(icon: Texture) -> void:
	$StatBlock.set_icon(icon)

func set_value(value: int) -> void:
	$StatBlock.set_value(0, value, false)

func set_text(text: String) -> void:
	$MarginContainer/TextType.text = text
	$MarginContainer/TextType/Shadow.text = text
