extends Node

func _ready():
	print("Application started")
	if OS.has_feature("server"):
		print("Starting as server")
		get_tree().change_scene("res://server/ServerStartup.tscn")
	elif OS.has_feature("client"):
		print("Starting as client")
		get_tree().change_scene("res://client/ClientStartup.tscn")
	else:
		print("Unidentified startup, starting as client")
		get_tree().change_scene("res://client/ClientStartup.tscn")
		#get_tree().change_scene("res://server/ServerStartup.tscn")
