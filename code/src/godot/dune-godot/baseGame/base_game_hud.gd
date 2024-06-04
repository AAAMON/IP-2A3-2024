extends CanvasLayer
var playerInfoRequest = HTTPRequest.new()
	


# Called when the node enters the scene tree for the first time.
func _ready():
	var playerLeaders = get_node("popup/Leaders")
	playerLeaders.text = ' '
	if (PlayerData.faction == 1):
		PlayerData.myLeaders = PlayerData.leadersAtreides.duplicate()
	elif (PlayerData.faction == 2):
		PlayerData.myLeaders = PlayerData.leadersBene.duplicate()
	elif (PlayerData.faction == 3):
		PlayerData.myLeaders = PlayerData.leadersEmperor.duplicate()
	elif (PlayerData.faction == 4):
		PlayerData.myLeaders = PlayerData.leadersFremen.duplicate()
	elif (PlayerData.faction == 5):
		PlayerData.myLeaders = PlayerData.leadersGuild.duplicate()
	elif (PlayerData.faction == 6):
		PlayerData.myLeaders = PlayerData.leadersHarkonnen.duplicate()
	for leader in PlayerData.myLeaders:
		playerLeaders.text = playerLeaders.text + leader.name + ' Strength: ' + str(leader.strength) + '\n'
	var otherPlayersInfoRequest = HTTPRequest.new()
	otherPlayersInfoRequest.connect("request_completed", _on_other_players_info_request_completed)
	add_child(otherPlayersInfoRequest)
	playerInfoRequest.connect("request_completed", _on_player_info_request_completed)
	add_child(playerInfoRequest)
	var error = playerInfoRequest.request(PlayerData.api_url + "get_player_data/" + PlayerData.username)
	if error != OK:
		push_error("ERROR: HTTP: GET_PLAYER_DATA")

	#var error = otherPlayersInfoRequest.request(PlayerData.api_url + "get_other_players_data")
	#if error != OK:
		#push_error("ERROR: HTTP: GET_PLAYER_DATA")

func _on_player_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got player data from api")
		PlayerData.faction = json["faction"]
		PlayerData.spice = json["spice"]
		PlayerData.forcesReserve = json["forcesReserve"]
		PlayerData.forcesDeployed = json["forcesDeployed"]
		PlayerData.forcesDead = json["forcesDead"]
		PlayerData.treatcheryCards = json["treacheryCards"]
		PlayerData.traitors = json["traitors"]

func _on_other_players_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got other players data from api!")
		var place : int = 0;
		for i in range(PlayerData.turnId-1, 5):
			OtherPlayersData.otherPlayers.append(json[i])
			place = place + 1
		for i in range(0, PlayerData.turnId-1):
			OtherPlayersData.otherPlayers.append(json[i])
			place = place + 1
		#var player1Label = get_node("otherPlayersHUD/Player1/Player1NameBox/Player1Name")
		#var player2Label = get_node("otherPlayersHUD/Player2/Player2NameBox/Player2Name")
		#var player3Label = get_node("otherPlayersHUD/Player3/Player3NameBox/Player3Name")
		#var player4Label = get_node("otherPlayersHUD/Player4/Player4NameBox/Player4Name")
		#var player5Label = get_node("otherPlayersHUD/Player5/Player5NameBox/Player5Name")
		#player1Label.text = OtherPlayersData.otherPlayers[0]["username"]
		#player1Label.text = player1Label.text + " t:" + str(OtherPlayersData.otherPlayers[0]["turnId"])
		#player2Label.text = OtherPlayersData.otherPlayers[1]["username"]
		#player2Label.text = player2Label.text + " t:" + str(OtherPlayersData.otherPlayers[1]["turnId"])
		#player3Label.text = OtherPlayersData.otherPlayers[2]["username"]
		#player3Label.text = player3Label.text + " t:" + str(OtherPlayersData.otherPlayers[2]["turnId"])
		#player4Label.text = OtherPlayersData.otherPlayers[3]["username"]
		#player4Label.text = player4Label.text + " t:" + str(OtherPlayersData.otherPlayers[3]["turnId"])
		#player5Label.text = OtherPlayersData.otherPlayers[4]["username"]
		#player5Label.text = player5Label.text + " t:" + str(OtherPlayersData.otherPlayers[4]["turnId"])
		## update spice
		#player1Label = get_node("otherPlayersHUD/Player1/Player1SpiceBox/Player1Spice")
		#player2Label = get_node("otherPlayersHUD/Player2/Player2SpiceBox/Player2Spice")
		#player3Label = get_node("otherPlayersHUD/Player3/Player3SpiceBox/Player3Spice")
		#player4Label = get_node("otherPlayersHUD/Player4/Player4SpiceBox/Player4Spice")
		#player5Label = get_node("otherPlayersHUD/Player5/Player5SpiceBox/Player5Spice")
		#player1Label.text = OtherPlayersData.otherPlayers[0]["spice"]
		#player2Label.text = OtherPlayersData.otherPlayers[1]["spice"]
		#player3Label.text = OtherPlayersData.otherPlayers[2]["spice"]
		#player4Label.text = OtherPlayersData.otherPlayers[3]["spice"]
		#player5Label.text = OtherPlayersData.otherPlayers[4]["spice"]


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	update_hud_player()

# Updates information from HUD (player section) with information from PlayerData singleton.
func update_hud_player():
	# basic info
	get_node("playerHUD/buttonExit4/username").text = PlayerData.username
	get_node("playerHUD/buttonExit8/spice").text = str(PlayerData.spice)
	get_node("playerHUD/buttonForces/forces").text = str(PlayerData.forcesReserve) + "R " + str(PlayerData.forcesDeployed) + "M " + str(PlayerData.forcesDead) + "D "
	get_node("playerHUD/buttonExit15/faction").text = str(PlayerData.faction)
	
	# cards
	# TODO THE GAME LOGIC SHOULD GIVE THIS, BUT FOR NOW, THEY'RE LETTING US EAT CAKE...
	#var playerLeaders = get_node("popup/Leaders")
	#playerLeaders.text = ' '
	#if (PlayerData.faction == 1):
		#for leader in PlayerData.leadersAtreides:
			#playerLeaders.text = playerLeaders.text + leader.name + ' Strength: ' + str(leader.strength) + '\n'
	
	var treacheryLabel = get_node("playerHUD/buttonExit13/TreacheryCards")
	treacheryLabel.text = ' '
	for card in PlayerData.treatcheryCards:
		if (card != null):
			for howMany in range(card["Count"]):
				treacheryLabel.text = treacheryLabel.text + card["Name"] + "\n"
	if (treacheryLabel.text == ' '):
		treacheryLabel.text = "None"
		
	var traitorsLabel = get_node("playerHUD/buttonExit14/TraitorCards")
	traitorsLabel.text = ' '
	for traitor in PlayerData.traitors:
		if (traitor != null):
			traitorsLabel.text = traitorsLabel.text + traitor["Name"] + ' ' + str(traitor["Strength"]) + '\n'
	if (traitorsLabel.text == ' '):
		traitorsLabel.text = "None"
	# TODO THESE NEED TO BE SEPARATED
	get_node("otherPlayersHUD/Status/phase").text = "Phase " + str(GameData.phase)
	get_node("otherPlayersHUD/Status/turn").text = "Phase Moment " + str(GameData.phaseMoment)
