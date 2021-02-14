extends "Network.gd"

signal player_added
signal game_started

var local_player_name: String
var local_player_faction: int

func join_game(player_name: String, player_faction: int) -> bool:
	get_tree().connect('connected_to_server', self, 'on_connected_to_server')
	
	self.local_player_name = player_name
	self.local_player_faction = player_faction
	
	var peer = NetworkedMultiplayerENet.new()
	var result = peer.create_client(Constants.server_ip, Constants.server_port)
	
	if result == OK:
		get_tree().set_network_peer(peer)
		print("Connecting to server...")
		return true
	else:
		return false

func on_connected_to_server():
	print("Connected to server.")

func register_player(recipient_id: int, player_id: int, player_name: String, player_faction: int):
	rpc_id(recipient_id, "on_register_player", player_id, player_name, player_faction)

remote func on_register_player(player_id: int, player_name: String, player_faction: int):
	print(player_name)
	print("on_register_player: " + str(player_id))
	Data.add_player(player_id, player_name, player_faction)
	emit_signal("player_added", player_id)
	print("Total players: %d" % Data.players.size())

func start_game():
	rpc("on_start_game")

remotesync func on_start_game():
	emit_signal("game_started")
