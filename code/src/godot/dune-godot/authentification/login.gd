extends Control


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass


func _on_cancel_pressed():
	queue_free()


func _on_login_pressed():
	var username = get_node("NinePatchRect/VBoxContainer/Username")
	var password = get_node("NinePatchRect/VBoxContainer/Password1")
	var loginRequest = HTTPRequest.new()
	loginRequest.connect("request_completed", _on_login_completed)
	add_child(loginRequest)
	var error = loginRequest.request(PlayerData.api_url + "login/" + username.get_text() + "/" + password.get_text())
	if error != OK:
		push_error("ERROR: HTTP: LOGIN")

func _on_login_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json["username"] == "full"):
		print("Lobby is full.")
	else:
		print("Successfully logged in as " + json["username"] + "!")
		PlayerData.loggedIn = true
		PlayerData.username = json["username"]
		queue_free()
