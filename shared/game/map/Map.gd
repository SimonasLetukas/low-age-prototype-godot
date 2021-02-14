extends Node2D
class_name Map

# Master node for all map-related communications: game data, visuals,
# pathfinding.

export(bool) var debug_enabled = true

# Pathfinding used as a gateway to Dijkstra library
onready var pathfinding: Node = $Pathfinding

signal starting_positions_declared(starting_positions)

func _ready() -> void:
	pass
