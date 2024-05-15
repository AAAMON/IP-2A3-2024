extends Control

var spice_no = 100
var city_name = "City_X"

# Called when the node enters the scene tree for the first time.
func _ready():
	var label = get_node("TextureRect/Label")
	label.text = "Territory %s received %d spice!" % [city_name, spice_no]

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
