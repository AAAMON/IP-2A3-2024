extends Control
var beneInputRequest
var traitorInputRequest
var fremenInputRequest
var traitorSelected : bool = false
var benePickedUser

# Called when the node enters the scene tree for the first time.
func _ready():
	beneInputRequest = HTTPRequest.new()
	beneInputRequest.connect("request_completed", _on_bene_input_request_completed)
	add_child(beneInputRequest)
	traitorInputRequest = HTTPRequest.new()
	traitorInputRequest.connect("request_completed", _on_traitor_input_request_completed)
	add_child(traitorInputRequest)
	if (PlayerData.faction == 4):
		fremenInputRequest = HTTPRequest.new()
		fremenInputRequest.connect("request_completed", _on_fremen_input_request_completed)
		add_child(fremenInputRequest)
		#get_node("InputBoxBene/VBoxContainer/HBoxContainer1/user1").text = OtherPlayersData.otherPlayers[0]["Username"]
		#get_node("InputBoxBene/VBoxContainer/HBoxContainer1/user2").text = OtherPlayersData.otherPlayers[1]["Username"]
		#get_node("InputBoxBene/VBoxContainer/HBoxContainer2/user3").text = OtherPlayersData.otherPlayers[2]["Username"]
		#get_node("InputBoxBene/VBoxContainer/HBoxContainer2/user4").text = OtherPlayersData.otherPlayers[3]["Username"]
		#get_node("InputBoxBene/VBoxContainer/HBoxContainer3/user5").text = OtherPlayersData.otherPlayers[4]["Username"]
		#get_node("InputBoxBene/VBoxContainer/HBoxContainer3/user6").text = OtherPlayersData.otherPlayers[5]["Username"]
	MapData.selectedRegion = null;
		

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var message = get_node("MessageBox/Description")
	message.text = GameData.phaseMoment
	if (GameData.phaseMoment == "Bene Gesserit prediction" && PlayerData.faction == 2):
		get_node("InputBox").hide()
		get_node("InputBoxBene").show()
		get_node("InputBox/Input").editable = true
		get_node("InputBox/Title").text = "Bene prediction"
	elif (GameData.phaseMoment == "Bene Gesserit prediction" && PlayerData.faction != 2):
		get_node("InputBox/Title").text = "Traitor Selection"
		get_node("InputBoxBene").hide()
		get_node("InputBox").hide()
	elif (GameData.phaseMoment == "traitor selection" && traitorSelected == false):
		get_node("InputBox/Title").text = "Traitor Selection"
		get_node("InputBoxBene").hide()
		if (PlayerData.faction != 6):
			get_node("InputBox").show()
	elif (GameData.phaseMoment == "traitor selection" && traitorSelected == true):
		get_node("InputBox/Title").text = "Traitor Selection"
		if (PlayerData.faction != 6):
			get_node("InputBox").hide()
	elif (GameData.phaseMoment == "initial faction forces placement" && PlayerData.faction != 4):
		get_node("InputBox/Title").text = "Fremen Forces"
		get_node("InputBox").hide()
	elif (GameData.phaseMoment == "initial faction forces placement" && PlayerData.faction == 4 && MapData.selectedRegion == null):
		get_node("InputBox").hide()
		get_node("MessageBox/Description").text = "Fremen! Pick a section to send your troops."
	elif (GameData.phaseMoment == "initial faction forces placement" && PlayerData.faction == 4 && MapData.selectedRegion != null):
		get_node("MessageBox/Description").text = "Fremen! How many troops to send there? (max 10)."
		get_node("InputBox/Input").editable = true
		get_node("InputBox").show()


func _on_submit_pressed():
	var playerInput = get_node("InputBox/Input")
	if (GameData.phaseMoment == "traitor selection" && PlayerData.faction != 6):
		var error = traitorInputRequest.request(PlayerData.api_url + "traitor_select_input/" + PlayerData.username + "/" + PlayerData.leaders_dict[int(playerInput.text)])
		traitorSelected = true
		if error != OK:
			push_error("ERROR: HTTP: GET_TRAITOR_SELECT")
	if (GameData.phaseMoment == "initial faction forces placement" && PlayerData.faction == 4):
		var error = traitorInputRequest.request(PlayerData.api_url + "fremen_setup_input/" + str(MapData.sections_dict[MapData.selectedRegion.region_name]) + "/" + playerInput.text)
		if error != OK:
			push_error("ERROR: HTTP: GET_TRAITOR_SELECT")

func _on_fremen_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got fremen input confirmation")
		# on first request, see if player needs to input anything
		if (json["message"] == "ok"):
			print("OK!")

func _on_traitor_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got traitor input confirmation")
		# on first request, see if player needs to input anything
		if (json["message"] == "ok"):
			print("OK!")

func _on_bene_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got bene input confirmation")
		# on first request, see if player needs to input anything
		if (json["message"] == "ok"):
			print("OK!")


func _on_submit_bene_pressed():
	var playerInput = get_node("InputBoxBene/VBoxContainer/HBoxContainer/InputBene")
	if (GameData.phaseMoment == "Bene Gesserit prediction" && PlayerData.faction == 2):
		var error = beneInputRequest.request(PlayerData.api_url + "bene_predict_input/" + PlayerData.username + "/" + str(benePickedUser) + '/' + playerInput.text)
		if error != OK:
			push_error("ERROR: HTTP: GET_BENE_PREDICT")


func _on_user_1_pressed():
	benePickedUser = 1;


func _on_user_2_pressed():
	benePickedUser = 2;


func _on_user_3_pressed():
	benePickedUser = 3;


func _on_user_4_pressed():
	benePickedUser = 4;


func _on_user_5_pressed():
	benePickedUser = 5;


func _on_user_6_pressed():
	benePickedUser = 6;
