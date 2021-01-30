extends MarginContainer

export var types: PoolIntArray = [Constants.EntityType.LIGHT, Constants.EntityType.BIOLOGICAL]

var separator: String = " - "

func _ready():
	set_types(types)

func set_types(types: PoolIntArray) -> void:
	var count: int = 0
	$Types.text = ""
	$Types/Shadow.text = ""
	
	for type in types:
		assert(type in Constants.EntityType.values(), "Type is of enum type 'EntityType'")
		if count == 0:
			$Types.text += Constants.EntityType.keys()[type].capitalize()
			$Types/Shadow.text += Constants.EntityType.keys()[type].capitalize()
		else:
			$Types.text += separator + Constants.EntityType.keys()[type].capitalize()
			$Types/Shadow.text += separator + Constants.EntityType.keys()[type].capitalize()
		count += 1
