extends Control
var firstCheck = true


# Called when the node enters the scene tree for the first time.
func _ready():
	pass;
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if (PlayerData.lastSpice != PlayerData.spice):
		get_node("MessageBox/Description").text = 'You received ' + str(PlayerData.spice-PlayerData.lastSpice) + ' spice!'
	else:
		get_node("MessageBox/Description").text = 'You received no spice...'

