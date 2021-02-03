extends Control

enum View {UNIT_STATS, STRUCTURE_STATS, ATTACK_MELEE, ATTACK_RANGED, ABILITY}

onready var ability_buttons: AbilityButtons = $Panel/Abilities
onready var display: InfoDisplay = $Panel/InfoDisplay
onready var ability_text_box: RichTextLabel = $Panel/InfoDisplay/VBoxContainer/AbilityDescription/Text

var is_ability_selected: bool = false
var is_switching_between_abilities: bool = false
var biggest_previous_ability_text_size_y: int = 0
var y_size_for_ability: int = 	834
var y_size_for_unit: int = 		784
var y_size_for_structure: int = 816

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
		biggest_previous_ability_text_size_y = 0

func move_panel() -> void:
	var new_size: Vector2
	if is_ability_selected:
		var ability_text_box_size_y: int = calculate_biggest_previous_size(
			ability_text_box.get_size().y)
		var new_y: int = y_size_for_ability - ability_text_box_size_y
		if (new_y > y_size_for_unit): # TODO check if structure is selected
			new_y = y_size_for_unit
		new_size = Vector2(self.rect_size.x, new_y)
	else:
		new_size = Vector2(self.rect_size.x, y_size_for_unit)
	
	$Tween.interpolate_property(self, "rect_size", self.rect_size, new_size, 0.1, Tween.TRANS_QUAD, Tween.EASE_IN_OUT)
	$Tween.start()

func calculate_biggest_previous_size(current_size_y: int = 0) -> int:
	if is_switching_between_abilities:
		if current_size_y > biggest_previous_ability_text_size_y:
			biggest_previous_ability_text_size_y = current_size_y
			return biggest_previous_ability_text_size_y
		else:
			return biggest_previous_ability_text_size_y
	else:
		biggest_previous_ability_text_size_y = current_size_y
		return current_size_y

func _on_AbilityButton_clicked(ability_button: AbilityButton) -> void:
	is_switching_between_abilities = false
	
	if ability_button.is_selected:
		ability_button.set_selected(false)
		is_ability_selected = false
		change_display(null)
		move_panel()
		return
	
	if ability_buttons.is_any_selected():
		is_switching_between_abilities = true
	
	ability_buttons.deselect_all()
	ability_button.set_selected(true)
	is_ability_selected = true
	change_display(ability_button)
	move_panel()

func _on_Abilities_abilities_populated():
	connect_ability_buttons()


func _on_InfoDisplay_abilities_closed():
	is_switching_between_abilities = false
	
	ability_buttons.deselect_all()
	is_ability_selected = false
	change_display(null)
	move_panel()

func _on_InfoDisplay_ability_text_resized():
	move_panel()
