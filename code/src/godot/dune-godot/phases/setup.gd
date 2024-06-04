extends Control


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var message = get_node("MessageBox/Description")
	message.text = GameData.phaseMoment
	if (GameData.phaseMoment == "Bene Gesserit prediction" && PlayerData.faction == 2):
		get_node("InputBox").show()
		get_node("InputBox/Title").text = "Bene prediction"
	if (GameData.phaseMoment == "traitor selection"):
		get_node("InputBox/Title").text = "Traitor Selection"
		get_node("InputBox").show()


func _on_submit_pressed():
		# PHASE INPUT REQUEST ###################################################
	var beneInputRequest = HTTPRequest.new()
	beneInputRequest.connect("request_completed", _on_bene_input_request_completed)
	add_child(beneInputRequest)
	var playerInput = get_node("InputBox/Input")
	var error = beneInputRequest.request(PlayerData.api_url + "bene_predict_input/" + PlayerData.username + "/" + playerInput.text)
	if error != OK:
		push_error("ERROR: HTTP: GET_PHASE_1_INFO")

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
