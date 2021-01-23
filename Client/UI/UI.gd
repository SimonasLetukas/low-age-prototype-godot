extends CanvasLayer

export(bool) var debug_enabled = true

var map_size: Vector2

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

func _on_Map_new_tile_hovered(tile_hovered: Vector2, terrain_index: int):
	var coordinates_text: String
	var terrain_text: String
	if ExtendedVector2.is_in_bounds(tile_hovered, map_size) == false:
		coordinates_text = "-, -"
		terrain_text = "Mountains"
	else: 
		coordinates_text = tile_hovered.x as String + ", " + tile_hovered.y as String
		match terrain_index:
			Constants.Terrain.GRASS:
				terrain_text = "Grass"
			Constants.Terrain.MOUNTAINS:
				terrain_text = "Mountains"
			Constants.Terrain.MARSH:
				terrain_text = "Marsh"
			Constants.Terrain.SCRAPS:
				terrain_text = "Scraps"
			Constants.Terrain.CELESTIUM:
				terrain_text = "Celestium"
			_:
				terrain_text = "Unknown"
	
	$Theme/DebugPanel/Coordinates.text = coordinates_text
	$Theme/DebugPanel/Coordinates/Shadow.text = coordinates_text
	$Theme/DebugPanel/TerrainType.text = terrain_text
	$Theme/DebugPanel/TerrainType/Shadow.text = terrain_text


func _on_MapCreator_map_size_declared(_map_size: Vector2):
	self.map_size = _map_size
