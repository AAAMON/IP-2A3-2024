extends Control

var neededInput = true
var phase5InfoRequest
var phase5InputRequest
var requestCompleted: bool = true
@onready var timer: Timer = $Timer
# Called when the node enters the scene tree for the first time.
func _ready():
	# GET PHASE INFO REQUEST ###################################################
	phase5InfoRequest = HTTPRequest.new()
	phase5InfoRequest.connect("request_completed", _on_phase_5_info_request_completed)
	add_child(phase5InfoRequest)
	phase5InputRequest = HTTPRequest.new()
	phase5InputRequest.connect("request_completed", _on_phase_5_input_request_completed)
	add_child(phase5InputRequest)
	var error = phase5InfoRequest.request(PlayerData.api_url + "get_phase_5_info/" + PlayerData.username)
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_5")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase5InfoRequest.request(PlayerData.api_url + "get_phase_5_info/" + PlayerData.username)
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_5_INFO")

func _on_phase_5_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 5 info")
		print(json["revivableGenerals"])
		# no need to input if we cantt revive shit
		if (json["forces"] == 0 && json["revivableGenerals"] == null):
			get_node('MessageBox/Description').text = 'No forces or leaders to revive!'
			if (neededInput == true):
				var error = phase5InputRequest.request(PlayerData.api_url + "phase_5_input/" + PlayerData.username + '/pass');
				if error != OK:
					push_error("ERROR: HTTP: GET_PHASE_5_INFO")
				neededInput = false
		elif (json["forces"] == 0):
			get_node('MessageBox/Description').text = 'Leaders to revive!'
		elif (json["revivableGenerals"] == null):
			get_node('MessageBox/Description').text = 'Forces to revive!'
		else:
			get_node('MessageBox/Description').text = 'Forces and leaders to revive!'
		
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_submit_forces_pressed():
	var playerInput = get_node("InputBox/Input")
	var error = phase5InputRequest.request(PlayerData.api_url + "phase_5_input/" + PlayerData.username + '/' + playerInput.text )
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_5_INFO")


func _on_submit_leader_pressed():
	var playerInput = get_node("InputBox/Input")
	var error = phase5InputRequest.request(PlayerData.api_url + "phase_5_input/" + PlayerData.username + '/' + PlayerData.leaders_dict[int(playerInput.text)] )
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_5_INFO")

func _on_phase_5_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got shipment input confirmation")
		# on first request, see if player needs to input anything
		if (json["message"] == "ok"):
			print("OK!")
