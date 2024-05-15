extends Control

# Called when the node enters the scene tree for the first time.
func _ready():
	var input = get_node("TextureRect/LineEdit")
	if (GameData.turn == 1):
		var titleLabel = get_node("TextureRect/Label")
		titleLabel.text = "First Storm"
		input.placeholder_text = "Enter a number from 1 to 20"
		input.connect("text_changed", self, "_on_LineEdit_text_changed", [20])
	else:
		input.placeholder_text = "Enter a number from 1 to 3"
		input.connect("text_changed", self, "_on_LineEdit_text_changed", [3])

# Handler for input validation
func _on_LineEdit_text_changed(max_value, new_text):
	var input = get_node("TextureRect/LineEdit")
	var filtered_text = ""
	for c in new_text:
		if c in "0123456789":  # Allow only numeric characters
			filtered_text += c
	if filtered_text != "":
		try:
			var num = int(filtered_text)
			if num <= max_value and num >= 1:
				input.text = str(num)  # Set the text only if it's within the range
			else:
				input.text = ""  # Clear the input if out of range
		except:
			input.text = ""  # Clear the input if conversion fails
	else:
		input.text = ""  # Clear the input if it's not a valid string

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass
