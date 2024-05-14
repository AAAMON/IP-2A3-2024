extends Control

# Called when the node enters the scene tree for the first time.
func _ready():
	var input = get_node("TextureRect/SpinBox")
	input.max_value = PlayerData.forcesDead

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
