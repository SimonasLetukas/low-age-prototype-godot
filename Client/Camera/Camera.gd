extends Camera2D

export(bool) var debug_enabled = true
export(int, -200, 100, 5) var limit_horizontal_margin: int = 30
export(int, -200, 100, 5) var limit_vertical_margin: int = 5
export(float, 0.5, 3, 0.25) var minimum_zoom: float = 0.5
export(float, 0.5, 3, 0.25) var maximum_zoom: float = 1.5

var map_width_pixels: int = 904
var map_height_pixels: int = 452
var map_scroll_margin: int = 5
var map_scroll_on_boundary_enabled: bool = false
var map_scroll_speed: int = 300
var map_limit_elasticity: int = 6
var map_limit_left: int
var map_limit_right: int
var map_limit_top: int
var map_limit_bottom: int

onready var viewport_size: Vector2 = get_viewport().get_size_override()
onready var previous_position: Vector2 = Vector2(0, 0)
onready var camera_is_moving: bool = false
onready var mouse_is_on_ui: bool = false
onready var tile_width: int = Constants.tile_width
onready var tile_height: int = Constants.tile_height

signal dragging_started()
signal dragging_ended()

func _ready() -> void:
	self.position = Vector2(map_width_pixels / 2, map_height_pixels / 2)
	
	if debug_enabled:
		print("Map width: " + map_width_pixels as String)
		print("Map height: " + map_height_pixels as String)
		print("Camera position: " + self.position as String)
	
	set_limits()

func _process(delta: float) -> void:
	
	if zoomed_in():
		zoom_in()
	elif zoomed_out():
		zoom_out(delta)
	
	var move_vector: Vector2
	var mouse_pos: Vector2 = get_viewport().get_mouse_position()
	
	if Input.is_action_pressed("mouse_left"):
		if camera_is_moving == false:
			if mouse_is_on_ui == false:
				previous_position = mouse_pos
				camera_is_moving = true
				emit_signal("dragging_started")
		else:
			self.position += (previous_position - mouse_pos) * self.zoom
			previous_position = mouse_pos
	elif Input.is_action_just_released("mouse_left"):
		camera_is_moving = false
		emit_signal("dragging_ended")
	
	if (camera_is_moving == false):
		clamp_position_to_boundaries(delta)
		
		if moved_left(mouse_pos):
			move_vector.x -= 1
		if moved_right(mouse_pos):
			move_vector.x += 1
		if moved_up(mouse_pos):
			move_vector.y -= 1
		if moved_down(mouse_pos):
			move_vector.y += 1
		
		global_translate(move_vector.normalized() * delta * self.zoom.x * map_scroll_speed)

func set_limits() -> void:
	map_limit_left = limit_horizontal_margin * -1
	map_limit_right = map_width_pixels + limit_horizontal_margin
	map_limit_top = limit_vertical_margin * -1 - 25
	map_limit_bottom = map_height_pixels + limit_vertical_margin

func zoom_in() -> void:
	self.zoom.x -= 0.25
	self.zoom.y -= 0.25
	viewport_size = get_viewport().get_size_override()

func zoom_out(delta: float) -> void:
	self.zoom.x += 0.25
	self.zoom.y += 0.25
	viewport_size = get_viewport().get_size_override()
	clamp_position_to_boundaries(delta)

func clamp_position_to_boundaries(delta: float) -> void:
	if get_current_left_boundary() < map_limit_left:
		self.position.x += (map_limit_left - get_current_left_boundary()) * delta * map_limit_elasticity
	elif get_current_right_boundary() > map_limit_right:
		self.position.x -= (get_current_right_boundary() - map_limit_right) * delta * map_limit_elasticity
	if get_current_top_boundary() < map_limit_top:
		self.position.y += (map_limit_top - get_current_top_boundary()) * delta * map_limit_elasticity
	elif get_current_bottom_boundary() > map_limit_bottom:
		self.position.y -= (get_current_bottom_boundary() - map_limit_bottom) * delta * map_limit_elasticity

func get_current_left_boundary() -> float:
	var boundary: float = self.position.x - ((viewport_size.x / 2) * self.zoom.x)
	return boundary

func get_current_right_boundary() -> float:
	var boundary: float = self.position.x + ((viewport_size.x / 2) * self.zoom.x)
	return boundary

func get_current_top_boundary() -> float:
	var boundary: float = self.position.y - ((viewport_size.y / 2) * self.zoom.y)
	return boundary

func get_current_bottom_boundary() -> float:
	var boundary: float = self.position.y + ((viewport_size.y / 2) * self.zoom.y)
	return boundary

func zoomed_in() -> bool:
	if self.zoom.x <= minimum_zoom:
		return false
	if !Input.is_action_just_released("ui_zoom_in"):
		return false
	return true

func zoomed_out() -> bool:
	if self.zoom.x >= maximum_zoom:
		return false
	if !Input.is_action_just_released("ui_zoom_out"):
		return false
	return true

func moved_left(var mouse_pos: Vector2) -> bool:
	if get_current_left_boundary() <= map_limit_left:
		return false
	if Input.is_action_pressed("ui_left"):
		return true
	if mouse_pos.x < map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

func moved_right(var mouse_pos: Vector2) -> bool:
	if get_current_right_boundary() >= map_limit_right:
		return false
	if Input.is_action_pressed("ui_right"):
		return true
	if mouse_pos.x > viewport_size.x - map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

func moved_up(var mouse_pos: Vector2) -> bool:
	if get_current_top_boundary() <= map_limit_top:
		return false
	if Input.is_action_pressed("ui_up"):
		return true
	if mouse_pos.y < map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

func moved_down(var mouse_pos: Vector2) -> bool:
	if get_current_bottom_boundary() >= map_limit_bottom:
		return false
	if Input.is_action_pressed("ui_down"):
		return true
	if mouse_pos.y > viewport_size.y - map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

func _on_MapCreator_map_size_declared(map_size: Vector2):
	tile_width = Constants.tile_width
	tile_height = Constants.tile_height
	map_width_pixels = map_size.x * tile_width
	map_height_pixels = map_size.y * tile_height
	set_limits()

func _on_UI_mouse_entered():
	mouse_is_on_ui = true

func _on_UI_mouse_exited():
	mouse_is_on_ui = false
