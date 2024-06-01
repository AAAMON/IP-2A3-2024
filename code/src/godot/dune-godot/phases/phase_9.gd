extends Control
var phase8InfoRequest
@onready var timer: Timer = $Timer
var firstCheck = true
var requestCompleted: bool = true

# Called when the node enters the scene tree for the first time.
func _ready():
		# GET PHASE INFO REQUEST ###################################################
	phase8InfoRequest = HTTPRequest.new()
	phase8InfoRequest.connect("request_completed", _on_phase_8_info_request_completed)
	add_child(phase8InfoRequest)
	var error = phase8InfoRequest.request(PlayerData.api_url + "get_phase_9_info")
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_9_INFO")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase8InfoRequest.request(PlayerData.api_url + "get_phase_9_info")
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_9_INFO")
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_phase_8_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 8 info")
		# on first request, see if player needs to input anything
		if (firstCheck):
			if (json["someoneWon"] == false):
				get_node("MessageBox/Description").text = "Noone completed winning conditions..."
				# update other
			firstCheck = false
