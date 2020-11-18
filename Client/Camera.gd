extends Camera2D

var map_width: int = 904
var map_height: int = 452
var tile_width: int = 16
var tile_height: int = 8
var map_scroll_margin: int = 5
var map_scroll_on_boundary_enabled: bool = false
var map_scroll_speed: int = 300

var half_width: int
var half_height: int

export(int, 25, 200, 5) var limit_horizontal_margin: int = 50
export(int, 25, 200, 5) var limit_vertical_margin: int = 100
export(float, 0.5, 3, 0.25) var minimum_zoom: float = 0.5
export(float, 0.5, 3, 0.25) var maximum_zoom: float = 1.5

onready var viewport_size: Vector2 = get_viewport().get_size_override()

func _ready() -> void:
	half_width = (map_width / 2)
	half_height = (map_height / 2)
	self.position = Vector2(half_width, half_height)
	
	print("Map width: " + map_width as String)
	print("Map height: " + map_height as String)
	print("Camera position: " + self.position as String)
	
	self.limit_left = limit_horizontal_margin * -1
	self.limit_right = map_width + limit_horizontal_margin
	self.limit_top = limit_vertical_margin * -1.5
	self.limit_bottom = map_height + limit_vertical_margin

func _process(delta) -> void:
	#if zoomed_in():
	#	zoom_in()
	#elif zoomed_out():
	#	zoom_out()
	
	var move_vector: Vector2
	var mouse_pos: Vector2 = get_viewport().get_mouse_position()
	
	if moved_left(mouse_pos):
		move_vector.x -= 1
	if moved_right(mouse_pos):
		move_vector.x += 1
	if moved_up(mouse_pos):
		move_vector.y -= 1
	if moved_down(mouse_pos):
		move_vector.y += 1
		
	#print("Move vector: " + move_vector.normalized() as String)
	#print("Current viewport: " + viewport_size as String)
	#print("Mouse position: " + mouse_pos as String)
	#if !limits_reached():
	global_translate(move_vector.normalized() * delta * self.zoom.x * map_scroll_speed)

func zoom_in() -> void:
	self.zoom.x -= 0.25
	self.zoom.y -= 0.25
	viewport_size = get_viewport().get_size_override()

func zoom_out() -> void:
	self.zoom.x += 0.25
	self.zoom.y += 0.25
	viewport_size = get_viewport().get_size_override()
	clamp_position_to_boundaries()

func clamp_position_to_boundaries() -> void:
	if get_current_left_boundary() < self.limit_left:
		self.position.x += self.limit_left - get_current_left_boundary()
	elif get_current_right_boundary() > self.limit_right:
		self.position.x -= get_current_right_boundary() - self.limit_right
	if get_current_top_boundary() < self.limit_top:
		self.position.y += self.limit_top - get_current_top_boundary()
	elif get_current_bottom_boundary() > self.limit_bottom:
		self.position.y -= get_current_bottom_boundary() - self.limit_bottom

func get_current_left_boundary() -> float:
	return self.position.x * 2 - (viewport_size.x * self.zoom.x)

func get_current_right_boundary() -> float:
	return self.position.x * 2 + (viewport_size.x * self.zoom.x)

func get_current_top_boundary() -> float:
	return self.position.y * 2 - (viewport_size.y * self.zoom.y)

func get_current_bottom_boundary() -> float:
	return self.position.y * 2 + (viewport_size.y * self.zoom.y)

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
	if get_current_left_boundary() <= self.limit_left:
		return false
	if Input.is_action_pressed("ui_left"):
		return true
	if mouse_pos.x < map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

func moved_right(var mouse_pos: Vector2) -> bool:
	if get_current_right_boundary() >= self.limit_right:
		return false
	if Input.is_action_pressed("ui_right"):
		return true
	if mouse_pos.x > viewport_size.x - map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

func moved_up(var mouse_pos: Vector2) -> bool:
	if get_current_top_boundary() <= self.limit_top:
		return false
	if Input.is_action_pressed("ui_up"):
		return true
	if mouse_pos.y < map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

func moved_down(var mouse_pos: Vector2) -> bool:
	if get_current_bottom_boundary() >= self.limit_bottom:
		return false
	if Input.is_action_pressed("ui_down"):
		return true
	if mouse_pos.y > viewport_size.y - map_scroll_margin and map_scroll_on_boundary_enabled:
		return true
	return false

