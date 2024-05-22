extends Control
var phase3InfoRequest
@onready var timer: Timer = $Timer
var firstCheck = true

# Called when the node enters the scene tree for the first time.
func _ready():
	# GET PHASE INFO REQUEST ###################################################
	phase3InfoRequest = HTTPRequest.new()
	phase3InfoRequest.connect("request_completed", _on_phase_3_info_request_completed)
	add_child(phase3InfoRequest)
	var error = phase3InfoRequest.request(PlayerData.api_url + "get_phase_3_info")
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_3_INFO")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_timer_timeout():
	var error = phase3InfoRequest.request(PlayerData.api_url + "get_phase_3_info")
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_3_INFO")
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_phase_3_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 3 info")
		# on first request, see if player needs to input anything
		if (firstCheck):
			var phase3Message = get_node("MessageBox/Description")
			phase3Message.text = "You didn't receive spice as charity..."
			for i in range(json["howMany"]):
				var playerId = json["whichPlayers"][i]["turnId"]
				if (playerId == PlayerData.turnId):
					var addedSpice = json["whichPlayers"][i]["addedSpice"]
					phase3Message.text = "You received " + str(addedSpice) + " spice as charity!"
					PlayerData.spice += addedSpice
				# update other
			firstCheck = false
