extends Control
var requestCompleted: bool = true
var pickEnemyInputRequest
var battleInputRequest
var phase7InfoRequest
@onready var timer: Timer = $Timer

var battleSections = null
var currentBattleSection = null
var enemy = null
var aggressor = null
var selectedSection = null
var selectedFaction  : String = ''
var buttonsCreated = false
var selectedLeader = null
var selectedDefense = null
var selectedOffense = null

# Called when the node enters the scene tree for the first time.
func _ready():
	pickEnemyInputRequest = HTTPRequest.new()
	pickEnemyInputRequest.connect("request_completed", _on_pick_enemy_input_request_completed)
	add_child(pickEnemyInputRequest)
	battleInputRequest = HTTPRequest.new()
	battleInputRequest.connect("request_completed", _on_battle_input_request_completed)
	add_child(battleInputRequest)
	phase7InfoRequest = HTTPRequest.new()
	phase7InfoRequest.connect("request_completed", _on_phase_7_input_request_completed)
	add_child(phase7InfoRequest)
	var error = phase7InfoRequest.request(PlayerData.api_url + "get_phase_7_info/" + PlayerData.username)
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_7")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.3  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()

func _on_pick_enemy_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got enemy pick input confirmation")
		# on first request, see if player needs to input anything
		if (json["message"] == "ok"):
			print("OK!")

func _on_battle_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got enemy pick input confirmation")
		# on first request, see if player needs to input anything
		if (json["message"] == "ok"):
			print("OK!")

func _on_timer_timeout():
	if (PlayerData.requestCompleted):
		var error = phase7InfoRequest.request(PlayerData.api_url + "get_phase_7_info/" + PlayerData.username)
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_7_INFO")

func _on_phase_7_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		#print("Got phase 7 info")
		
		battleSections = json["whereAreBattles"]
		#print(battleSections)
		currentBattleSection = json["currentBattleSection"]
		enemy = json["enemy"]
		aggressor = json["agressor"]

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if (GameData.phaseMoment == "choosing battle"):
		selectedLeader = null
		selectedDefense = null
		selectedOffense = null
		if (buttonsCreated == true):
			for child in get_node("BattleBox/VBoxContainer/HBoxLeaders").get_children():
				if child is Button:
					child.queue_free()
			for child in get_node("BattleBox/VBoxContainer/HBoxOffense").get_children():
				if child is Button:
					child.queue_free()
			for child in get_node("BattleBox/VBoxContainer/HBoxDefense").get_children():
				if child is Button:
					child.queue_free()
			buttonsCreated == false
	if (PlayerData.myTurn == true && GameData.phaseMoment == "choosing battle" && selectedSection == null):
		#get_node("EnemyBox/Description").text = "Pick location to battle: "
		var mes = "Pick location to battle: "
		if (battleSections != null):
			for section in battleSections:
				mes = mes + MapData.sections_goofy_dict[int(section)] + ' '
			get_node("EnemyBox/Description").text = mes
	#if (PlayerData.myTurn == true && GameData.phaseMoment == "choosing battle"):
		#delete children of buttons
	if (PlayerData.myTurn == true && GameData.phaseMoment == "Battle Wheel" && buttonsCreated == false):
		for card in PlayerData.treatcheryCards:
			if (card != null):
				if (PlayerData.treachery[card].type == "leader"):
					var button = Button.new()
					button.text = card
					get_node("BattleBox/VBoxContainer/HBoxLeaders").add_child(button)
					button.connect("pressed", _on_leader_card_picked.bind(button))
				elif (PlayerData.treachery[card].mainType == "weapon"):
					var button = Button.new()
					button.text = card
					get_node("BattleBox/VBoxContainer/HBoxOffense").add_child(button)
					button.connect("pressed", _on_offense_card_picked.bind(button))
				elif (PlayerData.treachery[card].mainType == "defense"):
					var button = Button.new()
					button.text = card
					get_node("BattleBox/VBoxContainer/HBoxDefense").add_child(button)
					button.connect("pressed", _on_defense_card_picked.bind(button))
		# add leaders
		for leader in PlayerData.myLeaders:
			var button = Button.new()
			button.text = PlayerData.leaders_dict[leader.idForApi]
			get_node("BattleBox/VBoxContainer/HBoxLeaders").add_child(button)
			button.connect("pressed", _on_leader_card_picked.bind(button))
		buttonsCreated = true;
			# make buttons
	if (PlayerData.myTurn == false):
		get_node("MessageBox").show()
		get_node("MessageBox/Description").text = "Battles taking place..."

func _on_leader_card_picked(button):
	selectedLeader = button.text
