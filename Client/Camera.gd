extends Camera2D

var map_width: int = 904
var map_height: int = 452
var tile_width: int = 16
var tile_height: int = 8
var map_scroll_margin: int = 5
var map_scroll_on_boundary_enabled: bool = false
var map_scroll_speed: int = 300

export(int, 5, 200, 5) var limit_horizontal_margin: int = 300
export(int, 5, 200, 5) var limit_vertical_margin: int = 100
export(float, 0.5, 3, 0.25) var minimum_zoom: float = 0.5
export(float, 0.5, 3, 0.25) var maximum_zoom: float = 1.5

onready var viewport_size: Vector2 = get_viewport().get_size_override()
onready var previous_position: Vector2 = Vector2(0, 0)
onready var camera_is_moving: bool = false

func _ready() -> void:
	self.position = Vector2(map_width / 2, map_height / 2)
	
	print("Map width: " + map_width as String)
	print("Map height: " + map_height as String)
	print("Camera position: " + self.position as String)
	
	self.limit_left = limit_horizontal_margin * -1
	self.limit_right = map_width + limit_horizontal_margin
	self.limit_top = limit_vertical_margin * -2.5 - 15
	self.limit_bottom = map_height + limit_vertical_margin

func _process(delta) -> void:
	
	if zoomed_in():
		zoom_in()
	elif zoomed_out():
		zoom_out()
	
	var move_vector: Vector2
	var mouse_pos: Vector2 = get_viewport().get_mouse_position()
	
	if Input.is_action_pressed("mouse_left"):
		if camera_is_moving == false:
			previous_position = mouse_pos
			camera_is_moving = true
		else:
			self.position += (previous_position - mouse_pos) * self.zoom
			previous_position = mouse_pos
	elif Input.is_action_just_released("mouse_left"):
		camera_is_moving = false
	
	if (camera_is_moving == false):
		if moved_left(mouse_pos):
			move_vector.x -= 1
		if moved_right(mouse_pos):
			move_vector.x += 1
		if moved_up(mouse_pos):
			move_vector.y -= 1
		if moved_down(mouse_pos):
			move_vector.y += 1
		
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
	var boundary: float = self.position.x - ((map_width / 5) * self.zoom.x)
	#print("Left boundary: " + boundary as String)
	return boundary

func get_current_right_boundary() -> float:
	var boundary: float = self.position.x + ((map_width / 5) * self.zoom.x)
	#print("Right boundary: " + boundary as String)
	return boundary

func get_current_top_boundary() -> float:
	var boundary: float = self.position.y - ((map_height / 5) * self.zoom.y)
	#print("Top boundary: " + boundary as String)
	return boundary

func get_current_bottom_boundary() -> float:
	var boundary: float = self.position.y + ((map_height / 5) * self.zoom.y)
	#print("Bottom boundary: " + boundary as String)
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

