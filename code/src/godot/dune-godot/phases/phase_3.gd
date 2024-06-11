extends Control
var phase3InfoRequest
@onready var timer: Timer = $Timer
var firstCheck = true

# Called when the node enters the scene tree for the first time.
func _ready():
	pass;

func _process(delta):
	if (PlayerData.spice != PlayerData.lastSpice):
		get_node("MessageBox/Description").text = "You got " + str(PlayerData.spice-PlayerData.lastSpice) + " spice charity!"
	else:
		get_node("MessageBox/Description").text = "You did not receive charity."

