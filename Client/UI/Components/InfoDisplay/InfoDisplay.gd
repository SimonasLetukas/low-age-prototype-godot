extends MarginContainer

class_name InfoDisplay

enum View {UNIT_STATS, STRUCTURE_STATS, ATTACK_MELEE, ATTACK_RANGED, ABILITY}
var current_view: int = View.UNIT_STATS
var previous_view: int = View.UNIT_STATS

signal abilities_closed()
signal ability_text_resized()

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
var value_melee_name: String = "Venom Fangs"
var value_melee_distance: int = 1
var value_melee_damage: int = 999
var value_melee_bonus_damage: int = 999
var value_melee_bonus_type: int = Constants.EntityType.BIOLOGICAL

var has_ranged_attack: bool = true
var value_ranged_name: String = "Monev Fangs"
var value_ranged_distance: int = 999
var value_ranged_damage: int = 999
var value_ranged_bonus_damage: int = 999
var value_ranged_bonus_type: int = Constants.EntityType.ARMOURED

var value_ability_name: String = "Build"
var value_ability_type: int = Constants.AbilityType.PLANNING
var value_ability_text: String = "Place a ghostly rendition of a selected enemy unit in [b]7[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] to an unoccupied space in a [b]3[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] from the selected target. The rendition has the same amount of [img=15x11]Client/UI/Icons/icon_health_big.png[/img], [img=15x11]Client/UI/Icons/icon_melee_armour_big.png[/img] and [img=15x11]Client/UI/Icons/icon_ranged_armour_big.png[/img] as the selected target, cannot act, can be attacked and stays for [b]2[/b] action phases. [b]50%[/b] of all [img=15x11]Client/UI/Icons/icon_damage_big.png[/img] done to the rendition is done as pure [img=15x11]Client/UI/Icons/icon_damage_big.png[/img]to the selected target. If the rendition is destroyed before disappearing, the selected target emits a blast which deals [b]10[/b][img=15x11]Client/UI/Icons/icon_melee_attack.png[/img] and slows all adjacent enemies by [b]50%[/b] until the end of their next action."
var value_ability_cooldown: int = 3
var value_ability_cooldown_type: int = Constants.AbilityType.ACTION
var value_research_text: String = "Hardened Matrix"

func _ready() -> void:
	show_view(View.UNIT_STATS)

func show_view(view: int) -> void:
	assert(view in View.values(), "View is of enum type 'View'")
	previous_view = current_view
	current_view = view
	match current_view:
		View.UNIT_STATS:
			reset()
			show_unit_stats()
		View.STRUCTURE_STATS:
			reset()
			show_structure_stats()
		View.ATTACK_MELEE:
			reset()
			show_melee_attack()
		View.ATTACK_RANGED:
			reset()
			show_ranged_attack()
		View.ABILITY:
			reset()
			show_ability()

func reset() -> void:
	$VBoxContainer/TopPart/AbilityTitle.visible = false
	$VBoxContainer/TopPart/AbilityTitle/Top/Research.visible = false
	$VBoxContainer/TopPart/NavigationBox.visible = false
	$VBoxContainer/TopPart/LeftSide.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Top.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/TopText.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Movement.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Initiative.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Damage.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Distance.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/MeleeArmour.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/RangedArmour.visible = false
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/StatBlockText.visible = false
	$VBoxContainer/TopPart/RightSide.visible = false
	$VBoxContainer/TopPart/RightSide/Attacks/Melee.visible = false
	$VBoxContainer/TopPart/RightSide/Attacks/Ranged.visible = false
	$VBoxContainer/AbilityDescription.visible = false
	$VBoxContainer/EntityTypes.visible = false

