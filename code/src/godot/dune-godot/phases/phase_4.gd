extends Control


var neededInput = false
var phase4InfoRequest
var requestCompleted: bool = true
@onready var timer: Timer = $Timer

var cardIdBeingBidded
var playerIdLastBidded
var playerIdCurrentlyBidding
var playerIdStartedBid
var lastBidValue
# Called when the node enters the scene tree for the first time.
func _ready():
	# GET PHASE INFO REQUEST ###################################################
	phase4InfoRequest = HTTPRequest.new()
	phase4InfoRequest.connect("request_completed", _on_phase_4_info_request_completed)
	add_child(phase4InfoRequest)
	var error = phase4InfoRequest.request(PlayerData.api_url + "get_phase_4_info")
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_4")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
	
func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase4InfoRequest.request(PlayerData.api_url + "get_phase_4_info")
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_4_INFO")
	
func _on_phase_4_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 4 info")
		# on first request, see if player needs to input anything
		get_node('MessageBox/LastBid').text = 'Last bid: ' + str(json["lastBid"]) + ' spice by ' + str(json["lastBidder"])
		# noi trb sa trimitem date
		#print(json)
		# need to show the winner
		
		if (json["bidStopped"] == true):
			get_node('MessageBox/LastBid').text = 'Bid won by: ' + str(json["lastBid"]) + ' spice by ' + str(json["lastBidder"])
		elif (json["lastBidder"] + 1 == PlayerData.turnId || json["lastBidder"] - 5 == PlayerData.turnId):
			get_node('MessageBox/Submit').disabled = false;



func _on_submit_pressed():
	# PHASE INPUT REQUEST ###################################################
	var phase4InputRequest = HTTPRequest.new()
	phase4InputRequest.connect("request_completed", _on_phase_4_input_request_completed)
	add_child(phase4InputRequest)
	var playerInput = get_node('MessageBox/InputBid')
	var error = phase4InputRequest.request(PlayerData.api_url + "phase_4_input/" + playerInput.text)
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_4_INPUT")

func _on_phase_4_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got phase 4 input confirmation")
		# on first request, see if player needs to input anything
		if (json["response"] == "ok"):
			print("OK!")
