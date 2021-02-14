extends AnimatedSprite

func move_to(coordinates: Vector2) -> void:
	self.global_position = coordinates

func disable() -> void:
	self.visible = false

func enable() -> void:
	self.visible = true
