extends Control

var spice_no = 100

# Called when the node enters the scene tree for the first time.
func _ready():
	var label = get_node("TextureRect/Label")
	label.text = "You received %d spices as charity!" % [spice_no]

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
