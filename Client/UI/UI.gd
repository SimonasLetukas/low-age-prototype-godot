extends CanvasLayer

export(bool) var debug_enabled = true

signal mouse_entered
signal mouse_exited

func _ready():
	for control in get_children():
		control.connect("mouse_entered", self, "_on_Control_mouse_entered", [control])
		control.connect("mouse_exited", self, "_on_Control_mouse_exited", [control])

func _on_Control_mouse_entered(which: Control):
	if debug_enabled:
		print ("Control '" + which.name + "' was entered.")
	emit_signal("mouse_entered")

func _on_Control_mouse_exited(which: Control):
	if debug_enabled:
		print ("Control '" + which.name + "' was exited.")
	emit_signal("mouse_exited")

func _on_Map_new_tile_hovered(tile_hovered: Vector2):
	$Label.text = tile_hovered as String
