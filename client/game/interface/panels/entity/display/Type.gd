extends MarginContainer

func set_type(ability_type: int, cooldown: int = 0, cooldown_type: int = 0) -> void:
	var new_text: String = Constants.AbilityType.keys()[ability_type].capitalize()
	
	if cooldown > 0:
		new_text += " (cooldown: {value} {type} phases)".format({
			"value": cooldown as String, 
			"type": Constants.AbilityType.keys()[cooldown_type].to_lower()})
	
	$Label.text = new_text
	$Label/Shadow.text = new_text
