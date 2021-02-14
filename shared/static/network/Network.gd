extends Node

signal player_removed

func _ready():
	reset_network()
	
	get_tree().connect("network_peer_disconnected", self, "_player_disconnected")

# Every network peer needs to clean up the disconnected client
func _player_disconnected(id):
	print("Player disconnected: " + str(id))
	Data.players.erase(id)
	
	emit_signal("player_removed", id)
	
	var total_players := Data.players.size()
	print("Total players: %d" % total_players)
	
	if total_players < 2:
		reset_network()

# Completely reset the game state and clear the network
func reset_network():
	var peer = get_tree().network_peer
	if peer != null:
		peer.close_connection()
	
	# Cleanup all state related to the game session
	Data.reset()
