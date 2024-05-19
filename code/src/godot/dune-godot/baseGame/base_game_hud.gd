extends CanvasLayer

# Called when the node enters the scene tree for the first time.
func _ready():
	var otherPlayersInfoRequest = HTTPRequest.new()
	otherPlayersInfoRequest.connect("request_completed", _on_other_players_info_request_completed)
	add_child(otherPlayersInfoRequest)
	var error = otherPlayersInfoRequest.request(PlayerData.api_url + "get_other_players_data")
	if error != OK:
		push_error("ERROR: HTTP: GET_PLAYER_DATA")


func _on_other_players_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got other players data from api!")
		var place : int = 0;
		for i in range(PlayerData.turnId-1, 5):
			OtherPlayersData.otherPlayers[place] = json[i]
			place = place + 1
		for i in range(0, PlayerData.turnId-1):
			OtherPlayersData.otherPlayers[place] = json[i]
			place = place + 1
		var player1Label = get_node("otherPlayersHUD/TextureButton24/player1")
		var player2Label = get_node("otherPlayersHUD/TextureButton20/player2")
		var player3Label = get_node("otherPlayersHUD/TextureButton11/player3")
		var player4Label = get_node("otherPlayersHUD/TextureButton14/player4")
		var player5Label = get_node("otherPlayersHUD/TextureButton16/player5")
		player1Label.text = OtherPlayersData.otherPlayers[0]["username"]
		player1Label.text = player1Label.text + " t:" + str(OtherPlayersData.otherPlayers[0]["turnId"])
		player2Label.text = OtherPlayersData.otherPlayers[1]["username"]
		player2Label.text = player2Label.text + " t:" + str(OtherPlayersData.otherPlayers[1]["turnId"])
		player3Label.text = OtherPlayersData.otherPlayers[2]["username"]
		player3Label.text = player3Label.text + " t:" + str(OtherPlayersData.otherPlayers[2]["turnId"])
		player4Label.text = OtherPlayersData.otherPlayers[3]["username"]
		player4Label.text = player4Label.text + " t:" + str(OtherPlayersData.otherPlayers[3]["turnId"])
		player5Label.text = OtherPlayersData.otherPlayers[4]["username"]
		player5Label.text = player5Label.text + " t:" + str(OtherPlayersData.otherPlayers[4]["turnId"])

		
		#PlayerData.faction = json["faction"]
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	#make this hoe a function updatePlayerHud()
	# and move it to completed requests only
	var playerName = get_node("playerHUD/buttonExit4/username")
	playerName.text = PlayerData.username
	var playerSpice = get_node("playerHUD/buttonExit8/spice")
	playerSpice.text = str(PlayerData.spice)
	var playerForces = get_node("playerHUD/buttonForces/forces")
	playerForces.text = str(PlayerData.forcesReserve) + "R " + str(PlayerData.forcesDeployed) + "M " + str(PlayerData.forcesDead) + "D "
	#var playerLeaders = get_node("playerHUD/buttonExit7/leaders")
	#playerLeaders.text = str(PlayerData.leaders)
	var phaseLabel = get_node("otherPlayersHUD/TextureButton6/phase")
	phaseLabel.text = "Phase " + str(GameData.phase)
	var turnLabel = get_node("otherPlayersHUD/TextureButton6/turn")
	turnLabel.text = "Turn " + str(GameData.turn)
	# ERASE THIS
	var factionLabel = get_node("playerHUD/buttonExit15/faction")
	factionLabel.text = str(PlayerData.faction)


func _on_deleteme_2_pressed():
	if (GameData.phase + 1 == 10):
		GameData.phase  = 1
		GameData.turn = 2
	else:
		GameData.phase = GameData.phase + 1
