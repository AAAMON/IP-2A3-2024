extends Control

var phase6InfoRequest
var requestCompleted: bool = true
@onready var timer: Timer = $Timer
# Called when the node enters the scene tree for the first time.
func _ready():
	# GET PHASE INFO REQUEST ###################################################
	phase6InfoRequest = HTTPRequest.new()
	phase6InfoRequest.connect("request_completed", _on_phase_6_info_request_completed)
	add_child(phase6InfoRequest)
	var error = phase6InfoRequest.request(PlayerData.api_url + "get_phase_6_info")
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_5")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase6InfoRequest.request(PlayerData.api_url + "get_phase_5_info")
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_5_INFO")

func _on_phase_6_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 5 info")
		# no need to input if we cantt revive shit
		if (json["forcesToRevive"] == 0 && json["howManyLeadersToRevive"] == 0):
			get_node('MessageBox/Description').text = 'No forces or leaders to revive!'

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_skip_pressed():
	pass # Replace with function body.
