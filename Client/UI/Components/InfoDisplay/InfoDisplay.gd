extends MarginContainer

enum View {UNIT_STATS, STRUCTURE_STATS, ATTACK, ABILITY}
var current_view: int = View.UNIT_STATS

var value_current_health: int = 999
var value_max_health: int = 999
var value_current_shields: int = 999
var value_max_shields: int = 0
var value_current_movement: float = 99.9
var value_max_movement: int = 99
var value_initiative: int = 99
var value_melee_armour: int = 99
var value_ranged_armour: int = 99
var value_entity_types: PoolIntArray = [Constants.EntityType.BIOLOGICAL, Constants.EntityType.ARMOURED, Constants.EntityType.RANGED]

var has_melee_attack: bool = true
var value_melee_distance: int = 1
var value_melee_damage: int = 999
var value_melee_bonus_damage: int = 999
var value_melee_bonus_type: int = Constants.EntityType.BIOLOGICAL

var has_ranged_attack: bool = false
var value_ranged_distance: int = 999
var value_ranged_damage: int = 999
var value_ranged_bonus_damage: int = 999
var value_ranged_bonus_type: int = Constants.EntityType.ARMOURED

var value_ability_name: String = "Build"
var value_ability_type: int = Constants.AbilityType.PLANNING
var value_ability_text: String = "Place a ghostly rendition of a selected enemy unit in 7 Attack Distance to an unoccupied space in a 3 Attack Distance from the selected target. The rendition has the same amount of Health, Melee and Range Armour as the selected target, cannot act, can be attacked and stays for 2 action phases. 50% of all damage done to the rendition is done as Pure Damage to the selected target. If the rendition is destroyed before disappearing, the selected target emits a blast which deals 10 Melee Damage and slows all adjacent enemies by 50% until the end of their next action."

func show_view(view: int) -> void:
	assert(view in View.values(), "View is of enum type 'View'")
	current_view = view
	match current_view:
		View.UNIT_STATS:
			$VBoxContainer/TopPart/LeftSide/Rows/Top/Health.visible = true
			$VBoxContainer/TopPart/LeftSide/Rows/Top/Shields.visible = true
			$VBoxContainer/TopPart/LeftSide/Rows/Middle/Movement.visible = true
			$VBoxContainer/TopPart/LeftSide/Rows/Middle/Initiative.visible = true
			$VBoxContainer/TopPart/LeftSide/Rows/Bottom/MeleeArmour.visible = true
			$VBoxContainer/TopPart/LeftSide/Rows/Bottom/RangedArmour.visible = true
		View.STRUCTURE_STATS:
			pass
		View.ATTACK:
			pass
		View.ABILITY:
			pass

func set_parameters_entity_stats(
	current_health: int, 
	max_health: int, 
	current_movement: float, 
	max_movement: int, 
	initiative: int, 
	melee_armour: int, 
	ranged_armour: int, 
	entity_types: PoolIntArray,
	current_shields: int = 0, 
	max_shields: int = 0) -> void:
	
	value_current_health = current_health
	value_max_health = max_health
	value_current_shields = current_shields
	value_max_shields = max_shields
	value_current_movement = current_movement
	value_max_movement = max_movement
	value_initiative = initiative
	value_melee_armour = melee_armour
	value_ranged_armour = ranged_armour
	value_entity_types = entity_types

func set_parameters_melee_attack(
	has_attack: bool, 
	distance: int,
	damage: int,
	bonus_damage: int,
	bonus_type: int) -> void:
	
	assert(bonus_type in Constants.EntityType.values(), "Bonus damage type is of enum type 'EntityType'")
	has_melee_attack = has_attack
	value_melee_distance = distance
	value_melee_damage = damage
	value_melee_bonus_damage = bonus_damage
	value_melee_bonus_type = bonus_type

func set_parameters_ranged_attack(
	has_attack: bool, 
	distance: int,
	damage: int,
	bonus_damage: int,
	bonus_type: int) -> void:
	
	assert(bonus_type in Constants.EntityType.values(), "Bonus damage type is of enum type 'EntityType'")
	has_ranged_attack = has_attack
	value_ranged_distance = distance
	value_ranged_damage = damage
	value_ranged_bonus_damage = bonus_damage
	value_ranged_bonus_type = bonus_type

func set_parameters_ability(
	name: String,
	type: int,
	text: String) -> void:
	
	assert(type in Constants.AbilityType.values(), "Ability type is of enum type 'AbilityType'")
	value_ability_name = name
	value_ability_type = type
	value_ability_text = text

