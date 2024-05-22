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
		var player1Label = get_node("otherPlayersHUD/Player1/Player1NameBox/Player1Name")
		var player2Label = get_node("otherPlayersHUD/Player2/Player2NameBox/Player2Name")
		var player3Label = get_node("otherPlayersHUD/Player3/Player3NameBox/Player3Name")
		var player4Label = get_node("otherPlayersHUD/Player4/Player4NameBox/Player4Name")
		var player5Label = get_node("otherPlayersHUD/Player5/Player5NameBox/Player5Name")
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
		# update spice
		player1Label = get_node("otherPlayersHUD/Player1/Player1SpiceBox/Player1Spice")
		player2Label = get_node("otherPlayersHUD/Player2/Player2SpiceBox/Player2Spice")
		player3Label = get_node("otherPlayersHUD/Player3/Player3SpiceBox/Player3Spice")
		player4Label = get_node("otherPlayersHUD/Player4/Player4SpiceBox/Player4Spice")
		player5Label = get_node("otherPlayersHUD/Player5/Player5SpiceBox/Player5Spice")
		player1Label.text = OtherPlayersData.otherPlayers[0]["spice"]
		player2Label.text = OtherPlayersData.otherPlayers[1]["spice"]
		player3Label.text = OtherPlayersData.otherPlayers[2]["spice"]
		player4Label.text = OtherPlayersData.otherPlayers[3]["spice"]
		player5Label.text = OtherPlayersData.otherPlayers[4]["spice"]


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
	var phaseLabel = get_node("otherPlayersHUD/Status/phase")
	phaseLabel.text = "Phase " + str(GameData.phase)
	var turnLabel = get_node("otherPlayersHUD/Status/turn")
	turnLabel.text = "Turn " + str(GameData.turn)
	# ERASE THIS
	var factionLabel = get_node("playerHUD/buttonExit15/faction")
	factionLabel.text = str(PlayerData.faction)
