extends MarginContainer

export var icon: Texture

func _ready() -> void:
	set_icon(icon)

func set_icon(texture: Texture) -> void:
	$AttackTypePanel/AttackTypeIcon.texture = icon
	$AttackTypePanel/AttackTypeIcon/Shadow.texture = icon
