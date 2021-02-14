extends Node

# Setup client and open main menu
func _ready():
	get_tree().change_scene("res://client/menu/MainMenu.tscn")
