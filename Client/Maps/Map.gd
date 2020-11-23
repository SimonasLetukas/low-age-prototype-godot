extends Node2D

onready var tile_width: int = Constants.tile_width
onready var tile_height: int = Constants.tile_height

func _on_MapCreator_map_size_declared(map_size: Vector2):
	tile_width = Constants.tile_width
	tile_height = Constants.tile_height
	self.position.x = (map_size.x * tile_width) / 2
