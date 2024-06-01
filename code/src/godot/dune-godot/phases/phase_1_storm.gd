extends Control

var neededInput = false
var phase1InfoRequest
var checkedCondition : bool = false
@onready var timer: Timer = $Timer
# Called when the node enters the scene tree for the first time.
func _ready():
	var input = get_node("InputBox/Input")
	# On first round, the number chosen is from 1 to 20
	if GameData.roundd == 1:
		var titleLabel = get_node("InputBox/Title")
		titleLabel.text = "First Storm"
		input.placeholder_text = "Enter a number from 1 to 20"
	else:
		input.placeholder_text = "Enter a number from 1 to 3"
	# GET PHASE INFO REQUEST ###################################################
	phase1InfoRequest = HTTPRequest.new()
	phase1InfoRequest.connect("request_completed", _on_phase_1_info_request_completed)
	add_child(phase1InfoRequest)
	var error = phase1InfoRequest.request(PlayerData.api_url + "get_phase_1_info")
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_1")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase1InfoRequest.request(PlayerData.api_url + "get_phase_1_info")
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_1_INFO")

func _on_phase_1_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		#print("Got phase 1 info")
		# on first request, see if player needs to input anything
		if (checkedCondition == false):
			if (PlayerData.turnId == json["whichPlayers"][0]["turnId"] || PlayerData.turnId == json["whichPlayers"][1]["turnId"]):
				var inputBox = get_node("InputBox")
				inputBox.show();
		var phase1Message = get_node("MessageBox/Description")
		if (json["newStormPosition"] != -1):
			phase1Message.text = "Storm moved to position " + str(json["newStormPosition"])
			GameData.stormPosition = json["newStormPosition"]
			timer.stop()
		else:
			phase1Message.text = "Players " + str(json["whichPlayers"][0]["turnId"]) + " and " + str(json["whichPlayers"][1]["turnId"])
			phase1Message.text += " are calling storm..."
		
		

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass



func _on_submit_pressed():
	# PHASE INPUT REQUEST ###################################################
	var phase1InputRequest = HTTPRequest.new()
	phase1InputRequest.connect("request_completed", _on_phase_1_input_request_completed)
	add_child(phase1InputRequest)
	var playerInput = get_node("InputBox/Input")
	var error = phase1InputRequest.request(PlayerData.api_url + "phase_1_input/" + playerInput.text)
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
		if (json["response"] == "ok"):
			print("OK!")

