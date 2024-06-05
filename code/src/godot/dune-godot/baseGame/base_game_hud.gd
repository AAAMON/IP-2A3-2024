extends CanvasLayer
var playerInfoRequest = HTTPRequest.new()
var otherPlayersInfoRequest 
var requestCompleted : bool = true
@onready var timer: Timer = $Timer

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
	otherPlayersInfoRequest = HTTPRequest.new()
	otherPlayersInfoRequest.connect("request_completed", _on_other_players_info_request_completed)
	add_child(otherPlayersInfoRequest)
	playerInfoRequest.connect("request_completed", _on_player_info_request_completed)
	add_child(playerInfoRequest)
	var error = playerInfoRequest.request(PlayerData.api_url + "get_player_data/" + PlayerData.username)
	if error != OK:
		push_error("ERROR: HTTP: GET_PLAYER_DATA")
	error = otherPlayersInfoRequest.request(PlayerData.api_url + "get_other_players_data/" + PlayerData.username)
	if error != OK:
		push_error("ERROR: HTTP: GET_PLAYER_DATA")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.9  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_timer_timeout():
	if (requestCompleted):
		requestCompleted = false;
		var error = playerInfoRequest.request(PlayerData.api_url + "get_player_data/" + PlayerData.username)
		if error != OK:
			push_error("ERROR: HTTP: GET_PLAYER_DATA")
		error = otherPlayersInfoRequest.request(PlayerData.api_url + "get_other_players_data/" + PlayerData.username)
		if error != OK:
			push_error("ERROR: HTTP: GET_PLAYER_DATA")
		requestCompleted = true;

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
		PlayerData.forcesDead = json["forcesDead"]
		PlayerData.treatcheryCards = json["treacheryCards"]
		PlayerData.traitors = json["traitors"]
		PlayerData.myTurn = json["yourTurn"]
		update_hud_player()

func update_hud_other_players():
	#print(OtherPlayersData.otherPlayers)
	var index = 1
	for otherPlayer in OtherPlayersData.otherPlayers:
		
		if (otherPlayer != null && otherPlayer["turnId"] != PlayerData.turnId):
			#print(otherPlayer)
			var labelNodeName = "otherPlayersHUD/Player" + str(index) + "/Player" + str(index) + "SpiceBox/Player" + str(index) + "Spice" 
			get_node(labelNodeName).text = str(otherPlayer["Spice"])
			labelNodeName = "otherPlayersHUD/Player" + str(index) + "/Player" + str(index) + "NameBox/Player" + str(index) + "Name" 
			get_node(labelNodeName).text = otherPlayer["Username"]
			labelNodeName = "otherPlayersHUD/Player" + str(index) + "/Player" + str(index) + "CardsBox/Player" + str(index) + "Cards" 
			get_node(labelNodeName).text = str(otherPlayer["NrTreatcheryCards"])
		index = index + 1
	

func _on_other_players_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got other players data from api!")
		OtherPlayersData.otherPlayers = json["otherPlayers"]
		update_hud_other_players()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass;

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
	
	#var treacheryLabel = get_node("playerHUD/buttonExit13/TreacheryCards")
	#treacheryLabel.text = ' '
	#for card in PlayerData.treatcheryCards:
		#if (card != null):
			#for howMany in range(card["Count"]):
				#treacheryLabel.text = treacheryLabel.text + card["Name"] + "\n"
	#if (treacheryLabel.text == ' '):
		#treacheryLabel.text = "None"
		
	var traitorsLabel = get_node("playerHUD/buttonExit14/TraitorCards")
	traitorsLabel.text = ' '
	for traitor in PlayerData.traitors:
		if (traitor != null):
			traitorsLabel.text = traitorsLabel.text + str(traitor) + ' '  #'\n'
	if (traitorsLabel.text == ' '):
		traitorsLabel.text = "None"
	# TODO THESE NEED TO BE SEPARATED
	get_node("otherPlayersHUD/Status/phase").text = "Phase " + str(GameData.phase)
	get_node("otherPlayersHUD/Status/turn").text = "Phase Moment " + str(GameData.phaseMoment)
