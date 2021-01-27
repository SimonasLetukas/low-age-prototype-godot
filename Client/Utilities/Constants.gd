extends Node

onready var tile_width: int = 16
onready var tile_height: int = 8

enum Terrain {GRASS = 0, MOUNTAINS = 1, MARSH = 2, SCRAPS = 3, CELESTIUM = 4}
enum EntityType {LIGHT = 0, ARMOURED = 1, GIANT = 2, BIOLOGICAL = 3, MECHANICAL = 4, CELESTIAL = 5, STRUCTURE = 6, RANGED = 7}
enum AbilityType {PASSIVE = 0, PLANNING = 1, ACTION = 2}
