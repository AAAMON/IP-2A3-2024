extends TextureRect


var _popped_up = false


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass


func _on_button_forces_pressed():
	if !_popped_up:
		set_position(Vector2(0, 305), true)
		print("open")
	else:
		set_position(Vector2(0, 610), true)
		print("close");
	_popped_up = !_popped_up
