extends Control
var phase2InfoRequest
@onready var timer: Timer = $Timer
var firstCheck = true
var requestCompleted: bool = true
# Called when the node enters the scene tree for the first time.
func _ready():
	# GET PHASE INFO REQUEST ###################################################
	phase2InfoRequest = HTTPRequest.new()
	phase2InfoRequest.connect("request_completed", _on_phase_2_info_request_completed)
	add_child(phase2InfoRequest)
	var error = phase2InfoRequest.request(PlayerData.api_url + "get_phase_2_info/" + PlayerData.username)
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_2_INFO")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase2InfoRequest.request(PlayerData.api_url + "get_phase_2_info/" + PlayerData.username)
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_2_INFO")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
	
func _on_phase_2_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 2 info")
		# on first request, see if player needs to input anything
		if (firstCheck):
			var phase2Message = get_node("MessageBox/Description")
			print(json)
			if (json["lastSpiceCard"][0]["type"] == "Territory_Card"):

				phase2Message.text = "Spice added to " + MapData.territory_dict[MapData.sections_goofy_dict[ MapData.spice_goofy_to_section_dict[int(json["lastSpiceCard"][0]["Section_Position_In_List"])] ]]["origin_sector"] + "!"
			#mapspice gets update automatically
			firstCheck = false
	requestCompleted = true

