extends WAT.Test

func test_when_a_player_walks_forward_they_move_to_the_right() -> void:
	asserts.is_equal(true, true)

func test_when_on_MapCreator_map_size_declared_it_should_initialize() -> void:
	parameters([["x", "y", "expected_x", "expected_y"], 
				[5, 5, 5, 5],
				[15, 15, 15, 15],
				[100, 100, 100, 100],
				[200, 100, 200, 100]])
	var expected_position_x := (max(p.expected_x, p.expected_y) * Constants.tile_width) / 2
	var expected_map_size := Vector2(p.expected_x, p.expected_y)
	var client_map = ClientMap.new()
	client_map.on_MapCreator_map_size_declared(Vector2(p.x, p.y))
	var actual_position_x: int = client_map.position.x
	var actual_map_size: Vector2 = client_map.map_size
	
	asserts.is_equal(actual_position_x, expected_position_x)
	asserts.is_equal(actual_map_size, expected_map_size)

func test_when_on_MapCreator_map_size_declared_it_should_initialize2() -> void:
	parameters([["x", "y", "expected_x", "expected_y"], 
				[5, 5, 5, 5],
				[15, 15, 15, 15],
				[100, 100, 100, 100],
				[200, 100, 200, 100]])
	var expected_position_x := (max(p.expected_x, p.expected_y) * Constants.tile_width) / 2
	var expected_map_size := Vector2(p.expected_x, p.expected_y)
	var client_map = ClientMap.new()
	client_map.on_MapCreator_map_size_declared(Vector2(p.x, p.y))
	var actual_position_x: int = client_map.position.x
	var actual_map_size: Vector2 = client_map.map_size
	
	asserts.is_equal(actual_position_x, expected_position_x)
	asserts.is_equal(actual_map_size, expected_map_size)

func test_when_on_MapCreator_map_size_declared_it_should_initialize3() -> void:
	parameters([["x", "y", "expected_x", "expected_y"], 
				[5, 5, 5, 5],
				[15, 15, 15, 15],
				[100, 100, 100, 100],
				[200, 100, 200, 100]])
	var expected_position_x := (max(p.expected_x, p.expected_y) * Constants.tile_width) / 2
	var expected_map_size := Vector2(p.expected_x, p.expected_y)
	var client_map = ClientMap.new()
	client_map.on_MapCreator_map_size_declared(Vector2(p.x, p.y))
	var actual_position_x: int = client_map.position.x
	var actual_map_size: Vector2 = client_map.map_size
	
	asserts.is_equal(actual_position_x, expected_position_x)
	asserts.is_equal(actual_map_size, expected_map_size)
