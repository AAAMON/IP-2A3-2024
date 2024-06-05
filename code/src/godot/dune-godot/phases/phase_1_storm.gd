extends Control

var neededInput = false
var phase1InfoRequest
var checkedCondition : bool = false
@onready var timer: Timer = $Timer
# Called when the node enters the scene tree for the first time.
func _ready():
	var input = get_node("InputBox/Input")
	# On first round, the number chosen is from 1 to 20
	if (PlayerData.myTurn == true):
		get_node("InputBox").show()
		if (GameData.roundd == 0):
			var titleLabel = get_node("InputBox/Title")
			titleLabel.text = "First Storm"
			input.placeholder_text = "Enter a number from 1 to 20"
		else:
			input.placeholder_text = "Enter a number from 1 to 3"
	else:
		get_node("InputBox").hide()



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if (PlayerData.myTurn == true):
		get_node("InputBox").show()
	else:
		get_node("InputBox").hide()



func _on_submit_pressed():
	# PHASE INPUT REQUEST ###################################################
	var phase1InputRequest = HTTPRequest.new()
	phase1InputRequest.connect("request_completed", _on_phase_1_input_request_completed)
	add_child(phase1InputRequest)
	var playerInput = get_node("InputBox/Input")
	var error = phase1InputRequest.request(PlayerData.api_url + "phase_1_input/" + PlayerData.username + "/" + playerInput.text)
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_1_INFO")

func _on_phase_1_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 1 input confirmation")
		# on first request, see if player needs to input anything
		if (json["message"] == "ok"):
			print("OK!")

