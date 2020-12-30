extends Node2D

# Mouse actions by priority:
# 1. SELECT (UI, unit, ability): LEFT-UP
# 2. CANCEL / ESCAPE (UI, unit, ability): RIGHT-UP UI; LEFT-UP (when nothing to select)
# 3. EXECUTE (movement, attack, ability): RIGHT-UP
# 4. DRAG CAMERA: LEFT-DOWN (initiates) -> LEFT-HOLD (disables SELECT & CANCEL)
# 5. EXAMINE / ALT-SELECT (statistics, area) - RIGHT-DOWN (initiates) -> 
#    RIGHT-HOLD (disables EXECUTE) or CTRL-HOLD

onready var camera_is_moving: bool = false
onready var mouse_is_on_ui: bool = false
onready var previous_position: Vector2 = Vector2.ZERO
onready var start_position: Vector2 = Vector2.ZERO

var arrow = load("res://Client/UI/Cursors/cursor.png")
var grab = load("res://Client/UI/Cursors/cursor grab.png")

signal mouse_dragged(by)
signal taking_control(flag)
signal left_released_without_drag()
signal right_released_without_examine()

func _ready():
	set_cursor_to_arrow()

func _process(delta):
	var mouse_pos: Vector2 = get_viewport().get_mouse_position()
	
	if Input.is_action_pressed("mouse_left"):
		if camera_is_moving == false:
			if mouse_is_on_ui == false:
				previous_position = mouse_pos
				start_position = mouse_pos
				camera_is_moving = true
				emit_signal("taking_control", true)
		else:
			var travel_vector: Vector2 = previous_position - start_position
			if travel_vector.length() > 0.25:
				var change_vector: Vector2 = previous_position - mouse_pos
				emit_signal("mouse_dragged", change_vector)
				set_cursor_to_grab()
				previous_position = mouse_pos
			else:
				previous_position = mouse_pos + travel_vector
	
	if Input.is_action_just_released("mouse_left"):
		camera_is_moving = false
		set_cursor_to_arrow()
		emit_signal("taking_control", false)
		var travel_vector: Vector2 = previous_position - start_position
		if travel_vector.length() <= 0.25:
			emit_signal("left_released_without_drag")
	
	if Input.is_action_just_pressed("mouse_right"):
		# track holding of key
		pass
	
	if Input.is_action_just_released("mouse_right"):
		emit_signal("right_released_without_examine")

func set_cursor_to_arrow():
	Input.set_custom_mouse_cursor(arrow)

func set_cursor_to_grab():
	Input.set_custom_mouse_cursor(grab)

func _on_UI_mouse_entered():
	mouse_is_on_ui = true

func _on_UI_mouse_exited():
	mouse_is_on_ui = false
