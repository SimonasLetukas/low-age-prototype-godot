extends "Network.gd"

# Called by clients when they connect
func register_self(player_id: int, player_name: String, player_faction: int):
	rpc_id(Constants.server_id, "on_register_self", player_id, player_name, player_faction)

remote func on_register_self(player_id, player_name, player_faction):
	# Register this client with the server
	Client.on_register_player(player_id, player_name, player_faction)
	
	# Register the new player with all existing clients
	for current_player_id in Data.players.keys():
		Client.register_player(current_player_id, player_id, player_name, player_faction)
	
	# Catch the new player up on who is already here
	for current_player_id in Data.players.keys():
		if current_player_id != player_id:
			var player = Data.players[current_player_id]
			Client.register_player(player_id, current_player_id, player.name, player.faction)

func is_hosting() -> bool:
	if get_tree().network_peer != null and get_tree().network_peer.get_connection_status() != NetworkedMultiplayerENet.ConnectionStatus.CONNECTION_DISCONNECTED:
		return true
	else:
		return false

func host_game() -> bool:
	Client.reset_network()
	
	var peer = NetworkedMultiplayerENet.new()
	var result = peer.create_server(Constants.server_port, Constants.max_players)
	if result == OK:
		get_tree().set_network_peer(peer)
		
		get_tree().connect("network_peer_connected", self, "_player_connected")
		
		print("Server started.")
		return true
	else:
		print("Failed to host game: %d" % result)
		return false

func _player_connected(id):
	print("Player connected: " + str(id))