func show_unit_stats() -> void:
	$VBoxContainer/TopPart/LeftSide/Rows/Top/Health.set_value(value_current_health, value_max_health, false if value_current_health == value_max_health else true)
	$VBoxContainer/TopPart/LeftSide/Rows/Top/Shields.set_value(value_current_shields, value_max_shields, false if value_current_shields == value_max_shields else true)
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Movement.set_value(value_current_movement, value_max_movement, false if value_current_movement == value_max_movement else true)
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Initiative.set_value(0, value_initiative, false)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/MeleeArmour.set_value(0, value_melee_armour, false)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/RangedArmour.set_value(0, value_ranged_armour, false)
	$VBoxContainer/EntityTypes.set_types(value_entity_types)
	
	$VBoxContainer/TopPart/LeftSide.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Top.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Top/Health.visible = true
	if value_current_shields > 0 || value_max_shields > 0:
		$VBoxContainer/TopPart/LeftSide/Rows/Top/Shields.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Movement.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Initiative.visible = true 
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/MeleeArmour.visible = true 
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/RangedArmour.visible = true 
	$VBoxContainer/TopPart/RightSide.visible = true
	if has_melee_attack:
		$VBoxContainer/TopPart/RightSide/Attacks/Melee.visible = true
	if has_ranged_attack:
		$VBoxContainer/TopPart/RightSide/Attacks/Ranged.visible = true
	$VBoxContainer/EntityTypes.visible = true 

func show_structure_stats() -> void:
	$VBoxContainer/TopPart/LeftSide/Rows/Top/Health.set_value(value_current_health, value_max_health, false if value_current_health == value_max_health else true)
	$VBoxContainer/TopPart/LeftSide/Rows/Top/Shields.set_value(value_current_shields, value_max_shields, false if value_current_shields == value_max_shields else true)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/MeleeArmour.set_value(0, value_melee_armour, false)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/RangedArmour.set_value(0, value_ranged_armour, false)
	$VBoxContainer/EntityTypes.set_types(value_entity_types)
	
	$VBoxContainer/TopPart/LeftSide.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Top.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Top/Health.visible = true
	if value_current_shields > 0 || value_max_shields > 0:
		$VBoxContainer/TopPart/LeftSide/Rows/Top/Shields.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/MeleeArmour.visible = true 
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/RangedArmour.visible = true 
	$VBoxContainer/EntityTypes.visible = true 

func show_melee_attack() -> void:
	$VBoxContainer/TopPart/LeftSide/Rows/TopText.set_name(value_melee_name)
	$VBoxContainer/TopPart/LeftSide/Rows/TopText.set_melee()
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Damage.set_value(0, value_melee_damage, false)
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Distance.set_value(0, value_melee_distance, false)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/StatBlockText.set_value(value_melee_bonus_damage)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/StatBlockText.set_text(Constants.EntityType.keys()[value_melee_bonus_type].capitalize())
	$VBoxContainer/EntityTypes.set_types(value_entity_types)
	
	$VBoxContainer/TopPart/LeftSide.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/TopText.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Damage.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Distance.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/StatBlockText.visible = true
	$VBoxContainer/TopPart/RightSide.visible = true
	if has_melee_attack:
		$VBoxContainer/TopPart/RightSide/Attacks/Melee.visible = true
	if has_ranged_attack:
		$VBoxContainer/TopPart/RightSide/Attacks/Ranged.visible = true
	$VBoxContainer/EntityTypes.visible = true

func show_ranged_attack() -> void:
	$VBoxContainer/TopPart/LeftSide/Rows/TopText.set_name(value_ranged_name)
	$VBoxContainer/TopPart/LeftSide/Rows/TopText.set_ranged()
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Damage.set_value(0, value_ranged_damage, false)
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Distance.set_value(0, value_ranged_distance, false)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/StatBlockText.set_value(value_ranged_bonus_damage)
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/StatBlockText.set_text(Constants.EntityType.keys()[value_ranged_bonus_type].capitalize())
	$VBoxContainer/EntityTypes.set_types(value_entity_types)
	
	$VBoxContainer/TopPart/LeftSide.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/TopText.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Damage.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Middle/Distance.visible = true
	$VBoxContainer/TopPart/LeftSide/Rows/Bottom/StatBlockText.visible = true
	$VBoxContainer/TopPart/RightSide.visible = true
	if has_melee_attack:
		$VBoxContainer/TopPart/RightSide/Attacks/Melee.visible = true
	if has_ranged_attack:
		$VBoxContainer/TopPart/RightSide/Attacks/Ranged.visible = true
	$VBoxContainer/EntityTypes.visible = true

