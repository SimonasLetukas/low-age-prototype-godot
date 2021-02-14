extends TextureRect

export var image: Texture
export var image_clicked: Texture

func _ready() -> void:
	set_clicked(false)

func set_clicked(to: bool) -> void:
	match to:
		true:
			texture = image_clicked
			$AttackTypeIcon.modulate.a = 0.7
		false:
			texture = image
			$AttackTypeIcon.modulate.a = 1