func _on_offense_card_picked(button):
	selectedOffense = button.text
func _on_defense_card_picked(button):
	selectedDefense = button.text

func _on_atreides_picked():
	selectedFaction = '1'
func _on_bene_picked():
	selectedFaction = '2'
func _on_emperor_picked():
	selectedFaction = '3'
func _on_fremen_picked():
	selectedFaction = '4'
func _on_spacing_picked():
	selectedFaction = '5'
func _on_harkonnen_picked():
	selectedFaction = '6'

func _on_submit_pressed():
	if (selectedSection == null && GameData.phaseMoment == "choosing battle" ):
		selectedSection = MapData.selectedRegion
		var mes = "Pick faction to battle: "
		var factionsToBattle = []
		# find factions from each section
		#print(MapData.forces)
		for force in MapData.forces:
			if (force != null && GameData.phaseMoment == "choosing battle" ):
				if (MapData.sections_goofy_dict[int(force["GoofySectionId"])] == selectedSection.region_name):
					factionsToBattle.append(force["Faction"])
		if (factionsToBattle != null && GameData.phaseMoment == "choosing battle" ):
			for faction in factionsToBattle:
				if (faction != PlayerData.faction_to_goofy_dict[PlayerData.faction_dict[PlayerData.faction]]):
					mes = mes + str(faction) + ' '
					var button = Button.new()
					button.text = str(faction)
					get_node("EnemyBox/HBoxContainer").add_child(button)
					if (faction == "Atreides"):
						button.connect("pressed", _on_atreides_picked)
					if (faction == "Bene_Gesserit"):
						button.connect("pressed", _on_bene_picked)
					if (faction == "Emperor"):
						button.connect("pressed", _on_emperor_picked)
					if (faction == "Fremen"):
						button.connect("pressed", _on_fremen_picked)
					if (faction == "Spacing_Guild"):
						button.connect("pressed", _on_spacing_picked)
					if (faction == "Harkonnen"):
						button.connect("pressed", _on_harkonnen_picked)
			get_node("EnemyBox/Description").text = mes
	elif (selectedSection != null && selectedFaction != '' && GameData.phaseMoment == "choosing battle" ): #/player_id/phase_7_input/section_id/player_id
		var error = pickEnemyInputRequest.request(PlayerData.api_url + "pick_enemy/" + PlayerData.username + '/' + str(MapData.sections_dict[selectedSection.region_name]) + '/' + selectedFaction)
		if error != OK:
			push_error("ERROR: HTTP: GET_PHASE_7")
	elif (GameData.phaseMoment == "Battle Wheel"):
		
		if (selectedLeader != null && selectedDefense == null && selectedOffense == null): #/player_id/phase_7_input/number/general_name/offensive_treachery_card/defensive_treachery_card
			var error = battleInputRequest.request(PlayerData.api_url + "phase_7_input/1/" + PlayerData.username + '/' + get_node("BattleBox/VBoxContainer/HBoxForces/Input").text + '/' + selectedLeader)
			if error != OK:
				push_error("ERROR: HTTP: GET_PHASE_7")
		elif (selectedLeader != null && selectedDefense != null && selectedOffense == null) :
			var error = battleInputRequest.request(PlayerData.api_url + "phase_7_input/2/" + PlayerData.username + '/' + get_node("BattleBox/VBoxContainer/HBoxForces/Input").text + '/' + selectedLeader + '/' + selectedDefense)
			if error != OK:
				push_error("ERROR: HTTP: GET_PHASE_7")
		elif (selectedLeader != null && selectedDefense == null && selectedOffense != null):
			var str = PlayerData.api_url + "phase_7_input2/" + PlayerData.username + '/' + get_node("BattleBox/VBoxContainer/HBoxForces/Input").text + '/' + selectedLeader + '/' + selectedOffense
			print (str)
			var error = battleInputRequest.request(PlayerData.api_url + "phase_7_input2/" + PlayerData.username + '/' + get_node("BattleBox/VBoxContainer/HBoxForces/Input").text + '/' + selectedLeader + '/' + selectedOffense)
			if error != OK:
				push_error("ERROR: HTTP: GET_PHASE_7")
		elif (selectedLeader != null && selectedDefense != null && selectedOffense != null):
			var error = battleInputRequest.request(PlayerData.api_url + "phase_7_input/3/" + PlayerData.username + '/' + get_node("BattleBox/VBoxContainer/HBoxForces/Input").text + '/' + selectedLeader + '/' + selectedOffense + '/' + selectedDefense)
			if error != OK:
				push_error("ERROR: HTTP: GET_PHASE_7")
