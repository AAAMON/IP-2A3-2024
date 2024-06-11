extends Control


var phase4InfoRequest
var requestCompleted: bool = true
@onready var timer: Timer = $Timer
@onready var timerCard: Timer = $TimerNewCard
var oldTreacheryCount = [1, 1, 1, 1, 1, 2]
var newCard = false
var lastFaction = "noone"

var lastBid : int = 0
# Called when the node enters the scene tree for the first time.
func _ready():
	# GET PHASE INFO REQUEST ###################################################
	phase4InfoRequest = HTTPRequest.new()
	phase4InfoRequest.connect("request_completed", _on_phase_4_info_request_completed)
	add_child(phase4InfoRequest)
	var error = phase4InfoRequest.request(PlayerData.api_url + "get_phase_4_info/" + PlayerData.username)
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_4")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()
	timerCard.wait_time = 4.0  # 100 milliseconds
	timerCard.connect("timeout", _on_timer_card_timeout)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	#print('Is it my turn' + str(PlayerData.myTurn))
	if (PlayerData.myTurn == true && newCard == false):
		get_node("MessageBox/InputBid").editable = true
		get_node("MessageBox/InputBid").placeholder_text = "Enter Bid."
		get_node("MessageBox/Submit").disabled = false
		get_node("MessageBox/SkipBid").disabled = false
		get_node('MessageBox/LastBid').text = 'Last bid: ' + str(lastBid) + ' spice by ' + lastFaction + '!'
	elif (newCard == false):
		get_node("MessageBox/InputBid").editable = false
		get_node("MessageBox/Submit").disabled = true
		get_node("MessageBox/SkipBid").disabled = true
		get_node("MessageBox/InputBid").placeholder_text = "Not your turn."
		get_node('MessageBox/LastBid').text = 'Last bid: ' + str(lastBid) + ' spice by ' + lastFaction + '!'
	for otherPlayer in OtherPlayersData.otherPlayers:
		if (otherPlayer != null && otherPlayer["NrTreatcheryCards"] != 0 && otherPlayer["NrTreatcheryCards"] != oldTreacheryCount[otherPlayer["Faction"]-1]):
			print('server side ' + str(otherPlayer["NrTreatcheryCards"]) + ' and old ' + str(oldTreacheryCount[otherPlayer["Faction"]-1]) )
			newCard = true
			oldTreacheryCount[otherPlayer["Faction"]-1] = otherPlayer["NrTreatcheryCards"]
			get_node('MessageBox/LastBid').text = otherPlayer["Username"] + ' won the card!'
			get_node("MessageBox/InputBid").editable = false
			get_node("MessageBox/Submit").disabled = true
			get_node("MessageBox/SkipBid").disabled = true
			timerCard.start()
			break;
	for player in range(0,6):
		if (GameData.playersToInput[player] == true):
			get_node("MessageBox/WhoIsBidding").text = PlayerData.faction_dict[player+1] + ' is bidding...'
	
	
func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase4InfoRequest.request(PlayerData.api_url + "get_phase_4_info/" + PlayerData.username)
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_4_INFO")

func _on_timer_card_timeout():
	get_node('MessageBox/LastBid').text = 'New card being bidded!'
	newCard = false;
	print("card timeout")
	timerCard.stop()
	
func _on_phase_4_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		#print("Got phase 4 info")

		#print(json)
		if (json["lastBid"]["bid"] == 0):
			lastBid = json["lastBid"]["bid"]
			lastFaction = "noone"
		elif (lastBid != json["lastBid"]["bid"]):
			get_node('MessageBox/LastBid').text = 'Last bid: ' + str(json["lastBid"]["bid"]) + ' spice by ' + json["lastBid"]["faction"][0] + '!'
			lastBid = json["lastBid"]["bid"]
			lastFaction = json["lastBid"]["faction"][0]



func _on_submit_pressed():
	# PHASE INPUT REQUEST ###################################################
	var phase4InputRequest = HTTPRequest.new()
	phase4InputRequest.connect("request_completed", _on_phase_4_input_request_completed)
	add_child(phase4InputRequest)
	var playerInput = get_node('MessageBox/InputBid')
	var error = phase4InputRequest.request(PlayerData.api_url + "phase_4_input/" + PlayerData.username + '/' + playerInput.text)
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
		if (json["message"] == "ok"):
			print("OK!")


func _on_skip_bid_pressed():
	# PHASE INPUT REQUEST ###################################################
	var phase4InputRequest = HTTPRequest.new()
	phase4InputRequest.connect("request_completed", _on_phase_4_input_request_completed)
	add_child(phase4InputRequest)
	var playerInput = get_node('MessageBox/InputBid')
	var error = phase4InputRequest.request(PlayerData.api_url + "phase_4_input/" + PlayerData.username + '/pass')
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_4_INPUT")
