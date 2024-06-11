extends Control

var requestCompleted: bool = true
var shipmentInputRequest
var beneInputRequest
var shipped : bool = false
var from = null
var to = null
# Called when the node enters the scene tree for the first time.
func _ready():
	shipmentInputRequest = HTTPRequest.new()
	shipmentInputRequest.connect("request_completed", _on_shimpent_input_request_completed)
	add_child(shipmentInputRequest)
	beneInputRequest = HTTPRequest.new()
	beneInputRequest.connect("request_completed", _on_bene_input_request_completed)
	add_child(beneInputRequest)



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if (PlayerData.myTurn == false):
		get_node("MessageBox/Description").text = "Not your turn to send forces."
		get_node("MessageBox/Skip").disabled = true;
		get_node("InputBox").hide()
		get_node("InputBoxBene").hide()
	elif (PlayerData.myTurn && GameData.phaseMoment == "waiting for bene input..."):
		get_node("MessageBox/Description").text = "Send one force?"
		get_node("InputBoxBene").show()
		get_node("MessageBox/Skip").disabled = true;
		get_node("InputBox").hide()
	elif (PlayerData.myTurn == true && MapData.selectedRegion == null && shipped == false):
		get_node("InputBoxBene").hide()
		get_node("MessageBox/Description").text = "Select where to send forces or skip."
		get_node("MessageBox/Skip").disabled = false;
		get_node("InputBox").hide()
	elif (PlayerData.myTurn == true && MapData.selectedRegion != null && shipped == false):
		get_node("InputBoxBene").hide()
		get_node("MessageBox/Description").text = "Enter number of forces or cancel."
		get_node("MessageBox/Skip").disabled = false;
		get_node("InputBox").show()
	elif (PlayerData.myTurn == true && from == null && shipped == true):
		get_node("MessageBox/Description").text = "Select forces to move."
		get_node("MessageBox/Select").show()
		get_node("MessageBox/Skip").disabled = false;
		get_node("InputBox").hide()
	elif (PlayerData.myTurn == true && shipped == true && from != null && to == null):
		get_node("MessageBox/Description").text = "Select where to move forces."
		get_node("MessageBox/Skip").disabled = false;
		get_node("InputBox").hide()
	elif (PlayerData.myTurn == true && shipped == true && from != null && to != null):
		get_node("MessageBox/Description").text = "How many forces to move?"
		get_node("MessageBox/Skip").disabled = false;
		get_node("InputBox").show()
	#print(from) 
	#print ("to")
	#print (to)


func _on_skip_pressed():
	var playerInput = get_node("InputBox/Input")
	var error = shipmentInputRequest.request(PlayerData.api_url + "phase_6_input/" + PlayerData.username + '/pass')
	if error != OK:
		push_error("ERROR: HTTP: GET_TRAITOR_SELECT")


func _on_submit_pressed():
	if (shipped == false):
		var playerInput = get_node("InputBox/Input")
		var error = shipmentInputRequest.request(PlayerData.api_url + "phase_6_input/1/" + PlayerData.username + '/' + str(MapData.sections_dict[MapData.selectedRegion.region_name]) + "/" + playerInput.text)
		if error != OK:
			push_error("ERROR: HTTP: SHIP")
		shipped = true;
	elif (shipped == true):
		var playerInput = get_node("InputBox/Input")
		var error = shipmentInputRequest.request(PlayerData.api_url + "phase_6_input/2/" + PlayerData.username + '/' +  str(MapData.sections_dict[from.region_name]) + '/' + str(MapData.sections_dict[MapData.selectedRegion.region_name]) + "/" + playerInput.text)
		if error != OK:
			push_error("ERROR: HTTP: MOVE")
	MapData.selectedRegion = null

func _on_shimpent_input_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got shipment input confirmation")
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

func _on_select_pressed():
	if (from == null) : 
		from = MapData.selectedRegion
	elif (to == null) : 
		to = MapData.selectedRegion


func _on_yes_pressed():
	var error = beneInputRequest.request(PlayerData.api_url + "phase_6_input/" + PlayerData.username + '/y')
	if error != OK:
		push_error("ERROR: HTTP: MOVE")


func _on_no_pressed():
	var error = beneInputRequest.request(PlayerData.api_url + "phase_6_input/" + PlayerData.username + '/n')
	if error != OK:
		push_error("ERROR: HTTP: SHIP")
