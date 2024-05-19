extends LineEdit

var prev_value = 0
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_text_changed(new_text):
	var max_value = 3
	if GameData.turn == 1:
		max_value = 20
	var number = float(new_text) if new_text != "" else 0  # Convert text to float, default to 0 if empty
	
	# Validate number range (0 to 20)
	if number < 0 or number > max_value:
		# Reset text to last valid value or empty string
		text = str(prev_value)  # You can also set it to a default value or previous valid value
		set_caret_column(2)
	else:
		prev_value = number
		text = str(number)  # Set the text to the validated number
		set_caret_column(2)