func show_ability() -> void:
	$VBoxContainer/TopPart/AbilityTitle/Top/Name/Label.text = value_ability_name
	$VBoxContainer/TopPart/AbilityTitle/Top/Name/Label/Shadow.text = value_ability_name
	$VBoxContainer/TopPart/AbilityTitle/Top/Research.set_research(value_research_text)
	$VBoxContainer/TopPart/AbilityTitle/Type.set_type(value_ability_type, value_ability_cooldown, value_ability_cooldown_type)
	$VBoxContainer/AbilityDescription/Text.bbcode_text = value_ability_text
	$VBoxContainer/AbilityDescription/Text/Shadow.bbcode_text = value_ability_text
	
	$VBoxContainer/TopPart/AbilityTitle.visible = true
	if value_research_text.empty() == false:
		$VBoxContainer/TopPart/AbilityTitle/Top/Research.visible = true
	$VBoxContainer/TopPart/NavigationBox.visible = true
	$VBoxContainer/AbilityDescription.visible = true

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
	attack_name: String = "",
	distance: int = 0,
	damage: int = 0,
	bonus_damage: int = 0,
	bonus_type: int = 0) -> void:
	
	assert(bonus_type in Constants.EntityType.values(), "Bonus damage type is of enum type 'EntityType'")
	has_melee_attack = has_attack
	value_melee_name = attack_name
	value_melee_distance = distance
	value_melee_damage = damage
	value_melee_bonus_damage = bonus_damage
	value_melee_bonus_type = bonus_type

func set_parameters_ranged_attack(
	has_attack: bool, 
	attack_name: String = "",
	distance: int = 0,
	damage: int = 0,
	bonus_damage: int = 0,
	bonus_type: int = 0) -> void:
	
	assert(bonus_type in Constants.EntityType.values(), "Bonus damage type is of enum type 'EntityType'")
	has_ranged_attack = has_attack
	value_ranged_name = attack_name
	value_ranged_distance = distance
	value_ranged_damage = damage
	value_ranged_bonus_damage = bonus_damage
	value_ranged_bonus_type = bonus_type

func set_parameters_ability(
	ability_name: String,
	type: int,
	text: String,
	research: String = "",
	cooldown: int = 0,
	cooldown_type: int = 0) -> void:
	
	assert(type in Constants.AbilityType.values(), "Ability type is of enum type 'AbilityType'")
	assert(cooldown_type in Constants.AbilityType.values(), "Cooldown type is of enum type 'AbilityType'")
	value_ability_name = ability_name
	value_ability_type = type
	value_ability_text = text
	value_research_text = research
	value_ability_cooldown = cooldown
	value_ability_cooldown_type = cooldown_type

func _on_Melee_clicked():
	var button = $VBoxContainer/TopPart/RightSide/Attacks/Melee
	if button.is_selected:
		show_view(View.UNIT_STATS)
		return
	
	$VBoxContainer/TopPart/RightSide/Attacks/Ranged.set_selected(false)
	if current_view != View.ATTACK_MELEE:
		show_view(View.ATTACK_MELEE)

func _on_Ranged_clicked():
	var button = $VBoxContainer/TopPart/RightSide/Attacks/Ranged
	if button.is_selected:
		show_view(View.UNIT_STATS)
		return
	
	$VBoxContainer/TopPart/RightSide/Attacks/Melee.set_selected(false)
	if current_view != View.ATTACK_RANGED:
		show_view(View.ATTACK_RANGED)

func _on_Melee_hovering(started):
	match started:
		true:
			show_view(View.ATTACK_MELEE)
		false:
			show_view(previous_view)

func _on_Ranged_hovering(started):
	match started:
		true:
			show_view(View.ATTACK_RANGED)
		false:
			show_view(previous_view)

func _on_NavigationBox_clicked():
	show_view(previous_view)
	emit_signal("abilities_closed")


func _on_Text_resized():
	emit_signal("ability_text_resized")
