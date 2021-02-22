extends Node

func _ready():
	Constants.set_local_server()

func _on_StartAsServer_pressed():
	get_tree().change_scene("res://server/ServerStartup.tscn")

func _on_StartAsClient_pressed():
	get_tree().change_scene("res://client/ClientStartup.tscn")
