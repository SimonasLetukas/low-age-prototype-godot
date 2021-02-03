extends Control

enum View {UNIT_STATS, STRUCTURE_STATS, ATTACK_MELEE, ATTACK_RANGED, ABILITY}

onready var ability_buttons: AbilityButtons = $Panel/Abilities
onready var display: InfoDisplay = $Panel/InfoDisplay

var is_ability_selected: bool = false
var y_position_for_ability: int = 	280
var y_position_for_unit: int = 		380
var y_position_for_structure: int = 400

var ability_descriptions_by_name: Dictionary # <String (id), String (description)>

func _ready():
	var mocked_id1 = "slave_build"
	var mocked_description1 = "Place a ghostly rendition of a selected enemy unit in [b]7[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] to an unoccupied space in a [b]3[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] from the selected target. The rendition has the same amount of [img=15x11]Client/UI/Icons/icon_health_big.png[/img], [img=15x11]Client/UI/Icons/icon_melee_armour_big.png[/img] and [img=15x11]Client/UI/Icons/icon_ranged_armour_big.png[/img] as the selected target, cannot act, can be attacked and stays for [b]2[/b] action phases. [b]50%[/b] of all [img=15x11]Client/UI/Icons/icon_damage_big.png[/img] done to the rendition is done as pure [img=15x11]Client/UI/Icons/icon_damage_big.png[/img]to the selected target. If the rendition is destroyed before disappearing, the selected target emits a blast which deals [b]10[/b][img=15x11]Client/UI/Icons/icon_melee_attack.png[/img] and slows all adjacent enemies by [b]50%[/b] until the end of their next action."
	var mocked_id2 = "slave_manual_labour"
	var mocked_description2 = "Select an adjacent Hut. At the start of the next planning phase receive +2 Scraps. Maximum of one Slave per Hut."
	ability_descriptions_by_name[mocked_id1] = mocked_description1
	ability_descriptions_by_name[mocked_id2] = mocked_description2
	ability_buttons.populate(ability_descriptions_by_name.keys())

func connect_ability_buttons() -> void:
	var ability_button_instances: Array = ability_buttons.get_children()
	for ability_button in ability_button_instances:
		ability_button = ability_button as AbilityButton
		var id: String = ability_button.id
		ability_button.connect("clicked", self, "_on_AbilityButton_clicked")

func change_display(clicked_ability: AbilityButton) -> void:
	if is_ability_selected:
		var name = clicked_ability.id
		var type = Constants.AbilityType.PLANNING
		var text = ability_descriptions_by_name[clicked_ability.id]
		var research = name
		var cooldown = 1
		var cooldown_type = Constants.AbilityType.ACTION
		display.set_parameters_ability(name, type, text, research, cooldown, cooldown_type)
		display.show_view(View.ABILITY)
	else:
		# TODO set parameters here
		display.show_view(View.UNIT_STATS)

func move_panel() -> void:
	var new_position: Vector2
	if is_ability_selected:
		new_position = Vector2(self.rect_position.x, y_position_for_ability)
	else:
		new_position = Vector2(self.rect_position.x, y_position_for_unit)
	
	$Tween.interpolate_property(self, "rect_position", self.rect_position, new_position, 0.1, Tween.TRANS_QUAD, Tween.EASE_IN_OUT)
	$Tween.start()

func _on_AbilityButton_clicked(ability_button: AbilityButton) -> void:
	if ability_button.is_selected:
		ability_button.set_selected(false)
		is_ability_selected = false
		change_display(null)
		move_panel()
		return
	
	ability_buttons.deselect_all()
	ability_button.set_selected(true)
	is_ability_selected = true
	change_display(ability_button)
	move_panel()

func _on_Abilities_abilities_populated():
	connect_ability_buttons()


func _on_InfoDisplay_abilities_closed():
	ability_buttons.deselect_all()
	is_ability_selected = false
	move_panel()
