extends MarginContainer

export var hint_template: String = "Research needed: "
export var research_template: String = "[R]"
export var research_name: String = "Hardened Matrix"

func _ready() -> void:
	set_research(research_name)
	$Label.text = research_template
	$Label/Shadow.text = research_template

func set_research(name: String) -> void:
	hint_tooltip = hint_template + name + "."
