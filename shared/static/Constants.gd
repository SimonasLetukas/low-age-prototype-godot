extends Node

const tile_width: int = 16
const tile_height: int = 8

const player_id_key: String = "id"
const player_name_key: String = "name"
const player_faction_key: String = "faction"

const server_id := 1
const server_ip := "35.228.22.84"
const server_port := 3000
const max_players := 2

enum Faction {REVELATORS = 0, UEE = 1}
enum Terrain {GRASS = 0, MOUNTAINS = 1, MARSH = 2, SCRAPS = 3, CELESTIUM = 4}
enum EntityType {LIGHT = 0, ARMOURED = 1, GIANT = 2, BIOLOGICAL = 3, MECHANICAL = 4, CELESTIAL = 5, STRUCTURE = 6, RANGED = 7}
enum AbilityType {PASSIVE = 0, PLANNING = 1, ACTION = 2, MOVEMENT = 3}

