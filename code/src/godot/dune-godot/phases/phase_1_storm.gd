extends Control


# Called when the node enters the scene tree for the first time.
func _ready():
	if (GameData.turn == 1):
		var titleLabel = get_node("TextureRect/Label")
		titleLabel.text = "First Storm"
		var input = get_node("TextureRect/LineEdit")
		input.placeholder_text = "Enter a number from 1 to 20"


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass
