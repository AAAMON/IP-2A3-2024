extends CanvasLayer
var playerInfoRequest = HTTPRequest.new()
var otherPlayersInfoRequest 
var requestCompleted : bool = true
var popupTraitorVisible = false
var popupTreacheryVisible = false
var popupLeadersVisible = false
var popupForcesVisible = false
@onready var timer: Timer = $Timer
@onready var popupFaction1: TextureRect = $AtreidesInformation
@onready var popupFaction2: TextureRect = $BeneGesseritInformation
@onready var popupFaction3: TextureRect = $EmperorInformation
@onready var popupFaction4: TextureRect = $FremenInformation
@onready var popupFaction5: TextureRect = $SpacingGuildInformation
@onready var popupFaction6: TextureRect = $HarkonnenInformation
@onready var winningconditions: TextureRect = $WinningConditions
@onready var winningconditionsfremen: TextureRect = $WinningConditionsFremen
@onready var winningconditionsspacingguild: TextureRect = $WinningConditionsSpacingGuild
# Called when the node enters the scene tree for the first time.
func _ready():

	if (PlayerData.faction == 1):
		PlayerData.myLeaders = PlayerData.leadersAtreides.duplicate()
		get_node("playerHUD/buttonExit15/1").show()
	elif (PlayerData.faction == 2):
		PlayerData.myLeaders = PlayerData.leadersBene.duplicate()
		get_node("playerHUD/buttonExit15/2").show()
	elif (PlayerData.faction == 3):
		PlayerData.myLeaders = PlayerData.leadersEmperor.duplicate()
		get_node("playerHUD/buttonExit15/3").show()
	elif (PlayerData.faction == 4):
		PlayerData.myLeaders = PlayerData.leadersFremen.duplicate()
		get_node("playerHUD/buttonExit15/4").show()
	elif (PlayerData.faction == 5):
		PlayerData.myLeaders = PlayerData.leadersGuild.duplicate()
		get_node("playerHUD/buttonExit15/5").show()
	elif (PlayerData.faction == 6):
		PlayerData.myLeaders = PlayerData.leadersHarkonnen.duplicate()
		get_node("playerHUD/buttonExit15/6").show()
	
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
		#print("Got player data from api")
		PlayerData.faction = json["faction"]
		PlayerData.lastSpice = PlayerData.spice
		PlayerData.spice = json["spice"]
		PlayerData.forcesReserve = json["forcesReserve"]
		PlayerData.forcesDead = json["forcesDead"]
		#PlayerData.forcesDeployed = json["forcesDeployed"]
		PlayerData.treatcheryCards = json["treacheryCards"]
		PlayerData.traitors = json["traitors"]
		PlayerData.myTurn = json["yourTurn"]
		update_hud_player()

func update_hud_other_players():
	#print(OtherPlayersData.otherPlayers)
	var index = 1
	for otherPlayer in OtherPlayersData.otherPlayers:
		
		if (otherPlayer != null && otherPlayer["Faction"] != PlayerData.faction):
			#print(otherPlayer)
			var labelNodeName = "otherPlayersHUD/Player" + str(index) + "/Player" + str(index) + "SpiceBox/Player" + str(index) + "Spice" 
			get_node(labelNodeName).text = str(otherPlayer["Spice"])
			labelNodeName = "otherPlayersHUD/Player" + str(index) + "/Player" + str(index) + "NameBox/Player" + str(index) + "Name" 
			get_node(labelNodeName).text = str(otherPlayer["Username"])
			labelNodeName = "otherPlayersHUD/Player" + str(index) + "/Player" + str(index) + "CardsBox/Player" + str(index) + "Cards" 
			get_node(labelNodeName).text = str(otherPlayer["NrTreatcheryCards"])
			labelNodeName = "otherPlayersHUD/Player" + str(index) + "/Player" + str(index) + "Faction/" + str(otherPlayer["Faction"])
			print(labelNodeName)
			get_node(labelNodeName).show()
			index = index + 1
			
	

func _on_other_players_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		#print("Got other players data from api!")
		OtherPlayersData.otherPlayers = json["otherPlayers"]
		#print(json["otherPlayers"])
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
	
	get_node("popupForces/Traitors").text = 'Forces in reserve: ' + str(PlayerData.forcesReserve) + '\nForces on map: ' + str(PlayerData.forcesDeployed) + '\nForces dead: ' + str(PlayerData.forcesDead)
	
	var playerLeaders = get_node("popupLeaders/Leaders")
	playerLeaders.text = ' '
	for leader in PlayerData.myLeaders:
		playerLeaders.text = playerLeaders.text + leader.name + ' Strength: ' + str(leader.strength) + '\n'
	# cards
	# TODO THE GAME LOGIC SHOULD GIVE THIS, BUT FOR NOW, THEY'RE LETTING US EAT CAKE...
	#var playerLeaders = get_node("popup/Leaders")
	#playerLeaders.text = ' '
	#if (PlayerData.faction == 1):
		#for leader in PlayerData.leadersAtreides:
			#playerLeaders.text = playerLeaders.text + leader.name + ' Strength: ' + str(leader.strength) + '\n'
	
	var treacheryLabel = get_node("popupTreachery/Treachery")
	treacheryLabel.text = ' '
	for card in PlayerData.treatcheryCards:
		if (card != null):
			treacheryLabel.text = treacheryLabel.text + card + "\n"
	if (treacheryLabel.text == ' '):
		treacheryLabel.text = "None"
		
	var traitorsLabel = get_node("popupTraitors/Traitors")
	traitorsLabel.text = ' '
	for traitor in PlayerData.traitors:
		if (traitor != null):
			traitorsLabel.text = traitorsLabel.text + PlayerData.leaders_dict[int(traitor)] + str(traitor) + '\n'  #'\n'
	if (traitorsLabel.text == ' '):
		traitorsLabel.text = "None"
	# TODO THESE NEED TO BE SEPARATED
	get_node("otherPlayersHUD/Status/phase").text = "Phase " + str(GameData.phase)
	get_node("otherPlayersHUD/Status/turn").text = "Phase Moment " + str(GameData.phaseMoment)






func _on_exit_atreides_info_pressed():
	popupFaction1.hide()


func _on_exit_bene_gesserit_info_pressed():
	popupFaction2.hide()


func _on_exit_emperor_info_pressed():
	popupFaction3.hide()


func _on_exit_fremen_info_pressed():
	popupFaction4.hide()



func _on_exit_spacing_guild_info_pressed():
	popupFaction5.hide()
	

func _on_exit_harkonnen_info_pressed():
	popupFaction6.hide()


func _on_exit_winning_conditions_pressed():
	winningconditions.hide()
	
func _on_exit_winning_conditions_fremen_pressed():
	winningconditionsfremen.hide()

func _on_exit_winning_conditions_space_guild_pressed():
	winningconditionsspacingguild.hide()

func _on_button_faction_pressed():
	if(PlayerData.faction == 1):
		popupFaction1.show()
	if (PlayerData.faction == 2):
		popupFaction2.show()
	if (PlayerData.faction == 3):
		popupFaction3.show()
	if (PlayerData.faction == 4):
		popupFaction4.show()
	if (PlayerData.faction == 5):
		popupFaction5.show()
	if (PlayerData.faction == 6):
		popupFaction6.show()


func _on_button_winning_conditions_pressed():
	if (PlayerData.faction == 4):
		winningconditions.show()
	elif (PlayerData.faction == 5):
		winningconditionsfremen.show()
	else:
		winningconditionsspacingguild.show()


func _on_button_traitors_pressed():
	if (popupTraitorVisible == false):
		get_node("popupTraitors").show()
		popupTraitorVisible = true
	else:
		get_node("popupTraitors").hide()
		popupTraitorVisible = false


func _on_button_treachery_pressed():
	if (popupTreacheryVisible == false):
		get_node("popupTreachery").show()
		popupTreacheryVisible = true
	else:
		get_node("popupTreachery").hide()
		popupTreacheryVisible = false


func _on_button_leaders_pressed():
	if (popupLeadersVisible == false):
		get_node("popupLeaders").show()
		popupLeadersVisible = true
	else:
		get_node("popupLeaders").hide()
		popupLeadersVisible = false


func _on_button_forces_pressed():
	if (popupForcesVisible == false):
		get_node("popupForces").show()
		popupForcesVisible = true
	else:
		get_node("popupForces").hide()
		popupForcesVisible = false
