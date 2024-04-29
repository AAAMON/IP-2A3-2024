extends Node



func _ready():
	# Make a request to get player data when the scene is ready
	if (PlayerData.connected == false):
		connect_to_api()
		PlayerData.connected = true

func connect_to_api():
	var request = HTTPRequest.new()
	request.connect("request_completed", _on_connect_to_api_completed)
	add_child(request)
	var error = request.request(PlayerData.api_url + "connect")
	if error != OK:
		push_error("ERROR: HTTP: GET_PLAYER_DATA")

func _on_connect_to_api_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json["message"] == "ok!"):
		print("Successfully connected to API.")
	else:
		print("ERROR: CAN'T CONNECT TO API.")
