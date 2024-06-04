extends Control
var factionInputRequest
var lobbyInfoRequest
var requestCompleted: bool = true
var switch = false;
var pickedFaction : int = -1
@onready var timer: Timer = $Timer

func _ready():
	# GET PHASE INFO REQUEST ###################################################
	factionInputRequest = HTTPRequest.new()
	factionInputRequest.connect("request_completed", _on_faction_input_request_completed)
	add_child(factionInputRequest)
	lobbyInfoRequest = HTTPRequest.new()
	lobbyInfoRequest.connect("request_completed", _on_lobby_info_request_completed)
	add_child(lobbyInfoRequest)
	 # Configure and start the timer#############################################
	timer.wait_time = 0.5  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()
	
func _process(delta):
	if (switch):
		var root = get_parent()
		var gameScene = load("res://baseGame/base_game.tscn").instantiate()
		root.add_child(gameScene)
		queue_free()

func _on_timer_timeout():
	if (requestCompleted):
		var error = lobbyInfoRequest.request(PlayerData.api_url + "get_lobby_info")
		if error != OK:
			push_error("ERROR: HTTP: GET_LOBBY_INFO")

func _on_lobby_info_request_completed(_result, _response_code, _headers, body):
	requestCompleted = false
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		#print("Got phase 1 info")
		var lobbyMessage = get_node("MarginContainer/Players")
		if (json["howMany"] < 1):
			lobbyMessage.text = "Dafuk? No players?"
		else:
			lobbyMessage.text = ""
			for player in range(json["howMany"]):
				lobbyMessage.text = lobbyMessage.text + "Username: " + json["players"][player]["username"]
				var factn = json["players"][player]["faction"]
				lobbyMessage.text = lobbyMessage.text + " Faction: " + PlayerData.faction_dict[int(factn)] + '\n'
				if (json['startGame'] == true):
					switch = true
				#print(json["players"][player])
	requestCompleted = true;

func _on_faction_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got pick faction input confirmation")
		# on first request, see if player needs to input anything
		PlayerData.faction = pickedFaction
		if (json["message"] == "ok"):
			print("OK!")
		else:
			print("Faction already picked!")

func _on_return_button_pressed():
	var root = get_parent()
	var menuScene = load("res://mainMenu/menu.tscn").instantiate()
	root.add_child(menuScene)
	queue_free()

func _on_start_button_pressed():
	var playerInfoRequest = HTTPRequest.new()
	playerInfoRequest.connect("request_completed", _on_player_info_request_completed)
	add_child(playerInfoRequest)
	var error = playerInfoRequest.request(PlayerData.api_url + "get_player_data")
	if error != OK:
		push_error("ERROR: HTTP: GET_PLAYER_DATA")

func _on_player_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got player data from api")
		PlayerData.turnId = json["turnId"]
		PlayerData.faction = json["faction"]
		PlayerData.spice = json["spice"]
		PlayerData.forcesReserve = json["forcesReserve"]
		PlayerData.forcesDeployed = json["forcesDeployed"]
		PlayerData.forcesDead = json["forcesDead"]
		PlayerData.treatcheryCards = json["treacheryCards"]
		PlayerData.traitors = json["traitors"]
	# if we switch the scene sooner, the request won't be able to finish
	get_tree().change_scene_to_file("res://baseGame/base_game.tscn")


func _on_atreides_pressed():
	pickedFaction = 1
	var error = factionInputRequest.request(PlayerData.api_url + "pick_faction/" + PlayerData.username + "/" + str(pickedFaction))
	if error != OK:
		push_error("ERROR: HTTP: PICK_FACTION")
func _on_bene_pressed():
	pickedFaction = 2
	var error = factionInputRequest.request(PlayerData.api_url + "pick_faction/" + PlayerData.username + "/" + str(pickedFaction))
	if error != OK:
		push_error("ERROR: HTTP: PICK_FACTION")
func _on_emperor_pressed():
	pickedFaction = 3
	var error = factionInputRequest.request(PlayerData.api_url + "pick_faction/" + PlayerData.username + "/" + str(pickedFaction))
	if error != OK:
		push_error("ERROR: HTTP: PICK_FACTION")
func _on_fremen_pressed():
	pickedFaction = 4
	
	var error = factionInputRequest.request(PlayerData.api_url + "pick_faction/" + PlayerData.username + "/" + str(pickedFaction))
	if error != OK:
		push_error("ERROR: HTTP: PICK_FACTION")
func _on_guild_pressed():
	pickedFaction = 5
	var error = factionInputRequest.request(PlayerData.api_url + "pick_faction/" + PlayerData.username + "/" + str(pickedFaction))
	if error != OK:
		push_error("ERROR: HTTP: PICK_FACTION")
func _on_harkonnen_pressed():
	pickedFaction = 6
	var error = factionInputRequest.request(PlayerData.api_url + "pick_faction/" + PlayerData.username + "/" + str(pickedFaction))
	if error != OK:
		push_error("ERROR: HTTP: PICK_FACTION")
