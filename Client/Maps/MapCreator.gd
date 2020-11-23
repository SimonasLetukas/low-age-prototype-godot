extends Node2D

# Map creator encapsulates work done with .png files and any
# other map generation.

# Map creator accepts a .png containing a map, list of map 
# properties and a map with all tilesets to be modified.
# The output of this generation should be various signals: 
# Map size declared, starting positions, copies of tilesets 
# with tiles generated.

export(bool) var debug_enabled = true
export(String, FILE, "*.png") var map_file_location
export(Color) var color_grass = Color.white
export(Color) var color_mountains = Color.black
export(Color) var color_marsh = Color.magenta
export(Color) var color_scraps = Color.red
export(Color) var color_celestium = Color.green
export(Color) var color_start = Color.blue
export(NodePath) var tilemap_node_grass_celestium_layer
export(NodePath) var tilemap_node_scraps_layer
export(NodePath) var tilemap_node_marsh_layer
export(NodePath) var tilemap_node_mountains_layer
export(int) var tilemap_grass_index = 6
export(int) var tilemap_celestium_index = 7
export(int) var tilemap_scraps_index = 5
export(int) var tilemap_marsh_index = 3
export(int) var tilemap_mountains_index = 4

var map_size: Vector2
signal map_size_declared(map_size)
var starting_positions: PoolVector2Array
signal starting_positions_declared(starting_positions)
var tilemap_grass_celestium_generated: TileMap
signal tilemap_grass_celestium_generated(tilemap_grass_celestium_generated)
var tilemap_scraps_generated: TileMap
signal tilemap_scraps_generated(tilemap_scraps_generated)
var tilemap_marsh_generated: TileMap
signal tilemap_marsh_generated(tilemap_marsh_generated)
var tilemap_mountains_generated: TileMap
signal tilemap_mountains_generated(tilemap_mountains_generated)

func _ready():
	if debug_enabled:
		print("MapCreator.map_file_location: " + map_file_location as String)
		print("MapCreator.color_grass: " + color_grass as String)
		print("MapCreator.color_mountains: " + color_mountains as String)
		print("MapCreator.color_marsh: " + color_marsh as String)
		print("MapCreator.color_scraps: " + color_scraps as String)
		print("MapCreator.color_celestium: " + color_celestium as String)
		print("MapCreator.color_start: " + color_start as String)
		print("MapCreator.tilemap_node_grass_celestium_layer: " + tilemap_node_grass_celestium_layer as String)
		print("MapCreator.tilemap_node_scraps_layer: " + tilemap_node_scraps_layer as String)
		print("MapCreator.tilemap_node_marsh_layer: " + tilemap_node_marsh_layer as String)
		print("MapCreator.tilemap_node_mountains_layer: " + tilemap_node_mountains_layer as String)
	
	generate()
	
func generate() -> void:
	var image: Image = Image.new()
	var _response = image.load(map_file_location)
	if image.is_invisible() or image.is_empty():
		if debug_enabled:
			print("MapCreator.generate: loaded image is not available")
		return
	
	map_size = Vector2(image.get_width(), image.get_height())
	emit_signal("map_size_declared", map_size)
	if debug_enabled:
		print("MapCreator.generate: signal 'map_size_declared' emitted with " + map_size as String)
	
	var tilemap_grass_celestium_layer: TileMap = get_node(tilemap_node_grass_celestium_layer)
	var tilemap_scraps_layer: TileMap = get_node(tilemap_node_scraps_layer)
	var tilemap_marsh_layer: TileMap = get_node(tilemap_node_marsh_layer)
	var tilemap_mountains_layer: TileMap = get_node(tilemap_node_mountains_layer)
	
	tilemap_grass_celestium_layer.clear()
	tilemap_scraps_layer.clear()
	tilemap_marsh_layer.clear()
	tilemap_mountains_layer.clear()
	
	image.lock()
	for y in range(map_size.y):
		for x in range(map_size.x):
			var pixel: Color = image.get_pixel(x, y)
			tilemap_grass_celestium_layer.set_cell(x, y, tilemap_grass_index)
			if (pixel == color_grass):
				tilemap_grass_celestium_layer.set_cell(x, y, tilemap_grass_index)
			elif (pixel == color_mountains):
				tilemap_mountains_layer.set_cell(x, y, tilemap_mountains_index)
			elif (pixel == color_marsh):
				tilemap_marsh_layer.set_cell(x, y, tilemap_marsh_index)
			elif (pixel == color_scraps):
				tilemap_scraps_layer.set_cell(x, y, tilemap_scraps_index)
			elif (pixel == color_celestium):
				tilemap_grass_celestium_layer.set_cell(x, y, tilemap_celestium_index)
			elif (pixel == color_start):
				pass
	
	image.unlock()
	
	tilemap_grass_celestium_layer.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	tilemap_scraps_layer.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	tilemap_marsh_layer.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
	tilemap_mountains_layer.update_bitmask_region(Vector2(0, 0), Vector2(map_size.x, map_size.y))
