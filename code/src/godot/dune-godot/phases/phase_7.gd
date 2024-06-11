extends Control
var requestCompleted: bool = true
var pickEnemyInputRequest
var phase7InfoRequest
@onready var timer: Timer = $Timer

var battleSections = null
var currentBattleSection = null
var enemy = null
var aggressor = null
var selectedSection = null
# Called when the node enters the scene tree for the first time.
func _ready():
	pickEnemyInputRequest = HTTPRequest.new()
	pickEnemyInputRequest.connect("request_completed", _on_pick_enemy_input_request_completed)
	add_child(pickEnemyInputRequest)
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
	if (PlayerData.myTurn == true && GameData.phaseMoment == "choosing battle" && selectedSection == null):
		#get_node("EnemyBox/Description").text = "Pick location to battle: "
		var mes = "Pick location to battle: "
		if (battleSections != null):
			for section in battleSections:
				mes = mes + MapData.sections_goofy_dict[int(section)] + ' '
			get_node("EnemyBox/Description").text = mes
	if (PlayerData.myTurn == true && GameData.phaseMoment == "choosing battle" && selectedSection != null):
		var mes = "Pick faction to battle: "
		# find factions from each
		if (battleSections != null):
			for section in battleSections:
				mes = mes + MapData.sections_goofy_dict[int(section)] + ' '
			get_node("EnemyBox/Description").text = mes


func _on_submit_pressed():
	if (selectedSection == null):
		selectedSection = MapData.selectedRegion
