extends Node2D
class_name Creator

# Map creator encapsulates work done with .png files and any
# other map generation.

# Map creator accepts a .png containing a map, list of map 
# properties and a map with all tilesets to be modified.
# The output of this generation should be various signals: 
# Map size declared, starting positions, copies of tilesets 
# with tiles generated.

export(bool) var debug_enabled = true
export(String, FILE, "*.png") var map_file_location: String
export(Color) var color_grass: Color = Color.white
export(Color) var color_mountains: Color = Color.black
export(Color) var color_marsh: Color = Color.magenta
export(Color) var color_scraps: Color = Color.red
export(Color) var color_celestium: Color = Color.green
export(Color) var color_start: Color = Color.blue

var map_size: Vector2
signal map_size_declared(map_size)
var coordinates: Vector2
signal grass_found(coordinates)
signal mountains_found(coordinates)
signal marsh_found(coordinates)
signal scraps_found(coordinates)
signal celestium_found(coordinates)
signal starting_position_found(coordinates)
signal generation_ended()

func _ready():
	if debug_enabled:
		print("MapCreator.map_file_location: " + map_file_location as String)
		print("MapCreator.color_grass: " + color_grass as String)
		print("MapCreator.color_mountains: " + color_mountains as String)
		print("MapCreator.color_marsh: " + color_marsh as String)
		print("MapCreator.color_scraps: " + color_scraps as String)
		print("MapCreator.color_celestium: " + color_celestium as String)
		print("MapCreator.color_start: " + color_start as String)
	
func generate() -> void:
	var image: Image = load(map_file_location)
	if image.is_invisible() or image.is_empty():
		if debug_enabled:
			print("MapCreator.generate: loaded image is not available")
		return
	
	map_size = Vector2(image.get_width(), image.get_height())
	emit_signal("map_size_declared", map_size)
	if debug_enabled:
		print("MapCreator.generate: signal 'map_size_declared' emitted with " + map_size as String)
	
	image.lock()
	for y in range(map_size.y):
		for x in range(map_size.x):
			var pixel: Color = image.get_pixel(x, y)
			
			# By default all tiles are grass
			emit_signal("grass_found", Vector2(x, y))
			
			if (pixel == color_grass):
				emit_signal("grass_found", Vector2(x, y))
			elif (pixel == color_mountains):
				emit_signal("mountains_found", Vector2(x, y))
			elif (pixel == color_marsh):
				emit_signal("marsh_found", Vector2(x, y))
			elif (pixel == color_scraps):
				emit_signal("scraps_found", Vector2(x, y))
			elif (pixel == color_celestium):
				emit_signal("celestium_found", Vector2(x, y))
			elif (pixel == color_start):
				emit_signal("starting_position_found", Vector2(x, y))
	
	image.unlock()
	emit_signal("generation_ended")
