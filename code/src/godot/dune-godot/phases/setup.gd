extends Control
var beneInputRequest
var traitorInputRequest
var fremenInputRequest

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
	MapData.selectedRegion = null;
		

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var message = get_node("MessageBox/Description")
	message.text = GameData.phaseMoment
	if (GameData.phaseMoment == "Bene Gesserit prediction" && PlayerData.faction == 2):
		get_node("InputBox").show()
		get_node("InputBox/Title").text = "Bene prediction"
	if (GameData.phaseMoment == "traitor selection"):
		get_node("InputBox/Title").text = "Traitor Selection"
		if (PlayerData.faction != 6):
			get_node("InputBox").show()
	if (GameData.phaseMoment == "initial faction forces placement" && PlayerData.faction != 4):
		get_node("InputBox/Title").text = "Fremen Forces"
		get_node("InputBox").hide()
	if (GameData.phaseMoment == "initial faction forces placement" && PlayerData.faction == 4 && MapData.selectedRegion == null):
		get_node("InputBox").show()
		get_node("MessageBox/Description").text = "Fremen! Pick a section to send your troops."
	if (GameData.phaseMoment == "initial faction forces placement" && PlayerData.faction == 4 && MapData.selectedRegion != null):
		get_node("MessageBox/Description").text = "Fremen! How many troops to send there? (max 10)."
		get_node("InputBox/Input").editable = true
		get_node("InputBox").show()


func _on_submit_pressed():
	var playerInput = get_node("InputBox/Input")
	if (GameData.phaseMoment == "Bene Gesserit prediction" && PlayerData.faction == 2):
		var error = beneInputRequest.request(PlayerData.api_url + "bene_predict_input/" + PlayerData.username + "/" + playerInput.text)
		if error != OK:
			push_error("ERROR: HTTP: GET_BENE_PREDICT")
	if (GameData.phaseMoment == "traitor selection" && PlayerData.faction != 6):
		var error = traitorInputRequest.request(PlayerData.api_url + "traitor_select_input/" + PlayerData.username + "/" + PlayerData.leaders_dict[int(playerInput.text)])
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
