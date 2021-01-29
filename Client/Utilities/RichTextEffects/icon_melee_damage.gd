tool
extends RichTextEffect
class_name MeleeDamage

# Syntax: [matrix clean=2.0 dirty=1.0 span=50][/matrix]

# Define the tag name.
var bbcode = "matrix"

func _process_custom_fx(char_fx):
	#char_fx.character = "[img=11x11]Client/UI/Icons/icon_melee_attack.png[/img]
	return true
