extends VBoxContainer

func set_name(name: String) -> void:
	$Name/Label.text = name
	$Name/Label/Shadow.text = name

func set_ranged() -> void:
	$Type/Label.text = "Ranged attack"
	$Type/Label/Shadow.text = "Ranged attack"

func set_melee() -> void:
	$Type/Label.text = "Melee attack"
	$Type/Label/Shadow.text = "Melee attack"
