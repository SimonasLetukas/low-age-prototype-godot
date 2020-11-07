extends Camera2D

var map_width = 500
var map_height = 300
var tile_width = 16
var tile_height = 8

export(int, 25, 200, 5) var limit_horizontal_margin = 50
export(int, 25, 200, 5) var limit_vertical_margin = 100

onready var viewportsize = get_viewport().size

func _ready():
	var half_width_pixels = (map_width / 2) * tile_width
	var half_height_pixels = (map_height / 2) * tile_height
	self.position.x = half_height_pixels
	
	self.limit_left = (half_width_pixels * -1) - limit_horizontal_margin
	self.limit_right = half_width_pixels + limit_horizontal_margin
	self.limit_top = limit_vertical_margin * -1
	self.limit_bottom = map_height + limit_vertical_margin

func _process(delta):
	var mouse_pos = get_viewport().get_mouse_position()
